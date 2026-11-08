using Orders.API.Constants;
using Orders.API.DTOs;
using Orders.API.Exceptions;
using Orders.API.Http;
using Orders.API.Models;

namespace Orders.API.Services;

public class OrderService(
    UsersClient usersClient,
    ProductsClient productsClient,
    ILogger<OrderService> logger) : IOrderService
{
    // Guardamos las ordenes en memoria hasta que este la libreria de la catedra.
    private readonly List<Order> _orders = [];
    private readonly Lock _candado = new();

    public async Task<OrderResponse> CreateAsync(CreateOrderRequest request, CancellationToken ct)
    {
        if (!await usersClient.ExistsAsync(request.UsuarioId, ct))
            throw new NotFoundException(ErrorCodes.ORD_003, "Usuario no encontrado al crear la orden.");

        // Se suman las cantidades por producto: si el mismo producto viene repetido en
        // varios items, el stock tiene que alcanzar para el total y no para cada linea.
        var cantidadPedida = new Dictionary<Guid, int>();
        foreach (var item in request.Items)
        {
            if (cantidadPedida.ContainsKey(item.ProductoId))
            {
                cantidadPedida[item.ProductoId] += item.Cantidad;
            }
            else
            {
                cantidadPedida[item.ProductoId] = item.Cantidad;
            }
        }

        var precios = new Dictionary<Guid, decimal>();

        foreach (var (productoId, cantidad) in cantidadPedida)
        {
            var producto = await productsClient.GetByIdAsync(productoId, ct)
                ?? throw new NotFoundException(ErrorCodes.ORD_004, "Producto no encontrado al crear la orden.");

            if (producto.Stock < cantidad)
                throw new BusinessRuleException(ErrorCodes.ORD_005,
                    $"Stock insuficiente para '{producto.Nombre}'. " +
                    $"Disponible: {producto.Stock}, solicitado: {cantidad}.", 422);

            precios[productoId] = producto.Precio;
        }

        var items = new List<OrderItem>();
        foreach (var i in request.Items)
        {
            items.Add(new OrderItem
            {
                ProductoId = i.ProductoId,
                Cantidad = i.Cantidad,
                PrecioUnitario = precios[i.ProductoId]
            });
        }

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UsuarioId = request.UsuarioId,
            Items = items,
            Estado = OrderStates.Pendiente,
            FechaCreacion = DateTime.UtcNow
        };

        var total = 0m;
        foreach (var i in order.Items)
        {
            total += i.Cantidad * i.PrecioUnitario;
        }
        order.Total = total;

        lock (_candado)
        {
            _orders.Add(order);
        }

        logger.LogInformation("Orden creada. Id: {OrdenId}, usuario: {UsuarioId}, total: {Total}",
            order.Id, order.UsuarioId, order.Total);

        return Map(order);
    }

    public List<OrderResponse> GetAll(Guid? usuarioId)
    {
        lock (_candado)
        {
            var filtradas = new List<Order>();
            foreach (var o in _orders)
            {
                if (usuarioId is null || o.UsuarioId == usuarioId)
                {
                    filtradas.Add(o);
                }
            }

            // Se ordenan de la mas nueva a la mas vieja (burbuja simple, alcanza
            // con la cantidad de ordenes que maneja este servicio en memoria).
            for (var i = 0; i < filtradas.Count - 1; i++)
            {
                for (var j = 0; j < filtradas.Count - 1 - i; j++)
                {
                    if (filtradas[j].FechaCreacion < filtradas[j + 1].FechaCreacion)
                    {
                        var temp = filtradas[j];
                        filtradas[j] = filtradas[j + 1];
                        filtradas[j + 1] = temp;
                    }
                }
            }

            var resultado = new List<OrderResponse>();
            foreach (var o in filtradas)
            {
                resultado.Add(Map(o));
            }

            return resultado;
        }
    }

    public OrderResponse GetById(Guid id)
    {
        lock (_candado)
        {
            Order? order = null;
            foreach (var o in _orders)
            {
                if (o.Id == id)
                {
                    order = o;
                    break;
                }
            }

            if (order is null)
                throw new NotFoundException(ErrorCodes.ORD_001, "Orden no encontrada.");

            return Map(order);
        }
    }

    public OrderStatusResponse UpdateStatus(Guid id, UpdateOrderStatusRequest request)
    {
        var nuevo = request.Estado.Trim();

        if (!OrderStates.Existe(nuevo))
            throw new ValidationException(ErrorCodes.ORD_002,
                $"El estado '{nuevo}' no existe. Valores validos: {string.Join(", ", OrderStates.Todos)}.");

        nuevo = OrderStates.Normalizar(nuevo);

        lock (_candado)
        {
            Order? order = null;
            foreach (var o in _orders)
            {
                if (o.Id == id)
                {
                    order = o;
                    break;
                }
            }

            if (order is null)
                throw new NotFoundException(ErrorCodes.ORD_001, "Orden no encontrada.");

            if (!OrderStates.PuedePasarA(order.Estado, nuevo))
                throw new BusinessRuleException(ErrorCodes.ORD_006,
                    $"Una orden en estado '{order.Estado}' no puede pasar a '{nuevo}'.",
                    409, "No se puede modificar el estado.");

            order.Estado = nuevo;
            order.FechaActualizacion = DateTime.UtcNow;

            logger.LogInformation("Orden {OrdenId} pasa a estado {Estado}", order.Id, nuevo);

            return new OrderStatusResponse
            {
                Id = order.Id,
                Estado = order.Estado,
                FechaActualizacion = order.FechaActualizacion.Value
            };
        }
    }

    public bool HasActiveOrders(Guid productoId)
    {
        lock (_candado)
        {
            foreach (var o in _orders)
            {
                if (!OrderStates.Activos.Contains(o.Estado)) continue;

                foreach (var i in o.Items)
                {
                    if (i.ProductoId == productoId)
                        return true;
                }
            }

            return false;
        }
    }

    private static OrderResponse Map(Order order)
    {
        var items = new List<OrderItemResponse>();
        foreach (var i in order.Items)
        {
            items.Add(new OrderItemResponse
            {
                ProductoId = i.ProductoId,
                Cantidad = i.Cantidad,
                PrecioUnitario = i.PrecioUnitario
            });
        }

        return new OrderResponse
        {
            Id = order.Id,
            UsuarioId = order.UsuarioId,
            Items = items,
            Total = order.Total,
            Estado = order.Estado,
            FechaCreacion = order.FechaCreacion
        };
    }
}

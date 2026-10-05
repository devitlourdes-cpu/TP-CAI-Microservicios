## Products.API

Microservicio de catálogo de productos del e-commerce. Permite listar, consultar, crear, actualizar y eliminar productos.

| Dato | Valor |
|---|---|
| Puerto HTTP | 65085 |
| Puerto HTTPS | 65084 |
| Swagger | https://localhost:65084/swagger |
| Persistencia | En memoria (los datos se pierden al reiniciar el servicio) |
| Logs | Consola y archivo `logs/products-api-<fecha>.json` |

### Cómo ejecutarlo

Desde la raíz del repositorio:

```bash
dotnet run --project src/Products.API
```

También se puede ejecutar desde Visual Studio, con Products.API como proyecto de inicio.

### Endpoints

| Método | Ruta | Descripción | Respuestas |
|---|---|---|---|
| GET | /api/products | Lista productos (filtros `?categoria=` y `?nombre=`) | 200, 500 |
| GET | /api/products/{id} | Obtiene un producto por ID | 200, 404, 500 |
| POST | /api/products | Crea un producto | 201, 400, 409, 500 |
| PUT | /api/products/{id} | Actualiza un producto | 200, 400, 404, 500 |
| DELETE | /api/products/{id} | Elimina un producto | 204, 404, 409, 500 |

### Modelo de producto

| Campo | Tipo | Regla |
|---|---|---|
| id | GUID | Lo genera el servicio |
| nombre | texto | Obligatorio, máximo 100 caracteres |
| descripcion | texto | Opcional |
| precio | decimal | Mayor a 0 |
| stock | entero | Mayor o igual a 0 |
| categoria | texto | Obligatoria |
| fechaCreacion | fecha | La genera el servicio |

### Códigos de error

| Código | HTTP | Mensaje | Cuándo ocurre |
|---|---|---|---|
| PRD-001 | 404 | Producto no encontrado. | GET, PUT o DELETE con un ID que no existe |
| PRD-002 | 400 | Los datos del producto son inválidos. | POST o PUT con campos faltantes o con formato incorrecto |
| PRD-003 | 409 | Ya existe un producto con ese nombre en la categoría. | POST de un producto duplicado |
| PRD-004 | 409 | El producto tiene órdenes activas y no puede eliminarse. | DELETE con órdenes Pendientes o Confirmadas (requiere Orders.API) |
| PRD-005 | 500 | Error interno al procesar el producto. | Error inesperado |

Todos los errores tienen este formato:

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "detail": "El recurso solicitado no fue encontrado.",
  "instance": "/api/products/3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "errorCode": "PRD-001",
  "errorMessage": "Producto no encontrado.",
  "correlationId": "3e80c6ce-68ea-4e3b-9952-e6e1a5d19379"
}
```

### Health checks

| Ruta | Qué verifica |
|---|---|
| /health | Estado general del servicio |
| /health/live | El proceso está vivo |
| /health/ready | El servicio está listo para recibir tráfico |

### Correlation ID

Cada request lleva un header `X-Correlation-Id`. Si el cliente no lo envía, el servicio genera uno. Aparece en la respuesta, en el cuerpo de los errores y en todos los logs de ese request.
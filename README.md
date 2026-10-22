# TP-CAI-Microservicios

TP de Construcción de Aplicaciones Informáticas — E-commerce con microservicios en .NET 10.

## Requisitos

SDK de .NET 10.

## Levantar todo

```bash
./scripts/run-all.sh
```

Compila y levanta todos los microservicios de `src/`. Ctrl+C para bajarlos.
En Windows se corre desde Git Bash.

## Levantar uno solo

```bash
dotnet run --project src/Users.API
```

## Compilar

```bash
dotnet build ECommerce.sln
```

## Puertos

| Servicio | HTTP | HTTPS | Swagger |
|---|---|---|---|
| Products.API | 65085 | 65084 | http://localhost:65085/swagger |
| Users.API | 5002 | 7002 | http://localhost:5002/swagger |
| Orders.API | 5003 | 7003 | http://localhost:5003/swagger |

---

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

---

## Users.API

Microservicio de usuarios del e-commerce. Permite registrar usuarios, autenticarlos y consultarlos por id.

| Dato | Valor |
|---|---|
| Puerto HTTP | 5002 |
| Puerto HTTPS | 7002 |
| Swagger | http://localhost:5002/swagger |
| Persistencia | En memoria (los datos se pierden al reiniciar el servicio) |
| Logs | Consola y archivo `logs/users-api-<fecha>.json` |

### Cómo ejecutarlo

Desde la raíz del repositorio:

```bash
dotnet run --project src/Users.API
```

### Endpoints

| Método | Ruta | Descripción | Respuestas |
|---|---|---|---|
| GET | /api/users/{id} | Obtiene un usuario por id | 200, 404, 500 |
| POST | /api/users/register | Registra un usuario | 201, 400, 409, 500 |
| POST | /api/users/login | Autentica un usuario | 200, 400, 401, 403, 500 |

### Modelo de usuario

| Campo | Tipo | Regla |
|---|---|---|
| id | GUID | Lo genera el servicio |
| nombre | texto | Obligatorio |
| apellido | texto | Obligatorio |
| email | texto | Obligatorio, único y con formato válido |
| passwordHash | texto | Hash de la contraseña, nunca se devuelve en una respuesta |
| fechaRegistro | fecha | La genera el servicio |
| activo | booleano | false cuando la cuenta está bloqueada |
| intentosFallidos | entero | Sube con cada login fallido, se resetea al entrar bien |

### Regla de bloqueo

Al tercer intento fallido consecutivo la cuenta pasa a `activo = false` y el login empieza
a responder 403. Un login correcto resetea el contador.

### Códigos de error

| Código | HTTP | Mensaje | Cuándo ocurre |
|---|---|---|---|
| USR-001 | 409 | El email ya está registrado. | POST /register con un email que ya existe |
| USR-002 | 400 | Los datos del usuario son inválidos. | POST con campos faltantes o con formato incorrecto |
| USR-003 | 401 | Credenciales incorrectas. | POST /login cuando el email o la password no coinciden |
| USR-004 | 403 | Usuario bloqueado por demasiados intentos fallidos. | POST /login con 3 o más intentos fallidos |
| USR-005 | 403 | Usuario bloqueado por detección de fraude. | POST /login con la cuenta desactivada manualmente |
| USR-006 | 500 | Error interno al procesar el usuario. | Error inesperado |
| USR-007 | 404 | Usuario no encontrado. | GET /api/users/{id} con un id que no existe |

---

## Orders.API

Microservicio de órdenes del e-commerce. Al crear una orden valida el usuario contra Users.API, y los productos y su stock contra Products.API.

| Dato | Valor |
|---|---|
| Puerto HTTP | 5003 |
| Puerto HTTPS | 7003 |
| Swagger | http://localhost:5003/swagger |
| Persistencia | En memoria (los datos se pierden al reiniciar el servicio) |
| Logs | Consola y archivo `logs/orders-api-<fecha>.json` |
| Depende de | Users.API y Products.API |

### Cómo ejecutarlo

Necesita Users.API y Products.API levantados. Lo más simple es `./scripts/run-all.sh`.

```bash
dotnet run --project src/Orders.API
```

Las URLs de los servicios que consume salen de la sección `Services` de `appsettings.json`.

### Endpoints

| Método | Ruta | Descripción | Respuestas |
|---|---|---|---|
| GET | /api/orders | Lista órdenes (filtro `?usuarioId=`) | 200, 500 |
| GET | /api/orders/{id} | Obtiene una orden por ID | 200, 404, 500 |
| POST | /api/orders | Crea una orden | 201, 400, 404, 422, 500 |
| PUT | /api/orders/{id}/status | Actualiza el estado de una orden | 200, 400, 404, 409, 500 |
| GET | /api/orders/product/{productId}/active | Indica si un producto tiene órdenes en curso | 200, 500 |

### Modelo de orden

| Campo | Tipo | Regla |
|---|---|---|
| id | GUID | Lo genera el servicio |
| usuarioId | GUID | Tiene que existir en Users.API |
| items | lista | Al menos un item |
| total | decimal | Lo calcula el servicio sumando cantidad por precio unitario |
| estado | texto | Pendiente, Confirmada, Enviada, Entregada o Cancelada |
| fechaCreacion | fecha | La genera el servicio |
| fechaActualizacion | fecha | Se setea al cambiar el estado |

Cada item tiene:

| Campo | Tipo | Regla |
|---|---|---|
| productoId | GUID | Tiene que existir en Products.API |
| cantidad | entero | Mayor a 0 |
| precioUnitario | decimal | Se copia del producto al crear la orden |

### Códigos de error

| Código | HTTP | Mensaje | Cuándo ocurre |
|---|---|---|---|
| ORD-001 | 404 | Orden no encontrada. | GET o PUT con un id de orden que no existe |
| ORD-002 | 400 | Los datos de la orden son inválidos. | POST con campos faltantes o lista de items vacía |
| ORD-003 | 404 | Usuario no encontrado al crear la orden. | POST cuando el usuarioId no existe en Users.API |
| ORD-004 | 404 | Producto no encontrado al crear la orden. | POST cuando algún productoId no existe en Products.API |
| ORD-005 | 422 | Stock insuficiente para uno o más productos. | POST cuando la cantidad pedida supera el stock |
| ORD-006 | 409 | El estado de la orden no puede ser modificado. | PUT /status con una transición inválida |
| ORD-007 | 500 | Error interno al procesar la orden. | Error inesperado |

### Transiciones de estado

| Desde | Hacia |
|---|---|
| Pendiente | Confirmada, Cancelada |
| Confirmada | Enviada, Cancelada |
| Enviada | Entregada |
| Entregada | — |
| Cancelada | — |

Cualquier otra combinación responde 409 con ORD-006.

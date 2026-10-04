# Products.API - Microservicio de Gestión de Productos

Este microservicio forma parte del sistema de E-Commerce y se encarga de la administración del catálogo de productos.

##  Tecnologías y Arquitectura
- **Framework:** .NET 10 Web API
- **Documentación:** Swagger / OpenAPI con anotaciones de respuestas
- **Manejo de Errores:** `IExceptionHandler` nativo con `ProblemDetails` estandarizados
- **Logging:** Serilog (Consola y archivos JSON estructurados)
- **Observabilidad:** Health Checks y Middleware de Correlation ID (`X-Correlation-Id`)

---

##  Endpoints del Catálogo

| Método | Endpoint | Descripción | Códigos HTTP |
| :--- | :--- | :--- | :--- |
| **GET** | `/api/products` | Lista productos (filtros opcionales `?categoria=` y `?nombre=`) | 200, 500 |
| **GET** | `/api/products/{id}` | Obtiene un producto por su ID | 200, 404, 500 |
| **POST** | `/api/products` | Crea un nuevo producto | 201, 400, 409, 500 |
| **PUT** | `/api/products/{id}` | Actualiza un producto existente | 200, 400, 404, 500 |
| **DELETE** | `/api/products/{id}` | Elimina un producto por su ID | 204, 404, 409, 500 |

---

##  Catálogo de Errores Manejados
- `PRD-001`: Producto no encontrado (HTTP 404).
- `PRD-002`: Datos de entrada inválidos / Error de validación (HTTP 400).
- `PRD-003`: Conflicto de producto duplicado en la misma categoría (HTTP 409).
- `PRD-004`: Regla de negocio al intentar eliminar un producto con órdenes activas (HTTP 409).
- `PRD-005`: Error interno del servidor / Inesperado (HTTP 500).

---

##  Endpoints de Salud (Health Checks)
- `/health`: Estado general del microservicio.
- `/health/live`: Liveness probe.
- `/health/ready`: Readiness probe.
# Changelog

Registro de los cambios más importantes del Trabajo Práctico Integrador (E-commerce con Microservicios).

## [Entrega final] - 2026-11-09

### Agregado

- 22 capturas de Swagger UI que documentan el catálogo completo de códigos de error de los 5 servicios (sección 9.2 del enunciado), guardadas en `docs/screenshots/` y referenciadas desde el README en la sección "Capturas de Swagger — Catálogo de errores".
- `Constants/ErrorCodes.cs` en Products.API, centralizando los códigos de error en constantes en vez de strings sueltos ("PRD-001", etc.) repetidos en el código.

### Corregido

- Cart.API y Orders.API usaban los mismos puertos (5003 y 7003), lo que impedía levantar los dos servicios al mismo tiempo. Se le asignaron puertos propios a Cart.API (5005 y 7005).
- La regla PRD-004 ("no se puede borrar un producto con órdenes activas") no se aplicaba de verdad: el código existía pero nunca se llamaba desde donde correspondía. Se corrigió para que Products.API le pregunte a Orders.API antes de eliminar un producto.
- Faltaba el campo `fechaActualizacion` en la respuesta del carrito, pedido por el enunciado. Se agregó y se actualiza cada vez que se modifica el carrito.
- Había un endpoint duplicado (`GET /api/users/{id}`), agregado sin querer en dos partes del proyecto a la vez. Al combinar el trabajo del equipo se mantuvo la versión correcta (la que incluye el login).

### Cambiado

- Se reemplazaron expresiones LINQ y funciones lambda por recorridos explícitos (`foreach`/`if`) y métodos con nombre propio en los 5 servicios. Se mantuvieron sin tocar, a propósito, las expresiones `switch` usadas para armar los ejemplos de error de Swagger (por ejemplo en `ErrorResponseWriter.cs`): no son lambdas ni tienen relación con LINQ, son solo otra forma de escribir un `switch` común.
- Se resolvieron en conjunto, sin perder trabajo de ningún integrante, los conflictos de Git generados al combinar la simplificación de código con el catálogo de errores de Products.API y los ejemplos de error agregados a la documentación de Swagger en Cart.API, Notifications.API y Users.API.

### Verificado

- Prueba end-to-end con los 5 microservicios levantados simultáneamente (múltiples proyectos de inicio en Visual Studio): flujo completo de registro de usuario, creación de producto, carrito, orden y notificación, además de los casos de error de negocio (PRD-004, ORD-006) y los distintos códigos del catálogo de cada servicio.
- Build sin errores (0 errores; 2 advertencias sin relación con estos cambios, sobre la versión del SDK de .NET y una alerta de seguridad de una librería).

using Microsoft.AspNetCore.Mvc;
using Users.API.DTOs;
using Users.API.Services;

namespace Users.API.Controllers;

/// <summary>Registro y autenticacion de usuarios.</summary>
[ApiController]
[Route("api/users")]
[Tags("Users")]
[Produces("application/json")]
public class UsersController(IUserService userService) : ControllerBase
{
    /// <summary>Registra un nuevo usuario.</summary>
    /// <remarks>
    /// El email no puede repetirse. La password se guarda hasheada y nunca se devuelve.
    ///
    ///     POST /api/users/register
    ///     {
    ///       "nombre": "María",
    ///       "apellido": "González",
    ///       "email": "maria@email.com",
    ///       "password": "MiPassword123!"
    ///     }
    ///
    /// </remarks>
    /// <response code="201">Usuario registrado.</response>
    /// <response code="400">Datos invalidos (USR-002).</response>
    /// <response code="409">El email ya esta registrado (USR-001).</response>
    /// <response code="500">Error interno (USR-006).</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterUserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public ActionResult<RegisterUserResponse> Register([FromBody] RegisterUserRequest request)
    {
        var response = userService.Register(request);

        return Created($"/api/users/{response.Id}", response);
    }

    /// <summary>Obtiene un usuario por su Id.</summary>
    /// <remarks>
    /// Lo usan otros microservicios (por ejemplo Notifications.API) para validar que
    /// un usuario exista antes de operar sobre el.
    ///
    ///     GET /api/users/a1b2c3d4-0000-0000-0000-111122223333
    ///
    /// </remarks>
    /// <response code="200">Usuario encontrado.</response>
    /// <response code="404">No existe un usuario con ese Id (USR-007).</response>
    /// <response code="500">Error interno (USR-006).</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(RegisterUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public ActionResult<RegisterUserResponse> GetById(Guid id)
    {
        var response = userService.GetById(id);

        return Ok(response);
    }
}
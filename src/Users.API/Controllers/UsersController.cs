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

    /// <summary>Autentica un usuario.</summary>
    /// <remarks>
    /// Al tercer intento fallido consecutivo la cuenta se bloquea y pasa a responder 403.
    /// Un login correcto borra los intentos acumulados.
    ///
    ///     POST /api/users/login
    ///     {
    ///       "email": "maria@email.com",
    ///       "password": "MiPassword123!"
    ///     }
    ///
    /// </remarks>
    /// <response code="200">Usuario autenticado.</response>
    /// <response code="400">Datos invalidos (USR-002).</response>
    /// <response code="401">Credenciales incorrectas (USR-003).</response>
    /// <response code="403">Cuenta bloqueada por intentos fallidos (USR-004) o por fraude (USR-005).</response>
    /// <response code="500">Error interno (USR-006).</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public ActionResult<LoginUserResponse> Login([FromBody] LoginUserRequest request)
    {
        return Ok(userService.Login(request));
    }
}

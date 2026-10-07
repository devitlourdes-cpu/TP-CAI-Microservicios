using System.Security.Cryptography;
using System.Text;
using Users.API.Constants;
using Users.API.DTOs;
using Users.API.Exceptions;
using Users.API.Models;

namespace Users.API.Services;

public class UserService(ILogger<UserService> logger) : IUserService
{
    // Al llegar a esta cantidad de intentos fallidos la cuenta se bloquea.
    private const int MaxIntentosFallidos = 3;

    // Guardamos los usuarios en memoria hasta que este la libreria de la catedra.
    private readonly List<User> _users = [];
    private readonly Lock _candado = new();

    public UserResponse Register(RegisterUserRequest request)
    {
        var email = request.Email.Trim();

        lock (_candado)
        {
            if (_users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
                throw new BusinessRuleException(ErrorCodes.USR_001, $"El email '{email}' ya está registrado.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Nombre = request.Nombre.Trim(),
                Apellido = request.Apellido.Trim(),
                Email = email,
                PasswordHash = Hash(request.Password),
                FechaRegistro = DateTime.UtcNow,
                Activo = true,
                IntentosFallidos = 0
            };

            _users.Add(user);
            logger.LogInformation("Usuario registrado. Id: {UserId}", user.Id);

            return Map(user);
        }
    }

    public UserResponse GetById(Guid id)
    {
        lock (_candado)
        {
            var user = _users.FirstOrDefault(u => u.Id == id)
                ?? throw new NotFoundException(ErrorCodes.USR_007, "Usuario no encontrado.");

            return Map(user);
        }
    }

    public LoginUserResponse Login(LoginUserRequest request)
    {
        var email = request.Email.Trim();

        lock (_candado)
        {
            var user = _users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            // Si el email no existe se responde lo mismo que con password incorrecta,
            // para no revelar que emails estan registrados.
            if (user is null)
                throw new BusinessRuleException(ErrorCodes.USR_003, "Credenciales incorrectas.", 401);

            // La cuenta bloqueada se rechaza incluso con la password correcta.
            if (!user.Activo)
                throw Bloqueada(user);

            if (user.PasswordHash != Hash(request.Password))
            {
                user.IntentosFallidos++;

                if (user.IntentosFallidos >= MaxIntentosFallidos)
                {
                    user.Activo = false;
                    logger.LogWarning("Usuario bloqueado por intentos fallidos. Id: {UserId}", user.Id);
                    throw Bloqueada(user);
                }

                logger.LogWarning("Login fallido. Id: {UserId}, intentos: {Intentos}",
                    user.Id, user.IntentosFallidos);

                throw new BusinessRuleException(ErrorCodes.USR_003, "Credenciales incorrectas.", 401);
            }

            // Un login correcto borra los intentos fallidos acumulados.
            user.IntentosFallidos = 0;
            logger.LogInformation("Login correcto. Id: {UserId}", user.Id);

            return new LoginUserResponse
            {
                Id = user.Id,
                Nombre = user.Nombre,
                Apellido = user.Apellido,
                Email = user.Email
            };
        }
    }

    // USR-004 y USR-005 son los dos 403 posibles y comparten Activo en false.
    // Se distinguen por los intentos: con 3 o mas fue el bloqueo automatico,
    // si no, la cuenta la desactivaron a mano por fraude.
    private static BusinessRuleException Bloqueada(User user) =>
        user.IntentosFallidos >= MaxIntentosFallidos
            ? new BusinessRuleException(ErrorCodes.USR_004,
                "Su cuenta fue bloqueada por superar el máximo de intentos fallidos. Contacte a soporte.", 403)
            : new BusinessRuleException(ErrorCodes.USR_005,
                "Su cuenta fue suspendida por razones de seguridad. Contacte a soporte.", 403);

    // Pasa la entidad al DTO, asi el PasswordHash nunca sale en una respuesta.
    private static UserResponse Map(User user) => new()
    {
        Id = user.Id,
        Nombre = user.Nombre,
        Apellido = user.Apellido,
        Email = user.Email,
        FechaRegistro = user.FechaRegistro,
        Activo = user.Activo
    };

    // La contrasena nunca se guarda en texto plano.
    private static string Hash(string password) =>
        Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
}

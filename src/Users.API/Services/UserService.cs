using System.Security.Cryptography;
using System.Text;
using Users.API.Constants;
using Users.API.DTOs;
using Users.API.Exceptions;
using Users.API.Models;

namespace Users.API.Services;

public class UserService(ILogger<UserService> logger) : IUserService
{
    // Guardamos los usuarios en memoria hasta que este la libreria de la catedra.
    private readonly List<User> _users = [];
    private readonly Lock _candado = new();

    public RegisterUserResponse Register(RegisterUserRequest request)
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

            return Mapear(user);
        }
    }

    public RegisterUserResponse GetById(Guid id)
    {
        lock (_candado)
        {
            var user = _users.FirstOrDefault(u => u.Id == id)
                ?? throw new NotFoundException(ErrorCodes.USR_007, "Usuario no encontrado.");

            return Mapear(user);
        }
    }

    // La contrasena nunca se guarda en texto plano.
    private static string Hash(string password) =>
        Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(password)));

    private static RegisterUserResponse Mapear(User user) => new()
    {
        Id = user.Id,
        Nombre = user.Nombre,
        Apellido = user.Apellido,
        Email = user.Email,
        FechaRegistro = user.FechaRegistro,
        Activo = user.Activo
    };
}
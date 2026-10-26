using Users.API.DTOs;

namespace Users.API.Services;

public interface IUserService
{
    UserResponse Register(RegisterUserRequest request);

    LoginUserResponse Login(LoginUserRequest request);

    UserResponse GetById(Guid id);
}

using Users.API.DTOs;

namespace Users.API.Services;

public interface IUserService
{
    RegisterUserResponse Register(RegisterUserRequest request);

    LoginUserResponse Login(LoginUserRequest request);
}

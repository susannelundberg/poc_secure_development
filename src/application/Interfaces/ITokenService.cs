using domain.Entities;

namespace application.Interfaces;

public interface ITokenService
{
    Task<string> CreateToken(User user);
}

using ChattyBot.Server.Domain.Entities;

namespace ChattyBot.Server.Infrastructure.Security.Interfaces
{
    public interface ITokenGenerator
    {
        string GenerateJwtToken(User user);
    }
}

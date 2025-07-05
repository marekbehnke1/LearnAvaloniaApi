using LearnAvaloniaApi.Models;

namespace LearnAvaloniaApi.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        bool ValidateToken(string token);
        int? GetUserIdFromToken(string token);
    }
}

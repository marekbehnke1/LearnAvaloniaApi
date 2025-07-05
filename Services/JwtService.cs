using System.IdentityModel.Tokens.Jwt;
using System.Text;
using LearnAvaloniaApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
namespace LearnAvaloniaApi.Services
{
    public class JwtService : IJwtService
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly int _expirationMinutes;
        public JwtService(IConfiguration configuration)
        {
            _secretKey = configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
            _issuer = configuration["Jwt:Issuer"] ?? "LearnAvaloniaApi";
            _expirationMinutes = int.Parse(configuration["Jwt:ExpirationMinutes"] ?? "60");
        }

        public string GenerateToken(User user)
        {
            // Initiate tokenhandler
            var tokenHandler = new JwtSecurityTokenHandler();

            // encode the key
            var key = Encoding.ASCII.GetBytes(_secretKey);

            // Initiate token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("sub", user.Id.ToString()),
                    new Claim("email", user.Email),
                    new Claim("name", $"{user.FirstName} {user.LastName}")
                }),
                Expires  = DateTime.UtcNow.AddMinutes(_expirationMinutes),
                Issuer = _issuer,
                Audience = _issuer,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            // create token from descriptor
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Write the token and return it
            return tokenHandler.WriteToken(token);
        }

        public bool ValidateToken(string token)
        {
            try
            {
                // intiate token handler
                var tokenHandler = new JwtSecurityTokenHandler();

                // encode key
                var key = Encoding.ASCII.GetBytes(_secretKey);

                // set the validation parameters and call the validation method
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _issuer,
                    ValidateLifetime = true,
                    // this helps to prevent system clock manipulation attacks
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                // if the validate token method throws no exceptions, we good.
                return true;
            }
            catch
            {
                // If exception is thrown, bad token 
                return false;
            }
        }

        public int? GetUserIdFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwt = tokenHandler.ReadJwtToken(token);

                // This returns the value of the first claim that matches the condition of being equal to "sub"
                var userIdClaim = jwt.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;

                // we then attempt to parse this claim as an int
                return int.TryParse(userIdClaim, out int userId) ? userId : null;
            }
            catch
            {
                return null;
            }
        }
    }
}

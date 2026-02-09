using JWT_Token.Model;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWT_Token.Service
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

       
        public async Task<object> Login(LoginDto loginDto)
        {
            var user = _context.Users.FirstOrDefault(x => x.Username == loginDto.Username);

            if (user == null)
                return "Invalid username or password";
            // In a real application, you should hash the password and compare the hash// payload is the data we want to include in the token, it can be user id, username, role, etc
            var accessToken = GenerateJwtToken(user);

            //
            var refreshToken = GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiryDate = DateTime.Now.AddDays(7),
                IsRevoked = false
            };

            await _context.RefreshTokens.AddAsync(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<string> Register(User user)
        {
            if (user.Password != user.ConfirmPassword)
                return "Password and Confirm Password do not match";

            var existingUser = _context.Users.FirstOrDefault(u => u.Username == user.Username);

            if (existingUser != null)
                return "Username already exists";

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return "User registered successfully";
        }

        public async Task<object> RefreshToken(string refreshToken)
        {
            var storedToken = _context.RefreshTokens
                .FirstOrDefault(x => x.Token == refreshToken);

            if (storedToken == null ||
                storedToken.IsRevoked ||
                storedToken.ExpiryDate < DateTime.Now)
            {
                return "Invalid refresh token";
            }

            var user = _context.Users.FirstOrDefault(x => x.Id == storedToken.UserId);

            var newAccessToken = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();

            storedToken.IsRevoked = true;

            var newRefreshTokenEntity = new RefreshToken
            {
                Token = newRefreshToken,
                UserId = user.Id,
                ExpiryDate = DateTime.Now.AddDays(7),
                IsRevoked = false
            };

            await _context.RefreshTokens.AddAsync(newRefreshTokenEntity);
            await _context.SaveChangesAsync();

            return new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }

        #region private methods
        //payload
        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSetttings = _configuration.GetSection("Jwt");

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSetttings["Key"])
            );

            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim("userId", user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("firstName", user.FirstName),
                new Claim("lastName", user.LastName),
            };

            var token = new JwtSecurityToken(
                issuer: jwtSetttings["Issuer"],
                audience: jwtSetttings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credential
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        #endregion
    }
}

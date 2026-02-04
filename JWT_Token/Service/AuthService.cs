using JWT_Token.Model;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWT_Token.Service
{

    // w
    //Inheritance

    // Encapsulation is wrapping the data and the methods that operate on that data into a single unit, which is the class.
    // In this case, the AuthService class encapsulates the logic for user registration and authentication.
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;

        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public  async Task<string> Login(LoginDto loginDto)
        {
            var user = _context.Users.FirstOrDefault(x => x.Username == loginDto.Username);
            if (user == null)
            {
                return "Invalid username or password";
            }
            return GenerateJwtToken(user);
        }

        public async Task<string> Register(User user)
        {
            if (user.Password != user.ConfirmPassword)

                return "Password and Confirm Password do not match";

            var existingUser = _context.Users.FirstOrDefault(u => u.Username == user.Username);
            if (existingUser != null)
            {
                return "Username already exists";
            }
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return "User registered successfully";
        }



        #region private methods

        // here we can add private methods for hashing password, generating JWT token, validating user input, etc.


        // this method generates a JWT token for the authenticated user

        //header.payload.signature
        private string GenerateJwtToken(User user)
        {
            /// step 1: we can read the "jwt" section from the appsettins.json file with the help of configuration
            /// 

            var jwtSetttings= _configuration.GetSection("Jwt");

            // step 2: we can get the key part for the signature from the jwt section

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSetttings["Key"])

                );

            //step 3: we will defineher our header secttion and we will use the HmacSha256 algorithm for signing the token
            // HmacSha256 is a symmetric algorithm that uses the same key for both signing and verifying the token
            // HmacSha256 full form is Hash-based Message Authentication Code with SHA-256 hash function
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            // step 4: we will define our payload section(User claims)


            //jwt payload

            var claims = new List<Claim>
            {
                new Claim("userId", user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("firstName", user.FirstName),
                new Claim("lastName", user.LastName),

            };

          //   step  5: here we can create jwt token by combinging header and payload with an expiration time
            var token = new JwtSecurityToken(
                issuer: jwtSetttings["Issuer"],
                audience: jwtSetttings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credential
                );
            // step 6: we will return the token as a string
            return new JwtSecurityTokenHandler().WriteToken(token);

        }
        #endregion
    }
}

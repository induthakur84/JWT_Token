using JWT_Token.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JWT_Token.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

  
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("register")]

        public async Task<IActionResult> Register(User user)
        {
            var result = await _authService.Register(user);
            if (result == "User registered successfully")
            {
                return Ok(new { message = result });
            }
            else
            {
                return BadRequest(new { message = result });
            }
        }
        ////api/auth/login/username:ram/password:ram123
        ///


        //
        [Authorize]
        [HttpPost("login")]

        public async Task<IActionResult>Login([FromBody] LoginDto loginDto)
        {
            var result = await _authService.Login(loginDto);
            if (result != null)
                return Ok(result);
            return BadRequest("Invalid username or password");
        }

    }
}

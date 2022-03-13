using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using P2_JWT_Net5._0.Dtos;
using P2_JWT_Net5._0.Models;
using P2_JWT_Net5._0.Security;
using P2_JWT_Net5._0.Services;
using System.Threading.Tasks;

namespace P2_JWT_Net5._0.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // Mock database -> Data
        public static User user = new User();
        private readonly IUserService userService;
        private readonly JWTHandler jwtHandler;

        public AuthController(IUserService userService, JWTHandler passwordHashing)
        {
            this.userService = userService;
            this.jwtHandler = passwordHashing;
        }

        [HttpGet]  
        [Authorize]
        public ActionResult<string> GetMe()
        {
            var userName = userService.GetMyName();
            return Ok(userName);
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            this.jwtHandler.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.Username = request.Username;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto request)
        {
            if (user.Username != request.Username)
                return BadRequest("User not found.");

            if (!this.jwtHandler.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
                return BadRequest("Wrong password.");

            string token = this.jwtHandler.CreateJWTToken(user);
            return Ok(token);
        }
    }
}

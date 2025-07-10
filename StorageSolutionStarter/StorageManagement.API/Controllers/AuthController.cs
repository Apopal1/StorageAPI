using Microsoft.AspNetCore.Mvc;
using StorageManagement.API.Models;
using StorageManagement.API.Repositories;

namespace StorageManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public AuthController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestModel model)
        {
            if (await _userRepository.UsernameExistsAsync(model.Username))
                return BadRequest("Username already exists");

            if (await _userRepository.EmailExistsAsync(model.Email))
                return BadRequest("Email already exists");

            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Username = model.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password)
            };

            var createdUser = await _userRepository.CreateAsync(user);
            return Ok(new { Message = "User registered successfully", UserId = createdUser.Id });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel model)
        {
            var user = await _userRepository.GetByUsernameAsync(model.Username);
            
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                return Unauthorized("Invalid username or password");

            return Ok(new { Message = "Login successful", UserId = user.Id });
        }
    }
}

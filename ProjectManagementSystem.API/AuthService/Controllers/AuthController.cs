using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectManagementSystem.API.AuthService.Managers;
using ProjectManagementSystem.Common.Contracts.Auth;

namespace ProjectManagementSystem.API.AuthService.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthManager _authManager;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthManager authManager, ILogger<AuthController> logger)
        {
            _authManager = authManager;
            _logger = logger;
        }

        [HttpPost("login")]
        [AllowAnonymous]  // ✅ Allow login without authentication
        public async Task<AuthResponse> Login([FromBody] UserLoginRequest request)
        {
            _logger.LogInformation($"Received request to log in user {request.Username}");
            var token = await _authManager.SignInAsync(request.Username, request.Password);
            return new AuthResponse { Token = token };
        }
    }
}
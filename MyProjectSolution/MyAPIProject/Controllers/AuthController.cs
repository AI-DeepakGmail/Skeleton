using AuthLib.Interfaces;
using AuthLib.Models;
using Microsoft.AspNetCore.Mvc;

namespace MyAPIProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ITokenService _tokenService;

        public AuthController(
            IAuthService authService,
            IRefreshTokenService refreshTokenService,
            ITokenService tokenService)
        {
            _authService = authService;
            _refreshTokenService = refreshTokenService;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Username and password are required.");

            var result = await _authService.AuthenticateAsync(request.Username, request.Password);
            if (result == null)
                return Unauthorized();

            var refreshTokenResult = await _refreshTokenService.CreateAsync(result.User.Id);

            return Ok(new
            {
                accessToken = result.AccessToken,
                refreshToken = refreshTokenResult.RawToken,
                refreshTokenId = refreshTokenResult.StoredToken.Id,
                userId = result.User.Id,
                username = result.User.Username,
                role = result.User.Role
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Username and password are required.");

            var result = await _authService.RegisterAsync(request.Username, request.Password, request.Role ?? "User");
            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return Ok("User registered successfully.");
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
        {
            // ✅ Validate the refresh token and get full user identity
            var user = await _refreshTokenService.ValidateAsync(request.UserId, request.RefreshToken);
            if (user == null)
                return Unauthorized("Invalid or expired refresh token.");

            // ✅ Generate and store a new refresh token
            var newRefreshToken = await _refreshTokenService.CreateAsync(user.Id);

            // ✅ Revoke the old token for audit compliance
            await _refreshTokenService.RevokeAsync(request.OldTokenId, newRefreshToken.StoredToken.Id);

            // ✅ Generate a new access token
            var accessToken = _tokenService.GenerateAccessToken(user);

            // ✅ Return both tokens and metadata
            return Ok(new
            {
                accessToken,
                refreshToken = newRefreshToken.RawToken,
                refreshTokenId = newRefreshToken.StoredToken.Id,
                userId = user.Id,
                username = user.Username,
                role = user.Role
            });
        }



        public class LoginRequest
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public class RegisterRequest
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string? Role { get; set; }
        }

        public class RefreshRequest
        {
            public int UserId { get; set; }
            public string RefreshToken { get; set; } = string.Empty;
            public Guid OldTokenId { get; set; }
        }
    }
}

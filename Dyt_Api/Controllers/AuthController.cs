using System.Security.Claims;
using Dyt_Api.DTOs.Auth;
using Dyt_Api.Entities;
using Dyt_Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Dyt_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private const string InvalidLoginMessage = "Email or password is invalid.";

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            IJwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto request)
        {
            var email = request.Email.Trim();

            if (await _userManager.FindByEmailAsync(email) is not null)
            {
                return BadRequest(new { message = "A user with this email already exists." });
            }

            var now = DateTime.UtcNow;
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                CreatedAt = now,
                UpdatedAt = now,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    errors = result.Errors.Select(error => new
                    {
                        error.Code,
                        error.Description
                    })
                });
            }

            return StatusCode(StatusCodes.Status201Created, new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email
            });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email.Trim());

            if (user is null ||
                !user.IsActive ||
                !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return Unauthorized(new { message = InvalidLoginMessage });
            }

            var tokenResult = await _jwtTokenService.GenerateTokenAsync(user);

            return Ok(new AuthResponseDto
            {
                Token = tokenResult.Token,
                Expiration = tokenResult.Expiration,
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty
            });
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            return Ok(new
            {
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                Email = User.FindFirstValue(ClaimTypes.Email),
                FirstName = User.FindFirstValue("firstName"),
                LastName = User.FindFirstValue("lastName"),
                Roles = User.FindAll(ClaimTypes.Role).Select(claim => claim.Value)
            });
        }
    }
}

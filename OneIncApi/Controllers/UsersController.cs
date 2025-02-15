using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OneIncApi.Domain.Exceptions;
using OneIncApi.Services.DTOs;
using OneIncApi.Services.Interfaces;

namespace OneIncApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            IUserService userService,
            ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<UserResponse>> CreateUser(CreateUserRequest request)
        {
            try
            {
                var user = await _userService.CreateUserAsync(request);
                return CreatedAtAction(nameof(CreateUser), new { id = user.Id }, user);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error occurred");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all users");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponse>> GetUser(int id)
        {
            try
            {
                return await _userService.GetUserAsync(id);
            }
            catch (DomainException ex)
            {
                _logger.LogWarning(ex, "User not found: {UserId}", id);
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserRequest request)
        {
            try
            {
                await _userService.UpdateUserAsync(id, request);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error updating user {UserId}", id);
                return BadRequest(ex.Errors);
            }
            catch (DomainException ex)
            {
                _logger.LogWarning(ex, "Error updating user {UserId}", id);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);
                return NoContent();
            }
            catch (DomainException ex)
            {
                _logger.LogWarning(ex, "Error deleting user {UserId}", id);
                return NotFound(ex.Message);
            }
        }
    }
}

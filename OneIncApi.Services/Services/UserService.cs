using FluentValidation;
using Microsoft.Extensions.Logging;
using OneIncApi.Domain.Exceptions;
using OneIncApi.Domain.Models;
using OneIncApi.Services.DTOs;
using OneIncApi.Services.Interfaces;

namespace OneIncApi.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IValidator<CreateUserRequest> _createValidator;
        private readonly IValidator<UpdateUserRequest> _updateValidator;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUserRepository userRepository,
            IValidator<CreateUserRequest> createValidator,
            IValidator<UpdateUserRequest> updateValidator,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _logger = logger;
        }

        public async Task<UserResponse> CreateUserAsync(CreateUserRequest request)
        {
            _logger.LogInformation("Creating user: {Email}", request.Email);

            var validationResult = await _createValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            if (await _userRepository.EmailExistsAsync(request.Email))
                throw new DomainException("Email already exists");

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                DateOfBirth = request.DateOfBirth,
                PhoneNumber = request.PhoneNumber
            };

            var createdUser = await _userRepository.CreateAsync(user);
            return UserResponse.FromUser(createdUser);
        }

        public async Task DeleteUserAsync(int id)
        {
            _logger.LogInformation("Deleting user with ID {UserId}", id);

            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null)
            {
                throw new DomainException("User not found");
            }

            await _userRepository.DeleteAsync(id);
            _logger.LogInformation("User {UserId} deleted", id);
        }

        public async Task<IEnumerable<UserResponse>> GetAllUsersAsync()
        {
            _logger.LogInformation("Fetching all users");

            var users = await _userRepository.GetAllAsync();
            return users.Select(UserResponse.FromUser);
        }

        public async Task<UserResponse> GetUserAsync(int id)
        {
            _logger.LogInformation("Fetching user with ID {UserId}", id);

            var user = await _userRepository.GetByIdAsync(id);
            return user != null
                ? UserResponse.FromUser(user)
                : throw new DomainException("User not found");
        }

        public async Task UpdateUserAsync(int id, UpdateUserRequest request)
        {
            _logger.LogInformation("Updating user with ID {UserId}", id);

            var validationResult = await _updateValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var existingUser = await _userRepository.GetByIdAsync(id)
                ?? throw new DomainException("User not found");

            if (existingUser.Email != request.Email &&
                await _userRepository.EmailExistsAsync(request.Email))
            {
                throw new DomainException("Email already exists");
            }

            var updatedUser = new User
            {
                Id = id,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                DateOfBirth = request.DateOfBirth,
                PhoneNumber = request.PhoneNumber
            };

            await _userRepository.UpdateAsync(updatedUser);
            _logger.LogInformation("User {UserId} updated", id);
        }
    }
}

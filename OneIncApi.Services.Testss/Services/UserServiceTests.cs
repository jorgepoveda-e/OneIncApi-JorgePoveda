using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using OneIncApi.Domain.Exceptions;
using OneIncApi.Domain.Models;
using OneIncApi.Services.DTOs;
using OneIncApi.Services.Interfaces;
using OneIncApi.Services.Services;

namespace OneIncApi.Services.Tests
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IUserRepository> _mockRepo;
        private Mock<IValidator<CreateUserRequest>> _mockValidator;
        private Mock<IValidator<UpdateUserRequest>> _mockUpdateValidator;
        private UserService _service;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IUserRepository>();
            _mockValidator = new Mock<IValidator<CreateUserRequest>>();
            _mockUpdateValidator = new Mock<IValidator<UpdateUserRequest>>();
            _service = new UserService(
                _mockRepo.Object,
                _mockValidator.Object,
                _mockUpdateValidator.Object,
                Mock.Of<ILogger<UserService>>()
            );
        }

        [Test]
        public async Task CreateUserAsync_ValidRequest_ReturnsUserResponse()
        {
            var request = new CreateUserRequest
            {
                FirstName = "Jorge",
                LastName = "Poveda",
                Email = "jpoveda@example.com",
                DateOfBirth = new DateTime(2000, 1, 1),
                PhoneNumber = "1234567890"
            };

            _mockValidator.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockRepo.Setup(r => r.EmailExistsAsync(request.Email))
                .ReturnsAsync(false);

            _mockRepo.Setup(r => r.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync(new User { Id = 1, FirstName = request.FirstName });

            var result = await _service.CreateUserAsync(request);

            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.FirstName, Is.EqualTo("Jorge"));

            _mockRepo.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Once);
        }

        [Test]
        public async Task CreateUserAsync_DuplicateEmail_ThrowsDomainException()
        {
            var request = new CreateUserRequest { Email = "existing@example.com" };

            _mockValidator.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockRepo.Setup(r => r.EmailExistsAsync(request.Email))
                .ReturnsAsync(true);

            var ex = Assert.ThrowsAsync<DomainException>(async () =>
                await _service.CreateUserAsync(request)
            );

            Assert.That(ex.Message, Is.EqualTo("Email already exists"));
        }

        [Test]
        public async Task CreateUserAsync_InvalidRequest_ThrowsValidationException()
        {
            var request = new CreateUserRequest();
            var failures = new List<ValidationFailure> { new("FirstName", "Required") };

            _mockValidator.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(failures));

            Assert.ThrowsAsync<ValidationException>(async () =>
                await _service.CreateUserAsync(request)
            );
        }
    }
}

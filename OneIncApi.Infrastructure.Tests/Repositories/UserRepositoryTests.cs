
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Moq;
using OneIncApi.Domain.Exceptions;
using OneIncApi.Domain.Models;
using OneIncApi.Infrastructure.Data;
using OneIncApi.Infrastructure.Repositories;

namespace OneIncApi.Infrastructure.Tests.Repositories
{
    [TestFixture]
    public class UserRepositoryTests
    {
        private const string TestConnectionString = "Server=localhost;Database=oneinc;Trusted_Connection=True;TrustServerCertificate=True";
        private Mock<ISqlConnectionFactory> _connectionFactoryMock;
        private UserRepository _repository;

        [OneTimeSetUp]
        public async Task InitializeAsync()
        {
            await using var setupConnection = new SqlConnection("Server=localhost;Trusted_Connection=True;TrustServerCertificate=True");
            await setupConnection.OpenAsync();

            // Create database if not exists
            var createDbCmd = setupConnection.CreateCommand();
            createDbCmd.CommandText = @"
                IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'oneinc')
                BEGIN
                    CREATE DATABASE oneinc
                END";
            await createDbCmd.ExecuteNonQueryAsync();

            await setupConnection.CloseAsync();

            // Connect to the newly created database
            await using var dbConnection = new SqlConnection(TestConnectionString);
            await dbConnection.OpenAsync();

            // Create table if not exists
            var createTableCmd = dbConnection.CreateCommand();
            createTableCmd.CommandText = @"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
                BEGIN
                    CREATE TABLE Users (
                        Id INT PRIMARY KEY IDENTITY(1,1),
                        FirstName NVARCHAR(128) NOT NULL,
                        LastName NVARCHAR(128),
                        Email NVARCHAR(255) NOT NULL UNIQUE,
                        DateOfBirth DATE NOT NULL,
                        PhoneNumber CHAR(10) NOT NULL
                    )
                END";
            await createTableCmd.ExecuteNonQueryAsync();
        }

        [SetUp]
        public void Setup()
        {
            _connectionFactoryMock = new Mock<ISqlConnectionFactory>();
            _connectionFactoryMock
                .Setup(f => f.CreateConnection())
                .Returns(() => new SqlConnection(TestConnectionString));
            _repository = new UserRepository(
                _connectionFactoryMock.Object,
                Mock.Of<ILogger<UserRepository>>()
            );
        }

        [TearDown]
        public async Task Cleanup()
        {
            await using var cleanupConnection = new SqlConnection(TestConnectionString);
            await cleanupConnection.OpenAsync();
            var cmd = cleanupConnection.CreateCommand();
            cmd.CommandText = "DELETE FROM Users";
            await cmd.ExecuteNonQueryAsync();
        }

        [Test]
        public async Task CreateAsync_DuplicateEmail_ThrowsDomainException()
        {
            var user1 = new User
            {
                FirstName = "User1",
                LastName = "Test1",
                Email = "duplicate@example.com",
                DateOfBirth = new DateTime(2000, 1, 1),
                PhoneNumber = "1234567890"
            };

            var user2 = new User
            {
                FirstName = "User2",
                LastName = "Test2",
                Email = "duplicate@example.com",
                DateOfBirth = new DateTime(1995, 5, 5),
                PhoneNumber = "0987654321"
            };

            await _repository.CreateAsync(user1);

            var ex = Assert.ThrowsAsync<DomainException>(async () =>
                await _repository.CreateAsync(user2)
            );

            Assert.That(ex.Message, Is.EqualTo("Email already exists"));

            await using var verifyConnection = new SqlConnection(TestConnectionString);
            await verifyConnection.OpenAsync();
            var cmd = new SqlCommand(
                "SELECT COUNT(*) FROM Users WHERE Email = @Email",
                verifyConnection
            );
            cmd.Parameters.AddWithValue("@Email", "duplicate@example.com");

            var count = (int)await cmd.ExecuteScalarAsync();
            Assert.That(count, Is.EqualTo(1));
        }
    }
}

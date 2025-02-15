using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using OneIncApi.Domain.Exceptions;
using OneIncApi.Domain.Models;
using OneIncApi.Infrastructure.Data;
using OneIncApi.Services.Interfaces;

namespace OneIncApi.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(
            ISqlConnectionFactory connectionFactory,
            ILogger<UserRepository> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public async Task<User> CreateAsync(User user)
        {
            using var connection = _connectionFactory.CreateConnection();
            var command = new SqlCommand(
                @"INSERT INTO Users 
                (FirstName, LastName, Email, DateOfBirth, PhoneNumber)
                OUTPUT INSERTED.Id
                VALUES
                (@FirstName, @LastName, @Email, @DateOfBirth, @PhoneNumber);",
                connection
            );

            // Agregar parámetros
            command.Parameters.AddWithValue("@FirstName", user.FirstName);
            command.Parameters.AddWithValue("@LastName", user.LastName ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@DateOfBirth", user.DateOfBirth);
            command.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber);

            try
            {
                await connection.OpenAsync();
                var userId = (int)await command.ExecuteScalarAsync();

                return new User
                {
                    Id = userId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    DateOfBirth = user.DateOfBirth,
                    PhoneNumber = user.PhoneNumber
                };
            }
            catch (SqlException ex) when (ex.Number == 2627)
            {
                _logger.LogError(ex, "Email conflict: {Email}", user.Email);
                throw new DomainException("Email already exists");
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            using var connection = _connectionFactory.CreateConnection();
            var command = new SqlCommand(
                "SELECT COUNT(1) FROM Users WHERE Email = @Email",
                connection
            );
            command.Parameters.AddWithValue("@Email", email);

            await connection.OpenAsync();
            var result = (int)await command.ExecuteScalarAsync();
            await connection.CloseAsync();

            return result > 0;
        }
        public async Task DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var command = new SqlCommand(
                "DELETE FROM Users WHERE Id = @Id",
                connection
            );
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
            await connection.CloseAsync();
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var users = new List<User>();

            using var connection = _connectionFactory.CreateConnection();
            var command = new SqlCommand("SELECT * FROM Users", connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                users.Add(MapUserFromReader(reader));
            }

            return users;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var command = new SqlCommand(
                "SELECT * FROM Users WHERE Id = @Id",
                connection
            );
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapUserFromReader(reader);
            }

            return null;
        }

        public async Task UpdateAsync(User user)
        {
            using var connection = _connectionFactory.CreateConnection();
            var command = new SqlCommand(
                 @"UPDATE Users 
                SET FirstName = @FirstName,
                    LastName = @LastName,
                    Email = @Email,
                    DateOfBirth = @DateOfBirth,
                    PhoneNumber = @PhoneNumber
                WHERE Id = @Id",
                connection
            );

            command.Parameters.AddWithValue("@Id", user.Id);
            command.Parameters.AddWithValue("@FirstName", user.FirstName);
            command.Parameters.AddWithValue("@LastName", user.LastName ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@DateOfBirth", user.DateOfBirth);
            command.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
            await connection.CloseAsync();
        }

        private User MapUserFromReader(SqlDataReader reader)
        {
            return new User
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ?
                    null : reader.GetString(reader.GetOrdinal("LastName")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                DateOfBirth = reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber"))
            };
        }
    }
}

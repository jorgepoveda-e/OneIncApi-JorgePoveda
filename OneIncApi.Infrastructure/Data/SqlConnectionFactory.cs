using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace OneIncApi.Infrastructure.Data
{
    public class SqlConnectionFactory : ISqlConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Default")!;
        }

        public SqlConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}

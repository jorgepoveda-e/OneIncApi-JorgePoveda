using Microsoft.Data.SqlClient;

namespace OneIncApi.Infrastructure.Data
{
    public interface ISqlConnectionFactory
    {
        SqlConnection CreateConnection();
    }
}

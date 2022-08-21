using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AspNetCore_WebApi_DevIO.Extensions
{
    public class SqlServerHealthCheck : IHealthCheck
    {
        readonly string Connection;

        public SqlServerHealthCheck(string connection)
        {
            Connection = connection;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                // In this scenario we're checking if we have products in the database.
                // If we don't have products we might have some kind of issue.
                using (var connection = new SqlConnection(Connection))
                {
                    await connection.OpenAsync(cancellationToken);

                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT COUNT(ID) FROM Product";

                    return Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken)) > 0 ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
                }
            }
            catch (Exception)
            {
                return HealthCheckResult.Unhealthy();
            }
        }
    }
}
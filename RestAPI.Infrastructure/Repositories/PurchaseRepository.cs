using Dapper;
using RestAPI.Core.Entities;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace RestAPI.Infrastructure.Repositories;

public class PurchaseRepository : GenericEntityRepository<Purchase>
{
    public PurchaseRepository(ILogger<PurchaseRepository> logger, IConfiguration configuration)
        : base(logger, configuration)
    {
    }

    public async Task<int> GetTotalPurchaseCountByCustomer(int customerId)
    {
        try
        {
            var sqlString = $"SELECT COUNT(*) FROM {_entityName} WHERE CustomerId = @CustomerId";
            using (MySqlConnection connection = CreateConnection())
            {
                return await connection.QuerySingleAsync<int>(sqlString, new { CustomerId = customerId });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"{GetType().Name}.GetTotalPurchaseCountByCustomer - Error: {ex.Message}");
            throw ex;
        }
    }
}
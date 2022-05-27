using RestAPI.Core.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace RestAPI.Infrastructure.Repositories;

public class CustomerRepository : GenericEntityRepository<Customer>
{
    public CustomerRepository(ILogger<CustomerRepository> logger, IConfiguration configuration)
        : base(logger, configuration)
    {
    }
}
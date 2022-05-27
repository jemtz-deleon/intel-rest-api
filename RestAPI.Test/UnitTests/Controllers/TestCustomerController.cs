
using Moq;
using Xunit;
using RestAPI.Web.Models;
using RestAPI.Web.Services;
using RestAPI.Web.Controllers;
using RestAPI.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RestAPI.Test.UnitTests.Controllers;

public class TestCustomerController
{
    // TODO: Use Class Fixture for the Arrange stuff.

    [Fact]
    public async Task GetAllCustomers_ShouldReturnOkStatus()
    {
        // Arrange
        var logger = new Mock<ILogger<CustomerController>>();
        var customerRepository = new Mock<CustomerRepository>();
        var customerService = new Mock<ICustomerService>();
        var purchaseService = new Mock<IPurchaseService>();
        customerService
            .Setup(_ => _.GetAll())
            .ReturnsAsync(GetCustomersTestData());

        var sut = new CustomerController(logger.Object,
            customerService.Object,
            customerRepository.Object,
            purchaseService.Object);

        // Act
        var result = await sut.GetCustomers();

        // Assert
        Assert.IsType<ActionResult<IEnumerable<CustomerDTO>>>(result);
    }

    private IEnumerable<CustomerDTO> GetCustomersTestData()
    {
        return new List<CustomerDTO>() {
             new CustomerDTO { Id = 1, Name="Test1" },
             new CustomerDTO { Id = 2, Name="Test2" },
             new CustomerDTO { Id = 3, Name="Test3" },
             new CustomerDTO { Id = 4, Name="Test4" },
        };
    }

    private CustomerDTO GetSingleCustomerTestData()
    {
        return  new CustomerDTO { Id = 1, Name="Test1" };
    }


}
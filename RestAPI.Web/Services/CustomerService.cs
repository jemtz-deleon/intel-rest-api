using RestAPI.Web.Models;
using RestAPI.Core.Entities;
using RestAPI.Infrastructure.Repositories;

namespace RestAPI.Web.Services;

public interface ICustomerService
{
    Task<int> AddCustomer(Customer newCustomer);
    Task<int> DeleteCustomer(int id);
    Task<IEnumerable<CustomerDTO>> GetAll();
    Task<CustomerDTO> GetById(int id);
    Task<int> UpdateCustomer(Customer updatedCustomer);
}

public class CustomerService : ICustomerService
{
    private readonly ILogger<ICustomerService> _logger;
    private readonly CustomerRepository _customerRepository;

    public CustomerService(ILogger<ICustomerService> logger, CustomerRepository customerRepository)
    {
        _logger = logger;
        _customerRepository = customerRepository;
    }

    public async Task<IEnumerable<CustomerDTO>> GetAll()
    {
        var customers = await _customerRepository.GetAllAsync();
        return (customers.Select(customer => ToModelDTO(customer))).ToList();
    }

    public async Task<CustomerDTO> GetById(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer is null) return null;
        return ToModelDTO(customer);
    }

    public async Task<int> AddCustomer(Customer newCustomer)
    {
        return await _customerRepository.AddAsync(newCustomer);
    }

    public async Task<int> UpdateCustomer(Customer updatedCustomer)
    {
        return await _customerRepository.UpdateAsync(updatedCustomer);
    }

    public async Task<int> DeleteCustomer(int id)
    {
        // TODO: Make it a soft delete?
        return await _customerRepository.DeleteAsync(id);
    }

    private static CustomerDTO ToModelDTO(Customer customer)
    {
        return new CustomerDTO
        {
            Id = customer.Id,
            Name = customer.Name
        };
    }
}
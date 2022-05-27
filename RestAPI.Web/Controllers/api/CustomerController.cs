using RestAPI.Web.Models;
using RestAPI.Web.Services;
using RestAPI.Core.Entities;
using RestAPI.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace RestAPI.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomerController : ControllerBase
{
    private readonly ILogger<CustomerController> _logger;
    private readonly CustomerRepository _customerRepository;
    private readonly ICustomerService _customerService;
    private readonly IPurchaseService _purchaseService;

    public CustomerController(ILogger<CustomerController> logger, ICustomerService customerService, CustomerRepository customerRepository, IPurchaseService purchaseService)
    {
        _logger = logger;
        _customerService = customerService;
        _customerRepository = customerRepository;
        _purchaseService = purchaseService;
    }

    // GET: api/Customer
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDTO>>> GetCustomers()
    {
        try
        {
            // TODO: Add filtering?
            var customers = await _customerService.GetAll();
            return Ok(customers);
        }
        catch (Exception ex)
        {
            _logger.LogError($"CustomerController.GetCustomers - Error: {ex.Message}");
            return BadRequest();
        }
    }

    // GET: api/Customer/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerDTO>> GetCustomer(int id)
    {
        try
        {
            var customer = await _customerService.GetById(id);
            return Ok(customer);
        }
        catch (Exception ex)
        {
            _logger.LogError($"CustomerController.GetCustomer - Error: {ex.Message}");
            return BadRequest();
        }
    }

    // PUT: api/Customer/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCustomer(int id, CustomerDTO customerToUpdate)
    {
        try
        {
            // Id doesn't match. Don't attempt update.
            if (customerToUpdate.Id != id) return BadRequest();

            // Validate name as well.
            if (string.IsNullOrWhiteSpace(customerToUpdate.Name)) return BadRequest();

            // Find actual record to be updated.
            // TODO: Not really happy calling directly the repo here. Move to service?
            Customer customer = await _customerRepository.GetByIdAsync(id);

            // Not found
            if (customer is null) return BadRequest();

            // Update fields. Only the name is updatable, for now.
            customer.Name = customerToUpdate.Name;

            // Do the update
            var updateResponse = await _customerService.UpdateCustomer(customer);
            if (updateResponse != 1) return BadRequest();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError($"CustomerController.UpdateCustomer - Error: {ex.Message}");
            return BadRequest();
        }
    }

    // POST: api/Customer
    [HttpPost]
    public async Task<ActionResult<CustomerDTO>> CreateCustomer(CustomerDTO newCustomer)
    {
        try
        {
            // Name is required.
            if (string.IsNullOrWhiteSpace(newCustomer.Name)) return BadRequest();

            Customer customer = new Customer
            {
                Name = newCustomer.Name
            };

            var createResponse = await _customerService.AddCustomer(customer);
            if (createResponse != 1) return BadRequest();

            // TODO: Bring id back?
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError($"CustomerController.CreateCustomer - Error: {ex.Message}");
            return BadRequest();
        }
    }

    // DELETE: api/Customer/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        try
        {
            CustomerDTO customerToDelete = await _customerService.GetById(id);

            // Could also return a UnprocessableEntity()
            // It depends on how much info we want to give back
            // e.g. Do we want them to be able to know that the record
            // attempted to delete actually doesn't exist?
            if (customerToDelete is null) return NotFound();

            // TODO: Need to handle scenario with Purchases tied to Clients.
            // Probably a soft delete. Don't allow a delete for now.
            var totalPurchasesCount = await _purchaseService.GetTotalPurchasesByClient(id);
            if (totalPurchasesCount > 0) return UnprocessableEntity();

            //Make the actual delete.
            await _customerRepository.DeleteAsync(id);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError($"CustomerController.DeleteCustomer - Error: {ex.Message}");
            return BadRequest();
        }
    }
}
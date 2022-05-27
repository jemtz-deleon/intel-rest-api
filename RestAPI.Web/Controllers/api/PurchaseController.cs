using RestAPI.Web.Models;
using RestAPI.Web.Services;
using RestAPI.Core.Entities;
using RestAPI.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace RestAPI.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PurchaseController : ControllerBase
{
    private readonly ILogger<PurchaseController> _logger;
    private readonly IPurchaseService _purchaseService;
    private readonly ICustomerService _customerService;

    public PurchaseController(ILogger<PurchaseController> logger, IPurchaseService purchaseService, ICustomerService customerService)
    {
        _logger = logger;
        _purchaseService = purchaseService;
        _customerService = customerService;
    }

    // GET: api/Purchase
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PurchaseDTO>>> GetPurchases()
    {
        try
        {
            // TODO: Add filtering?
            var purchases = await _purchaseService.GetAll();
            return Ok(purchases);
        }
        catch (Exception ex)
        {
            _logger.LogError($"PurchaseController.GetPurchases - Error: {ex.Message}");
            return BadRequest();
        }
    }

    // GET: api/Purchase/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<PurchaseDTO>> GetPurchase(int id)
    {
        try
        {
            var purchase = await _purchaseService.GetById(id);
            return Ok(purchase);
        }
        catch (Exception ex)
        {
            _logger.LogError($"PurchaseController.GetPurchase - Error: {ex.Message}");
            return BadRequest();
        }
    }

    // PUT: api/Purchase/{id}
    [HttpPut("{id}")]
    public IActionResult UpdatePurchase(int id, CustomerDTO customerToUpdate)
    {
        // TODO: Implement. Not required, for now.
        return NotFound();
    }

    // POST: api/Purchase
    [HttpPost]
    public async Task<ActionResult<PurchaseDTO>> CreatePurchase(PurchaseDTO newPurchase)
    {
        try
        {
            // Get Customer that we need to tie the Purchase to
            var customer = await _customerService.GetById(newPurchase.CustomerId);
            if (customer is null) return BadRequest();

            // Calculate a discount, based on previous purchases count
            var totalPurchasesCount = await _purchaseService.GetTotalPurchasesByClient(newPurchase.CustomerId);
            var discountToApply = calculateDiscount(totalPurchasesCount);

            var originalCostToApply = 100; // Hard-coded.

            Purchase purchase = new Purchase
            {
                CustomerId = newPurchase.CustomerId,
                OriginalCost = originalCostToApply,
                DiscountApplied = discountToApply,
                FinalCost = calculateFinalCost(originalCostToApply, discountToApply)
            };

            var createResponse = await _purchaseService.AddPurchase(purchase);
            if (createResponse != 1) return BadRequest();

            // TODO: Bring id back?
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError($"PurchaseController.CreatePurchase - Error: {ex.Message}");
            return BadRequest();
        }
    }

    // DELETE: api/Purchase/{id}
    [HttpDelete("{id}")]
    public IActionResult DeletePurchase(int id)
    {
        // TODO: Implement. Not required, for now.
        return NotFound();
    }

    private static decimal calculateFinalCost(int originalCost, decimal discountToApply)
    {
        return originalCost - (originalCost * discountToApply);
    }

    private static decimal calculateDiscount(int totalPurchasesCount)
    {
        decimal discountToApply;

        switch (totalPurchasesCount)
        {
            case < 0:
                discountToApply = 0;
                break;
            case >= 1 and <= 2:
                discountToApply = (decimal)0.01;
                break;
            case >= 3 and <= 5:
                discountToApply = (decimal)0.02;
                break;
            case >= 5 and <= 10:
                discountToApply = (decimal)0.05;
                break;
            case > 10:
                discountToApply = (decimal)0.1;
                break;
            default:
                // TODO: Error out or... just not apply a discount?
                discountToApply = 0;
                break;
        }
        return discountToApply;
    }
}
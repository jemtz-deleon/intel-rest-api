using RestAPI.Web.Models;
using RestAPI.Core.Entities;
using RestAPI.Infrastructure.Repositories;

namespace RestAPI.Web.Services;

public interface IPurchaseService
{
    Task<int> AddPurchase(Purchase newPurchase);
    Task<int> DeletePurchase(int id);
    Task<IEnumerable<PurchaseDTO>> GetAll();
    Task<PurchaseDTO> GetById(int id);
    Task<int> GetTotalPurchasesByClient(int id);
    Task<int> UpdatePurchase(Purchase updatedPurchase);
}

public class PurchaseService : IPurchaseService
{
    private readonly ILogger<IPurchaseService> _logger;
    private readonly PurchaseRepository _purchaseRepository;

    public PurchaseService(ILogger<IPurchaseService> logger, PurchaseRepository purchaseRepository)
    {
        _logger = logger;
        _purchaseRepository = purchaseRepository;
    }

    public async Task<IEnumerable<PurchaseDTO>> GetAll()
    {
        var purchases = await _purchaseRepository.GetAllAsync();
        return (purchases.Select(purchase => ToModelDTO(purchase))).ToList();
    }

    public async Task<PurchaseDTO> GetById(int id)
    {
        var purchase = await _purchaseRepository.GetByIdAsync(id);
        if (purchase is null) return null;
        return ToModelDTO(purchase);
    }

    public async Task<int> GetTotalPurchasesByClient(int id)
    {
        return await _purchaseRepository.GetTotalPurchaseCountByCustomer(id);
    }

    public async Task<int> AddPurchase(Purchase newPurchase)
    {
        return await _purchaseRepository.AddAsync(newPurchase);
    }

    public async Task<int> UpdatePurchase(Purchase updatedPurchase)
    {
        return await _purchaseRepository.UpdateAsync(updatedPurchase);
    }

    public async Task<int> DeletePurchase(int id)
    {
        // TODO: Make it a soft delete?
        return await _purchaseRepository.DeleteAsync(id);
    }

    private static PurchaseDTO ToModelDTO(Purchase purchase)
    {
        return new PurchaseDTO
        {
            Id = purchase.Id,
            CustomerId = purchase.CustomerId,
            FinalCost = purchase.FinalCost,
            PurchaseDate = purchase.DateAdded
        };
    }
}
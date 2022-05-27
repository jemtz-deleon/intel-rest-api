namespace RestAPI.Web.Models;

public class PurchaseDTO
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public decimal FinalCost { get; set; }
    public DateTime PurchaseDate { get; set; }
}
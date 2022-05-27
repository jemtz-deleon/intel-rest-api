namespace RestAPI.Core.Entities;

public class Purchase : BaseEntity
{
    public int CustomerId { get; set; }
    public decimal OriginalCost { get; set; }       // Hard-coded to 100
    public decimal FinalCost { get; set; }          // Original Cost - (Original Cost * DiscountApplied)
    public decimal DiscountApplied { get; set; }    // Let's keep track of it, for historical purposes
}
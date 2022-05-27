namespace RestAPI.Core.Interfaces;

public interface IEntity
{
    public int Id { get; set; }
    public DateTime DateAdded { get; set; }
    public DateTime? DateUpdated { get; set; }
}
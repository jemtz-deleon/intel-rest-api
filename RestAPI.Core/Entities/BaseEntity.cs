using RestAPI.Core.Interfaces;

namespace RestAPI.Core.Entities;

public class BaseEntity : IEntity
{
    public int Id { get; set; }
    public DateTime DateAdded { get; set; } = DateTime.Now; // UtcNow? Depends if regional or global.
    public DateTime? DateUpdated { get; set; }
}
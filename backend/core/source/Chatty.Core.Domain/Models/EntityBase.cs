using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Sensei.Core.Domain.Models;

public abstract class EntityBase
{
    public Guid Id { get; set; }
    
    public bool IsDeleted { get; set; }

    protected EntityBase(Guid id)
    {
        Id = id;
    }
}

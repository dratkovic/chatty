namespace Chatty.Domain.Common;

public class AuditEntityBase: EntityBase
{
    public string? CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }

    protected AuditEntityBase(Guid? id): base(id)
    {
    }

}
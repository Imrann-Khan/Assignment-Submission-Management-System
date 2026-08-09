using Domain.Enums;

namespace Domain.Entities;

public class Notification : BaseAuditableEntity
{
    public Guid UserId {get; set;}
    public User User {get; set;} = null!;
    public NotificationType Type {get; set;} 
    public string Message {get; set;} = string.Empty;
    public bool IsRead {get; set;} 
    public Guid? RelatedAssignmentId {get; set;}
}
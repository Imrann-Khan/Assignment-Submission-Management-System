using Domain.Common;

namespace Domain.Entities;

public class TeacherSubjectAssignment : BaseAuditableEntity
{
    public Guid TeacherId { get; set; }
    public User Teacher { get; set; } = null!;

    public Guid SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;
}

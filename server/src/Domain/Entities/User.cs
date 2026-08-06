using Domain.Enums;

namespace Domain.Entities;

public class User : BaseAuditableEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid? ClassId { get; set; }
    public Class? Class { get; set; }

    public ICollection<TeacherSubjectAssignment> TeacherSubjectAssignments { get; set; } = new List<TeacherSubjectAssignment>();
    public ICollection<Assignment> CreatedAssignments { get; set; } = new List<Assignment>();
    public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}

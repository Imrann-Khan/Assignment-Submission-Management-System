using Domain.Common;

namespace Domain.Entities;

public class Subject : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;

    public Guid ClassId { get; set; }
    public Class Class { get; set; } = null!;

    public ICollection<TeacherSubjectAssignment> TeacherSubjectAssignments { get; set; } = new List<TeacherSubjectAssignment>();
    public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
}

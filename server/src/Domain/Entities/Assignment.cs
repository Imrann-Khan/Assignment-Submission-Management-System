using Domain.Enums;

namespace Domain.Entities;

public class Assignment : BaseAuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Deadline { get; set; }
    public int MaxMarks { get; set; }
    public AssignmentStatus Status { get; set; } = AssignmentStatus.Draft;

    public Guid ClassId { get; set; }
    public Class Class { get; set; } = null!;

    public Guid SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;

    public Guid TeacherId { get; set; }
    public User Teacher { get; set; } = null!;

    public ICollection<Submission> Submissions { get; set; } = new List<Submission>();

    public bool IsPastDeadline(DateTime utcNow) => utcNow > Deadline;
}

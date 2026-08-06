using Domain.Enums;

namespace Domain.Entities;

public class Submission : BaseAuditableEntity
{
    public string AnswerText { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    public SubmissionStatus Status { get; set; } = SubmissionStatus.Submitted;

    public int? Marks { get; set; }
    public string? Feedback { get; set; }
    public DateTime? GradedAt { get; set; }
    public Guid? GradedById { get; set; }
    public User? GradedBy { get; set; }

    public Guid AssignmentId { get; set; }
    public Assignment Assignment { get; set; } = null!;

    public Guid StudentId { get; set; }
    public User Student { get; set; } = null!;
}

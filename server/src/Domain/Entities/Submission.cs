using Domain.Common;
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
    public int? GradedById { get; set; }
    public User? GradedBy { get; set; }

    public int AssignmentId { get; set; }
    public Assignment Assignment { get; set; } = null!;

    public int StudentId { get; set; }
    public User Student { get; set; } = null!;
}

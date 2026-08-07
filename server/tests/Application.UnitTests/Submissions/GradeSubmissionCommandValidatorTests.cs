using Application.Submissions.Grade;
using Application.UnitTests.Common;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;

namespace Application.UnitTests.Submissions;

public class GradeSubmissionCommandValidatorTests
{
    [Fact]
    public async Task Validate_WhenMarksExceedMaxMarks_HasValidationError()
    {
        using var context = TestDbContextFactory.Create();

        var @class = new Class { Name = "Class A" };
        var subject = new Subject { Name = "Math", Class = @class };
        var teacher = new User { FullName = "Teacher", Email = "t@test.com", PasswordHash = "x", Role = UserRole.Teacher };
        var student = new User { FullName = "Student", Email = "s@test.com", PasswordHash = "x", Role = UserRole.Student, Class = @class };

        context.AddRange(@class, subject, teacher, student);
        await context.SaveChangesAsync(CancellationToken.None);

        var assignment = new Assignment
        {
            Title = "Assignment",
            Description = "d",
            ClassId = @class.Id,
            SubjectId = subject.Id,
            TeacherId = teacher.Id,
            Deadline = DateTime.UtcNow.AddDays(5),
            MaxMarks = 50,
            Status = AssignmentStatus.Published
        };
        context.Assignments.Add(assignment);
        await context.SaveChangesAsync(CancellationToken.None);

        var submission = new Submission
        {
            AssignmentId = assignment.Id,
            StudentId = student.Id,
            AnswerText = "Answer",
            SubmittedAt = DateTime.UtcNow,
            Status = SubmissionStatus.Submitted
        };
        context.Submissions.Add(submission);
        await context.SaveChangesAsync(CancellationToken.None);

        var validator = new GradeSubmissionCommandValidator(context);
        var command = new GradeSubmissionCommand(submission.Id, 999, "Too high");

        var result = await validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Marks");
    }

    [Fact]
    public async Task Validate_WhenMarksWithinRange_IsValid()
    {
        using var context = TestDbContextFactory.Create();

        var @class = new Class { Name = "Class A" };
        var subject = new Subject { Name = "Math", Class = @class };
        var teacher = new User { FullName = "Teacher", Email = "t@test.com", PasswordHash = "x", Role = UserRole.Teacher };
        var student = new User { FullName = "Student", Email = "s@test.com", PasswordHash = "x", Role = UserRole.Student, Class = @class };

        context.AddRange(@class, subject, teacher, student);
        await context.SaveChangesAsync(CancellationToken.None);

        var assignment = new Assignment
        {
            Title = "Assignment",
            Description = "d",
            ClassId = @class.Id,
            SubjectId = subject.Id,
            TeacherId = teacher.Id,
            Deadline = DateTime.UtcNow.AddDays(5),
            MaxMarks = 50,
            Status = AssignmentStatus.Published
        };
        context.Assignments.Add(assignment);
        await context.SaveChangesAsync(CancellationToken.None);

        var submission = new Submission
        {
            AssignmentId = assignment.Id,
            StudentId = student.Id,
            AnswerText = "Answer",
            SubmittedAt = DateTime.UtcNow,
            Status = SubmissionStatus.Submitted
        };
        context.Submissions.Add(submission);
        await context.SaveChangesAsync(CancellationToken.None);

        var validator = new GradeSubmissionCommandValidator(context);
        var command = new GradeSubmissionCommand(submission.Id, 45, "Great");

        var result = await validator.ValidateAsync(command);

        result.IsValid.Should().BeTrue();
    }
}

using Application.Common.Exceptions;
using Application.Submissions.Grade;
using Application.UnitTests.Common;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;

namespace Application.UnitTests.Submissions;

public class GradeSubmissionCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenNotAssignmentOwner_ThrowsForbiddenAccessException()
    {
        using var context = TestDbContextFactory.Create();

        var @class = new Class { Name = "Class A" };
        var subject = new Subject { Name = "Math", Class = @class };
        var owner = new User { FullName = "Owner", Email = "owner@test.com", PasswordHash = "x", Role = UserRole.Teacher };
        var otherTeacher = new User { FullName = "Other", Email = "other@test.com", PasswordHash = "x", Role = UserRole.Teacher };
        var student = new User { FullName = "Student", Email = "s@test.com", PasswordHash = "x", Role = UserRole.Student, Class = @class };

        context.AddRange(@class, subject, owner, otherTeacher, student);
        await context.SaveChangesAsync(CancellationToken.None);

        var assignment = new Assignment
        {
            Title = "Assignment",
            Description = "d",
            ClassId = @class.Id,
            SubjectId = subject.Id,
            TeacherId = owner.Id,
            Deadline = DateTime.UtcNow.AddDays(5),
            MaxMarks = 100,
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

        var currentUser = new TestCurrentUserService { UserId = otherTeacher.Id, Role = UserRole.Teacher };
        var handler = new GradeSubmissionCommandHandler(context, currentUser);

        var act = () => handler.Handle(new GradeSubmissionCommand(submission.Id, 80, "Nice"), CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }

    [Fact]
    public async Task Handle_WhenOwner_GradesSuccessfully()
    {
        using var context = TestDbContextFactory.Create();

        var @class = new Class { Name = "Class A" };
        var subject = new Subject { Name = "Math", Class = @class };
        var owner = new User { FullName = "Owner", Email = "owner@test.com", PasswordHash = "x", Role = UserRole.Teacher };
        var student = new User { FullName = "Student", Email = "s@test.com", PasswordHash = "x", Role = UserRole.Student, Class = @class };

        context.AddRange(@class, subject, owner, student);
        await context.SaveChangesAsync(CancellationToken.None);

        var assignment = new Assignment
        {
            Title = "Assignment",
            Description = "d",
            ClassId = @class.Id,
            SubjectId = subject.Id,
            TeacherId = owner.Id,
            Deadline = DateTime.UtcNow.AddDays(5),
            MaxMarks = 100,
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

        var currentUser = new TestCurrentUserService { UserId = owner.Id, Role = UserRole.Teacher };
        var handler = new GradeSubmissionCommandHandler(context, currentUser);

        var result = await handler.Handle(new GradeSubmissionCommand(submission.Id, 85, "Well done"), CancellationToken.None);

        result.Marks.Should().Be(85);
        result.Feedback.Should().Be("Well done");
        result.Status.Should().Be("Graded");
    }
}

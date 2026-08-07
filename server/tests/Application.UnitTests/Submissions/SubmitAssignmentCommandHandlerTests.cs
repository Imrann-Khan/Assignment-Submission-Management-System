using Application.Common.Exceptions;
using Application.Submissions.Submit;
using Application.UnitTests.Common;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;

namespace Application.UnitTests.Submissions;

public class SubmitAssignmentCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenAssignmentIsDraft_ThrowsNotFoundException()
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
            Title = "Draft Assignment",
            Description = "d",
            ClassId = @class.Id,
            SubjectId = subject.Id,
            TeacherId = teacher.Id,
            Deadline = DateTime.UtcNow.AddDays(5),
            MaxMarks = 100,
            Status = AssignmentStatus.Draft
        };
        context.Assignments.Add(assignment);
        await context.SaveChangesAsync(CancellationToken.None);

        var currentUser = new TestCurrentUserService { UserId = student.Id, Role = UserRole.Student };
        var handler = new SubmitAssignmentCommandHandler(context, currentUser);

        var act = () => handler.Handle(new SubmitAssignmentCommand(assignment.Id, "My answer"), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenPastDeadline_ThrowsForbiddenAccessException()
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
            Title = "Past Deadline Assignment",
            Description = "d",
            ClassId = @class.Id,
            SubjectId = subject.Id,
            TeacherId = teacher.Id,
            Deadline = DateTime.UtcNow.AddDays(-1),
            MaxMarks = 100,
            Status = AssignmentStatus.Published
        };
        context.Assignments.Add(assignment);
        await context.SaveChangesAsync(CancellationToken.None);

        var currentUser = new TestCurrentUserService { UserId = student.Id, Role = UserRole.Student };
        var handler = new SubmitAssignmentCommandHandler(context, currentUser);

        var act = () => handler.Handle(new SubmitAssignmentCommand(assignment.Id, "My answer"), CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }

    [Fact]
    public async Task Handle_WhenStudentNotInAssignmentClass_ThrowsNotFoundException()
    {
        using var context = TestDbContextFactory.Create();

        var classA = new Class { Name = "Class A" };
        var classB = new Class { Name = "Class B" };
        var subject = new Subject { Name = "Math", Class = classA };
        var teacher = new User { FullName = "Teacher", Email = "t@test.com", PasswordHash = "x", Role = UserRole.Teacher };
        var studentInB = new User { FullName = "Student", Email = "s@test.com", PasswordHash = "x", Role = UserRole.Student, Class = classB };

        context.AddRange(classA, classB, subject, teacher, studentInB);
        await context.SaveChangesAsync(CancellationToken.None);

        var assignment = new Assignment
        {
            Title = "Class A Assignment",
            Description = "d",
            ClassId = classA.Id,
            SubjectId = subject.Id,
            TeacherId = teacher.Id,
            Deadline = DateTime.UtcNow.AddDays(5),
            MaxMarks = 100,
            Status = AssignmentStatus.Published
        };
        context.Assignments.Add(assignment);
        await context.SaveChangesAsync(CancellationToken.None);

        var currentUser = new TestCurrentUserService { UserId = studentInB.Id, Role = UserRole.Student };
        var handler = new SubmitAssignmentCommandHandler(context, currentUser);

        var act = () => handler.Handle(new SubmitAssignmentCommand(assignment.Id, "My answer"), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_FirstSubmission_CreatesNewSubmission()
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
            MaxMarks = 100,
            Status = AssignmentStatus.Published
        };
        context.Assignments.Add(assignment);
        await context.SaveChangesAsync(CancellationToken.None);

        var currentUser = new TestCurrentUserService { UserId = student.Id, Role = UserRole.Student };
        var handler = new SubmitAssignmentCommandHandler(context, currentUser);

        var result = await handler.Handle(new SubmitAssignmentCommand(assignment.Id, "My first answer"), CancellationToken.None);

        result.AnswerText.Should().Be("My first answer");
        result.Status.Should().Be("Submitted");
    }

    [Fact]
    public async Task Handle_Resubmission_UpdatesExistingSubmissionAndClearsGrade()
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
            MaxMarks = 100,
            Status = AssignmentStatus.Published
        };
        context.Assignments.Add(assignment);
        await context.SaveChangesAsync(CancellationToken.None);

        var existingSubmission = new Submission
        {
            AssignmentId = assignment.Id,
            StudentId = student.Id,
            AnswerText = "Old answer",
            SubmittedAt = DateTime.UtcNow.AddHours(-2),
            Status = SubmissionStatus.Graded,
            Marks = 90,
            Feedback = "Good job",
            GradedAt = DateTime.UtcNow.AddHours(-1),
            GradedById = teacher.Id
        };
        context.Submissions.Add(existingSubmission);
        await context.SaveChangesAsync(CancellationToken.None);

        var currentUser = new TestCurrentUserService { UserId = student.Id, Role = UserRole.Student };
        var handler = new SubmitAssignmentCommandHandler(context, currentUser);

        var result = await handler.Handle(new SubmitAssignmentCommand(assignment.Id, "Updated answer"), CancellationToken.None);

        result.Id.Should().Be(existingSubmission.Id);
        result.AnswerText.Should().Be("Updated answer");
        result.Status.Should().Be("Submitted");
        result.Marks.Should().BeNull();
        result.Feedback.Should().BeNull();
    }
}

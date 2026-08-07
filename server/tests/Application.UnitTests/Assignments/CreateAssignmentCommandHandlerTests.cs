using Application.Assignments.Create;
using Application.Common.Exceptions;
using Application.UnitTests.Common;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;

namespace Application.UnitTests.Assignments;

public class CreateAssignmentCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenTeacherNotAssignedToSubject_ThrowsForbiddenAccessException()
    {
        using var context = TestDbContextFactory.Create();

        var @class = new Class { Name = "Class A" };
        var subject = new Subject { Name = "Mathematics", Class = @class };
        var teacher = new User { FullName = "Teacher", Email = "t@test.com", PasswordHash = "x", Role = UserRole.Teacher };

        context.AddRange(@class, subject, teacher);
        await context.SaveChangesAsync(CancellationToken.None);
        // Note: no TeacherSubjectAssignment created — teacher is NOT assigned to this subject

        var currentUser = new TestCurrentUserService { UserId = teacher.Id, Role = UserRole.Teacher };
        var handler = new CreateAssignmentCommandHandler(context, currentUser);

        var command = new CreateAssignmentCommand(
            "Algebra Homework", "Solve the problems", @class.Id, subject.Id, DateTime.UtcNow.AddDays(5), 100);

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }

    [Fact]
    public async Task Handle_WhenTeacherAssignedToSubject_CreatesAssignmentAsDraft()
    {
        using var context = TestDbContextFactory.Create();

        var @class = new Class { Name = "Class A" };
        var subject = new Subject { Name = "Mathematics", Class = @class };
        var teacher = new User { FullName = "Teacher", Email = "t@test.com", PasswordHash = "x", Role = UserRole.Teacher };

        context.AddRange(@class, subject, teacher);
        await context.SaveChangesAsync(CancellationToken.None);

        context.TeacherSubjectAssignments.Add(new TeacherSubjectAssignment { TeacherId = teacher.Id, SubjectId = subject.Id });
        await context.SaveChangesAsync(CancellationToken.None);

        var currentUser = new TestCurrentUserService { UserId = teacher.Id, Role = UserRole.Teacher };
        var handler = new CreateAssignmentCommandHandler(context, currentUser);

        var command = new CreateAssignmentCommand(
            "Algebra Homework", "Solve the problems", @class.Id, subject.Id, DateTime.UtcNow.AddDays(5), 100);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Title.Should().Be("Algebra Homework");
        result.Status.Should().Be("Draft");
        result.TeacherId.Should().Be(teacher.Id);
    }
}

using Application.Assignments.Update;
using Application.Common.Exceptions;
using Application.UnitTests.Common;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;

namespace Application.UnitTests.Assignments;

public class UpdateAssignmentCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenNotOwner_ThrowsForbiddenAccessException()
    {
        using var context = TestDbContextFactory.Create();

        var @class = new Class { Name = "Class A" };
        var subject = new Subject { Name = "Mathematics", Class = @class };
        var owner = new User { FullName = "Owner Teacher", Email = "owner@test.com", PasswordHash = "x", Role = UserRole.Teacher };
        var otherTeacher = new User { FullName = "Other Teacher", Email = "other@test.com", PasswordHash = "x", Role = UserRole.Teacher };

        context.AddRange(@class, subject, owner, otherTeacher);
        await context.SaveChangesAsync(CancellationToken.None);

        var assignment = new Assignment
        {
            Title = "Original Title",
            Description = "Original Description",
            ClassId = @class.Id,
            SubjectId = subject.Id,
            TeacherId = owner.Id,
            Deadline = DateTime.UtcNow.AddDays(5),
            MaxMarks = 100,
            Status = AssignmentStatus.Draft
        };
        context.Assignments.Add(assignment);
        await context.SaveChangesAsync(CancellationToken.None);

        var currentUser = new TestCurrentUserService { UserId = otherTeacher.Id, Role = UserRole.Teacher };
        var handler = new UpdateAssignmentCommandHandler(context, currentUser);

        var command = new UpdateAssignmentCommand(assignment.Id, "New Title", "New Description", DateTime.UtcNow.AddDays(10), 50);

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }

    [Fact]
    public async Task Handle_WhenOwner_UpdatesSuccessfully()
    {
        using var context = TestDbContextFactory.Create();

        var @class = new Class { Name = "Class A" };
        var subject = new Subject { Name = "Mathematics", Class = @class };
        var owner = new User { FullName = "Owner Teacher", Email = "owner@test.com", PasswordHash = "x", Role = UserRole.Teacher };

        context.AddRange(@class, subject, owner);
        await context.SaveChangesAsync(CancellationToken.None);

        var assignment = new Assignment
        {
            Title = "Original Title",
            Description = "Original Description",
            ClassId = @class.Id,
            SubjectId = subject.Id,
            TeacherId = owner.Id,
            Deadline = DateTime.UtcNow.AddDays(5),
            MaxMarks = 100,
            Status = AssignmentStatus.Draft
        };
        context.Assignments.Add(assignment);
        await context.SaveChangesAsync(CancellationToken.None);

        var currentUser = new TestCurrentUserService { UserId = owner.Id, Role = UserRole.Teacher };
        var handler = new UpdateAssignmentCommandHandler(context, currentUser);

        var command = new UpdateAssignmentCommand(assignment.Id, "Updated Title", "Updated Description", DateTime.UtcNow.AddDays(10), 50);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Title.Should().Be("Updated Title");
        result.MaxMarks.Should().Be(50);
    }
}

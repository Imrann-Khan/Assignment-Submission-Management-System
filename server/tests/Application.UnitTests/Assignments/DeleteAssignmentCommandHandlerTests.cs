using Application.Assignments.Delete;
using Application.Common.Exceptions;
using Application.UnitTests.Common;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.Assignments;

public class DeleteAssignmentCommandHandlerTests
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
            Title = "Title",
            Description = "Description",
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
        var handler = new DeleteAssignmentCommandHandler(context, currentUser);

        var act = () => handler.Handle(new DeleteAssignmentCommand(assignment.Id), CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }

    [Fact]
    public async Task Handle_WhenOwner_DeletesAssignment()
    {
        using var context = TestDbContextFactory.Create();

        var @class = new Class { Name = "Class A" };
        var subject = new Subject { Name = "Mathematics", Class = @class };
        var owner = new User { FullName = "Owner Teacher", Email = "owner@test.com", PasswordHash = "x", Role = UserRole.Teacher };

        context.AddRange(@class, subject, owner);
        await context.SaveChangesAsync(CancellationToken.None);

        var assignment = new Assignment
        {
            Title = "Title",
            Description = "Description",
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
        var handler = new DeleteAssignmentCommandHandler(context, currentUser);

        await handler.Handle(new DeleteAssignmentCommand(assignment.Id), CancellationToken.None);

        var exists = await context.Assignments.AnyAsync(a => a.Id == assignment.Id, CancellationToken.None);
        exists.Should().BeFalse();
    }
}

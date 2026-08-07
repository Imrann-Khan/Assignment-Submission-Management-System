using Application.Assignments.List;
using Application.UnitTests.Common;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Infrastructure.Persistence;

namespace Application.UnitTests.Assignments;

public class GetAssignmentsQueryHandlerTests
{
    [Fact]
    public async Task Handle_AsAdmin_ReturnsAllAssignments()
    {
        using var context = TestDbContextFactory.Create();
        var scenario = await Seed(context);

        var currentUser = new TestCurrentUserService { UserId = scenario.Admin.Id, Role = UserRole.Admin };
        var handler = new GetAssignmentsQueryHandler(context, currentUser);

        var result = await handler.Handle(new GetAssignmentsQuery(null, null, null), CancellationToken.None);

        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task Handle_AsTeacher_ReturnsOnlyOwnAssignments()
    {
        using var context = TestDbContextFactory.Create();
        var scenario = await Seed(context);

        var currentUser = new TestCurrentUserService { UserId = scenario.TeacherA.Id, Role = UserRole.Teacher };
        var handler = new GetAssignmentsQueryHandler(context, currentUser);

        var result = await handler.Handle(new GetAssignmentsQuery(null, null, null), CancellationToken.None);

        result.Should().HaveCount(2);
        result.Should().OnlyContain(a => a.TeacherId == scenario.TeacherA.Id);
    }

    [Fact]
    public async Task Handle_AsStudent_ReturnsOnlyPublishedAssignmentsInOwnClass()
    {
        using var context = TestDbContextFactory.Create();
        var scenario = await Seed(context);

        var currentUser = new TestCurrentUserService { UserId = scenario.StudentInA.Id, Role = UserRole.Student };
        var handler = new GetAssignmentsQueryHandler(context, currentUser);

        var result = await handler.Handle(new GetAssignmentsQuery(null, null, null), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].Status.Should().Be("Published");
        result[0].ClassId.Should().Be(scenario.ClassA.Id);
    }

    private static async Task<TestScenario> Seed(ApplicationDbContext context)
    {
        var classA = new Class { Name = "Class A" };
        var classB = new Class { Name = "Class B" };
        var subjectA = new Subject { Name = "Math", Class = classA };
        var subjectB = new Subject { Name = "English", Class = classB };
        var admin = new User { FullName = "Admin", Email = "admin@test.com", PasswordHash = "x", Role = UserRole.Admin };
        var teacherA = new User { FullName = "Teacher A", Email = "teachera@test.com", PasswordHash = "x", Role = UserRole.Teacher };
        var teacherB = new User { FullName = "Teacher B", Email = "teacherb@test.com", PasswordHash = "x", Role = UserRole.Teacher };
        var studentInA = new User { FullName = "Student A", Email = "studenta@test.com", PasswordHash = "x", Role = UserRole.Student, Class = classA };

        context.AddRange(classA, classB, subjectA, subjectB, admin, teacherA, teacherB, studentInA);
        await context.SaveChangesAsync(CancellationToken.None);

        context.Assignments.AddRange(
            new Assignment
            {
                Title = "Published in A",
                Description = "d",
                ClassId = classA.Id,
                SubjectId = subjectA.Id,
                TeacherId = teacherA.Id,
                Deadline = DateTime.UtcNow.AddDays(5),
                MaxMarks = 100,
                Status = AssignmentStatus.Published
            },
            new Assignment
            {
                Title = "Draft in A",
                Description = "d",
                ClassId = classA.Id,
                SubjectId = subjectA.Id,
                TeacherId = teacherA.Id,
                Deadline = DateTime.UtcNow.AddDays(5),
                MaxMarks = 100,
                Status = AssignmentStatus.Draft
            },
            new Assignment
            {
                Title = "Published in B",
                Description = "d",
                ClassId = classB.Id,
                SubjectId = subjectB.Id,
                TeacherId = teacherB.Id,
                Deadline = DateTime.UtcNow.AddDays(5),
                MaxMarks = 100,
                Status = AssignmentStatus.Published
            });
        await context.SaveChangesAsync(CancellationToken.None);

        return new TestScenario(classA, admin, teacherA, studentInA);
    }

    private record TestScenario(Class ClassA, User Admin, User TeacherA, User StudentInA);
}

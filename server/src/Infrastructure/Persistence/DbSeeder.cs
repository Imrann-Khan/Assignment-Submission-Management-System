using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        if (await context.Users.AnyAsync())
        {
            return;
        }

        var classA = new Class { Name = "Class 10 - A" };
        var classB = new Class { Name = "Class 10 - B" };
        context.Classes.AddRange(classA, classB);
        await context.SaveChangesAsync();

        var math = new Subject { Name = "Mathematics", ClassId = classA.Id };
        var physics = new Subject { Name = "Physics", ClassId = classA.Id };
        var english = new Subject { Name = "English", ClassId = classB.Id };
        context.Subjects.AddRange(math, physics, english);
        await context.SaveChangesAsync();

        var admin = new User
        {
            FullName = "System Admin",
            Email = "admin@example.com",
            PasswordHash = passwordHasher.Hash("Admin@123"),
            Role = UserRole.Admin,
            IsActive = true
        };

        var teacher1 = new User
        {
            FullName = "Rahim Ahmed",
            Email = "teacher1@example.com",
            PasswordHash = passwordHasher.Hash("Teacher@123"),
            Role = UserRole.Teacher,
            IsActive = true
        };

        var teacher2 = new User
        {
            FullName = "Karim Hasan",
            Email = "teacher2@example.com",
            PasswordHash = passwordHasher.Hash("Teacher@123"),
            Role = UserRole.Teacher,
            IsActive = true
        };

        var student1 = new User
        {
            FullName = "Ayesha Siddiqua",
            Email = "student1@example.com",
            PasswordHash = passwordHasher.Hash("Student@123"),
            Role = UserRole.Student,
            IsActive = true,
            ClassId = classA.Id
        };

        var student2 = new User
        {
            FullName = "Nusrat Jahan",
            Email = "student2@example.com",
            PasswordHash = passwordHasher.Hash("Student@123"),
            Role = UserRole.Student,
            IsActive = true,
            ClassId = classA.Id
        };

        var student3 = new User
        {
            FullName = "Tanvir Islam",
            Email = "student3@example.com",
            PasswordHash = passwordHasher.Hash("Student@123"),
            Role = UserRole.Student,
            IsActive = true,
            ClassId = classB.Id
        };

        context.Users.AddRange(admin, teacher1, teacher2, student1, student2, student3);
        await context.SaveChangesAsync();

        context.TeacherSubjectAssignments.AddRange(
            new TeacherSubjectAssignment { TeacherId = teacher1.Id, SubjectId = math.Id },
            new TeacherSubjectAssignment { TeacherId = teacher1.Id, SubjectId = physics.Id },
            new TeacherSubjectAssignment { TeacherId = teacher2.Id, SubjectId = english.Id }
        );
        await context.SaveChangesAsync();

        var publishedAssignment = new Assignment
        {
            Title = "Algebra Basics",
            Description = "Solve the attached algebra problems covering linear equations.",
            ClassId = classA.Id,
            SubjectId = math.Id,
            TeacherId = teacher1.Id,
            Deadline = DateTime.UtcNow.AddDays(7),
            MaxMarks = 100,
            Status = AssignmentStatus.Published
        };

        var draftAssignment = new Assignment
        {
            Title = "Newton's Laws of Motion",
            Description = "Draft assignment covering the three laws of motion.",
            ClassId = classA.Id,
            SubjectId = physics.Id,
            TeacherId = teacher1.Id,
            Deadline = DateTime.UtcNow.AddDays(10),
            MaxMarks = 50,
            Status = AssignmentStatus.Draft
        };

        var gradedAssignment = new Assignment
        {
            Title = "Essay Writing",
            Description = "Write a 500-word essay on climate change.",
            ClassId = classB.Id,
            SubjectId = english.Id,
            TeacherId = teacher2.Id,
            Deadline = DateTime.UtcNow.AddDays(-2),
            MaxMarks = 20,
            Status = AssignmentStatus.Published
        };

        context.Assignments.AddRange(publishedAssignment, draftAssignment, gradedAssignment);
        await context.SaveChangesAsync();

        var pendingSubmission = new Submission
        {
            AssignmentId = publishedAssignment.Id,
            StudentId = student1.Id,
            AnswerText = "Here are my solutions to the algebra problems: ...",
            SubmittedAt = DateTime.UtcNow.AddDays(-1),
            Status = SubmissionStatus.Submitted
        };

        var gradedSubmission = new Submission
        {
            AssignmentId = gradedAssignment.Id,
            StudentId = student3.Id,
            AnswerText = "Climate change is one of the most pressing issues of our time...",
            SubmittedAt = DateTime.UtcNow.AddDays(-3),
            Status = SubmissionStatus.Graded,
            Marks = 17,
            Feedback = "Well-structured essay with clear arguments. Minor grammar issues.",
            GradedAt = DateTime.UtcNow.AddDays(-1),
            GradedById = teacher2.Id
        };

        context.Submissions.AddRange(pendingSubmission, gradedSubmission);
        await context.SaveChangesAsync();
    }
}

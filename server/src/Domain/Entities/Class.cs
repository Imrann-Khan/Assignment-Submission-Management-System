namespace Domain.Entities;

public class Class : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;

    public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
    public ICollection<User> Students { get; set; } = new List<User>();
    public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
}

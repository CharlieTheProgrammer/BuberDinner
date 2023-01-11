namespace BuberDinner.Domain.Entities;
public class User
{
    public Guid Id { get; init; } = new Guid();
    // null! (aka null-forgiving) operator means that null should not be assigned as a value.
    // However, this is only enforced as a message and not during runtime.
    // string? would not work because that means the value can be null, but that's not what I want to indicate.
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
}

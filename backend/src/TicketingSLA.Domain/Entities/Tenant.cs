namespace TicketingSLA.Domain.Entities;

public class Tenant
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public bool IsActive { get; private set; }

    private Tenant() { }

    public Tenant(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tenant name cannot be empty.", nameof(name));

        Id = Guid.NewGuid();
        Name = name;
        IsActive = true;
    }

    public void Deactivate() => IsActive = false;
}
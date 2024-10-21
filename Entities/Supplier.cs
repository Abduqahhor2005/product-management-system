namespace ProductManagementSystem.Entities;

public class Supplier
{
    public int Id { get; set; }
    public string Name { get; set; } = String.Empty;
    public string ContactPerson { get; set; } = String.Empty;
    public string Email { get; set; } = String.Empty;
    public string Phone { get; set; } = String.Empty;
    public ICollection<Order> Orders { get; set; } = [];
}
namespace EventManagement.Core.Models;

// Model lokacije na kojoj se održava događaj
public class Venue
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int Capacity { get; set; }

    // 1:N — jedna lokacija ima mnogo događaja
    public ICollection<Event> Events { get; set; } = new List<Event>();
}
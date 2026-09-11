namespace EventManagement.Core.Models;

public class EventCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    // 1:N — jedna kategorija ima mnogo događaja
    public ICollection<Event> Events { get; set; } = new List<Event>();
}
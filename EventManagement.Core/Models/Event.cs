namespace EventManagement.Core.Models;

public class Event
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int MaxParticipants { get; set; }

    // 1:N — Event pripada jednoj kategoriji
    public int CategoryId { get; set; }
    public EventCategory Category { get; set; } = null!;

    // 1:N — Event se održava na jednoj lokaciji
    public int VenueId { get; set; }
    public Venue Venue { get; set; } = null!;

    // 1:N — Event ima jednog organizatora
    public int OrganizerId { get; set; }
    public User Organizer { get; set; } = null!;

    // Navigacijska svojstva
    public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
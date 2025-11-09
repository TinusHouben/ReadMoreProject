using ReadMore.Models;

public class ContactMessage
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Message { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public bool IsResolved { get; set; }

    public ApplicationUser? User { get; set; }

    public string UserName { get; set; } = null!;
}

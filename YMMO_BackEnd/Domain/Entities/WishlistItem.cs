namespace YMMO.Backend.Domain.Entities;

public class WishlistItem
{
    public Guid WishlistItemID { get; set; }
    
    // N:1 Relationships
    public Guid ClientID { get; set; }
    public Client Client { get; set; } = null!;
    
    public Guid PropertyID { get; set; }
    public Property Property { get; set; } = null!;
}
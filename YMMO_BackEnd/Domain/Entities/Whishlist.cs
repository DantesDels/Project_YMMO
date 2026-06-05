namespace YMMO.Backend.Domain.Entities;

public class Wishlist
{
    public Guid WishlistID { get; set; }
    
    // N:1 Relationships
    public Guid ClientID { get; set; }
    public Client Client { get; set; }
    
    public Guid PropertyID { get; set; }
    public Property Property { get; set; }
}
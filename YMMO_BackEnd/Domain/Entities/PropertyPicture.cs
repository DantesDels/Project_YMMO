namespace YMMO.Backend.Domain.Entities;

public class PropertyPicture
{
    public Guid PropertyPictureId { get; set; } = Guid.NewGuid();
    public required string Url { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsMain { get; set; }

    // N:1 — A picture always belongs to a property
    public Guid PropertyId { get; set; }
    public Property Property { get; set; } = null!;
}
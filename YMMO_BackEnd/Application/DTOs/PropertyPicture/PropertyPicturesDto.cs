namespace YMMO.Backend.Application.DTOs.PropertyPicture;

public class PropertyPictureDto
{
    public Guid PropertyPictureId { get; set; }
    public string Url { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsMain { get; set; }
}
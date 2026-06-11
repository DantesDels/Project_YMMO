namespace YMMO.Backend.Application.DTOs.PropertyPicture;

public class PropertyPictureUploadDto
{
    // if the format is a Binary file
    public IFormFile File { get; set; } = null!;
    
    // Define main picture used for the property
    public bool IsMain { get; set; } 
}
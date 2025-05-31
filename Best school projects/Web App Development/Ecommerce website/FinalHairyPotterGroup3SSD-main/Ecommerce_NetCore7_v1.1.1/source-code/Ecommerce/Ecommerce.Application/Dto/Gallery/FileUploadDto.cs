using Microsoft.AspNetCore.Http;

namespace Ecommerce.Application.Dto;
public class FileUploadDto
{
    public IFormFile File { get; set; }
    public string? FileName { get; set; }
}
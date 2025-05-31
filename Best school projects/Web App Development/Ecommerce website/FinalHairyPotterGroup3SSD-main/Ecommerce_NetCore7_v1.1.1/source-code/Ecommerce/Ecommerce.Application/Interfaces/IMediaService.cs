using Ecommerce.Application.Dto;

namespace Ecommerce.Application.Interfaces;

public interface IMediaService
{
    Task<IEnumerable<GalleryDto>> GetAsync();
    Task<GalleryDto> GetByIdAsync(string id);
    Task<IEnumerable<GalleryDto>> GetPagedAsync(int pageIndex, int pageSize);
    Task<GalleryDto> UpdateAsync(GalleryDto gallery);
    Task<string> RemoveAsync(string id);
    Task<string> FileUploadAsync(FileUploadDto mediaUpload);
}

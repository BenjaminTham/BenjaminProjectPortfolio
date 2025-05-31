using Ecommerce.Application.Dto;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Identity.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Mvc.Controllers;

[Authorize]
public class GalleryController : Controller
{
    private readonly IMediaService _mediaService;
    public GalleryController(IMediaService mediaService)
    {
        _mediaService = mediaService;
    }
    [Authorize(Permissions.Permissions_Gallery_Manage)]
    public IActionResult Index()
    {
        return View();
    }

    [Route("getpagedmedia")]
    [Route("[controller]/[action]")]
    [HttpGet]
    public async Task<ActionResult> GetMedia(int pageIndex, int pageSize)
    {
        var query = await _mediaService.GetPagedAsync(pageIndex, pageSize);
        return Json(query);
    }

    [Route("getmediabyid/{id}")]
    [Route("[controller]/[action]/{id}")]
    [HttpGet]
    public async Task<ActionResult> GetMediaById(string id)
    {
        var query = await _mediaService.GetByIdAsync(id);
        return Json(query);
    }



    [HttpPost]
    [Route("uploadmedia")]
    [Route("[controller]/[action]/{id}")]
    public async Task<IActionResult> UploadMedia(ICollection<IFormFile> files)
    {
        if (files.Count == 0)
        {
            return Json(new { message = "No media detected!" });
        }

        var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".gif", ".bmp", // Images
        ".pdf" // PDFs
    };

        var allowedMimeTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/png", "image/gif", "image/bmp",
        "application/pdf"
    };

        var disallowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        ".exe", ".bat", ".cmd", ".sh", ".js", ".msi", ".scr", ".dll"
    };

        foreach (var file in files)
        {
            string fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            string mimeType = file.ContentType.ToLowerInvariant();

            // Check for misleading file names and double extensions
            string[] splitNames = file.FileName.Split('.');
            if (splitNames.Length > 2 || disallowedExtensions.Contains(fileExtension))
            {
                return Json(new { message = "Misleading file name or disallowed file type." });
            }

            if (!allowedExtensions.Contains(fileExtension) || !allowedMimeTypes.Contains(mimeType))
            {
                return Json(new { message = "Invalid file type." });
            }

            // Check the file content
            using (var stream = file.OpenReadStream())
            {
                if (!FileSignatureChecker.IsValidFileSignature(stream, fileExtension))
                {
                    return Json(new { message = "Invalid file content." });
                }
            }

            // Proceed with the file upload process
            FileUploadDto fileUpload = new()
            {
                File = file
            };
            await _mediaService.FileUploadAsync(fileUpload);
        }

        return Json(new { success = true });
    }


    public static class FileSignatureChecker
    {
        // Dictionary to map file extensions to their corresponding byte signatures
        private static readonly Dictionary<string, List<byte[]>> FileSignatures = new Dictionary<string, List<byte[]>>
    {
        { ".jpg", new List<byte[]> { new byte[] { 0xFF, 0xD8, 0xFF } } },
        { ".jpeg", new List<byte[]> { new byte[] { 0xFF, 0xD8, 0xFF } } },
        { ".png", new List<byte[]> { new byte[] { 0x89, 0x50, 0x4E, 0x47 } } },
        { ".gif", new List<byte[]> { new byte[] { 0x47, 0x49, 0x46, 0x38 } } },
        { ".bmp", new List<byte[]> { new byte[] { 0x42, 0x4D } } },
        { ".pdf", new List<byte[]> { new byte[] { 0x25, 0x50, 0x44, 0x46 } } }
    };

        public static bool IsValidFileSignature(Stream fileStream, string extension)
        {
            if (!FileSignatures.ContainsKey(extension))
                return false;

            List<byte[]> signatures = FileSignatures[extension];
            byte[] headerBytes = new byte[signatures.Max(m => m.Length)];
            fileStream.Read(headerBytes, 0, headerBytes.Length);

            return signatures.Any(signature => headerBytes.Take(signature.Length).SequenceEqual(signature));
        }
    }


    [HttpPost]
    [Route("updatemedia")]
    [Route("[controller]/[action]/{id}")]
    public async Task<ActionResult> UpdateMediaInfo(GalleryDto gallery)
    {
        var res = await _mediaService.UpdateAsync(gallery);
        return Json("{success:true}");
    }

    [Route("deletemedia")]
    [Route("[controller]/[action]")]
    [HttpPost]
    public async Task<ActionResult> DeleteMedia(string id)
    {
        try
        {
            var res = await _mediaService.RemoveAsync(id);
            return Json("success");
        }
        catch
        {
            return Json("failed");
        }

    }
}

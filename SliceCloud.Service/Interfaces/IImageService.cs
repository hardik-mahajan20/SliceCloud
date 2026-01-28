using Microsoft.AspNetCore.Http;

namespace SliceCloud.Service.Interfaces;

public interface IImageService
{
    /// <summary>
    /// Saves an uploaded image and returns its file path.
    /// </summary>
    /// <param name="Img">The uploaded image file.</param>
    /// <returns>A task that returns the file path of the saved image.</returns>
    Task<string?> ImgPath(IFormFile? Img);
}
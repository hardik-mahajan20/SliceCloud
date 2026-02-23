using Microsoft.AspNetCore.Http;
using SliceCloud.Repository.Constants;
using SliceCloud.Service.Interfaces;

namespace SliceCloud.Service.Implementations;

public class ImageService : IImageService
{
    public async Task<string?> ImgPath(IFormFile? Img)
    {
        if (Img != null)
        {
            string fileGuid = Guid.NewGuid().ToString();

            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), GenralConstants.WWWROOT, GenralConstants.IMAGES, GenralConstants.UPLOADS);

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }


            string fileExtension = Path.GetExtension(Img.FileName);
            string filePath = Path.Combine(uploadsFolder, fileGuid + fileExtension);

            using (FileStream? fileStream = new(filePath, FileMode.Create))
            {
                await Img.CopyToAsync(fileStream);
            }

            string path = fileGuid + fileExtension;
            return path;
        }
        return null;
    }

}

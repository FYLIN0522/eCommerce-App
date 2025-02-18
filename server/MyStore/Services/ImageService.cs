using System.Security.Cryptography;

namespace MyStore.Services
{
    public class ImageService
    {
        private readonly string FilePath = Path.Combine("Storage", "images");

        public async Task<(byte[] image, string? mimeType)> ReadImage(string fileName)
        {
            var fullPath = Path.Combine(FilePath, fileName);
            var mimeType = GetImageMimeType(fileName);
            byte[] imageBytes = await File.ReadAllBytesAsync(fullPath);

            return (imageBytes, mimeType);
        }


        public async Task RemoveImage(string fileName)
        {
            var fullPath = Path.Combine(FilePath, fileName);
            if (File.Exists(fullPath))
            {
                await Task.Run(() => File.Delete(fullPath));
            }
        }


        public async Task<string> AddImage(byte[] image, string fileExt)
        {
            var fileName = Guid.NewGuid().ToString("N") + fileExt;

            var fullPath = Path.Combine(FilePath, fileName);

            try
            {
                await File.WriteAllBytesAsync(fullPath, image);
                return fileName;
            }
            catch (Exception err)
            {
                Console.Error.WriteLine($"Error: {err.Message}");

                if (File.Exists(fullPath))
                {
                    await Task.Run(() => File.Delete(fullPath));
                }
                throw;
            }
        }

        public string GetImageMimeType(string fileName)
        {
            if (fileName.EndsWith(".jpeg") || fileName.EndsWith(".jpg")) return "image/jpeg";
            if (fileName.EndsWith(".png")) return "image/png";
            if (fileName.EndsWith(".gif")) return "image/gif";

            return "application/octet-stream";
        }


        public string? GetImageExtension(string fileExt)
        {
            switch (fileExt)
            {
                case "image/jpeg":
                    return ".jpeg";

                case "image/png":
                    return ".png";

                case "image/gif":
                    return ".gif";

                default:
                    return null;
            }
        }
    }
}

namespace Mimarlik.Core.Interfaces;

public interface IFileService
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string folder = "uploads");
    Task<string> SaveImageWithWatermarkAsync(Stream imageStream, string fileName, string watermarkText, string folder = "uploads");
    Task DeleteFileAsync(string filePath);
    Task<bool> FileExistsAsync(string filePath);
    string GetFileUrl(string filePath);
    Task<(int width, int height)> GetImageDimensionsAsync(Stream imageStream);
}
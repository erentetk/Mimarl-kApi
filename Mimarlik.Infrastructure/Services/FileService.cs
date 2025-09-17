using Mimarlik.Core.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using SixLabors.Fonts;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

namespace Mimarlik.Infrastructure.Services;

public class FileService : IFileService
{
    private readonly string _uploadsPath;
    private readonly string _baseUrl;

    public FileService(IConfiguration configuration)
    {
        _uploadsPath = configuration["FileSettings:UploadsPath"] ?? "Uploads";
        _baseUrl = configuration["FileSettings:BaseUrl"] ?? "";
        
        // Ensure upload directory exists
        if (!Directory.Exists(_uploadsPath))
        {
            Directory.CreateDirectory(_uploadsPath);
        }
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string folder = "uploads")
    {
        var folderPath = Path.Combine(_uploadsPath, folder);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var uniqueFileName = GenerateUniqueFileName(fileName);
        var filePath = Path.Combine(folderPath, uniqueFileName);

        using var fileStreamOut = new FileStream(filePath, FileMode.Create);
        await fileStream.CopyToAsync(fileStreamOut);

        return Path.Combine(folder, uniqueFileName).Replace("\\", "/");
    }

    public async Task<string> SaveImageWithWatermarkAsync(Stream imageStream, string fileName, string watermarkText, string folder = "uploads")
    {
        var folderPath = Path.Combine(_uploadsPath, folder);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var uniqueFileName = GenerateUniqueFileName(fileName);
        var filePath = Path.Combine(folderPath, uniqueFileName);

        using var image = await Image.LoadAsync(imageStream);
        
        // Add watermark
        AddWatermark(image, watermarkText);

        await image.SaveAsync(filePath);

        return Path.Combine(folder, uniqueFileName).Replace("\\", "/");
    }

    public async Task DeleteFileAsync(string filePath)
    {
        var fullPath = Path.Combine(_uploadsPath, filePath);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
        await Task.CompletedTask;
    }

    public async Task<bool> FileExistsAsync(string filePath)
    {
        var fullPath = Path.Combine(_uploadsPath, filePath);
        return await Task.FromResult(File.Exists(fullPath));
    }

    public string GetFileUrl(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return string.Empty;

        return $"{_baseUrl.TrimEnd('/')}/{filePath}";
    }

    public async Task<(int width, int height)> GetImageDimensionsAsync(Stream imageStream)
    {
        using var image = await Image.LoadAsync(imageStream);
        return (image.Width, image.Height);
    }

    private void AddWatermark(Image image, string watermarkText)
    {
        if (string.IsNullOrEmpty(watermarkText))
            return;

        // Calculate font size based on image size
        var fontSize = Math.Max(12, image.Width / 40);
        
        image.Mutate(ctx =>
        {
            // Create a semi-transparent background for the watermark
            var watermarkBounds = new Rectangle(
                image.Width - (watermarkText.Length * fontSize / 2) - 20,
                image.Height - fontSize - 20,
                watermarkText.Length * fontSize / 2 + 10,
                fontSize + 10
            );

            // Add semi-transparent background
            ctx.Fill(Color.FromRgba(0, 0, 0, 128), watermarkBounds);

            // Add watermark text
            var textOptions = new RichTextOptions(SystemFonts.CreateFont("Arial", fontSize))
            {
                Origin = new PointF(watermarkBounds.X + 5, watermarkBounds.Y + 5),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };

            ctx.DrawText(textOptions, watermarkText, Color.White);
        });
    }

    private string GenerateUniqueFileName(string fileName)
    {
        var fileExtension = Path.GetExtension(fileName);
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        
        // Clean the filename - remove problematic characters and limit length
        fileNameWithoutExtension = Regex.Replace(fileNameWithoutExtension, @"[^a-zA-Z0-9_-]", "_");
        
        // Limit filename length to prevent filesystem issues
        if (fileNameWithoutExtension.Length > 50)
        {
            fileNameWithoutExtension = fileNameWithoutExtension.Substring(0, 50);
        }
        
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        
        return $"{timestamp}_{uniqueId}_{fileNameWithoutExtension}{fileExtension}";
    }
}
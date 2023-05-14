using System;
using System.Collections.Generic;
using System.IO;
using SkiaSharp;
using Svg.Skia;

namespace SvgBuild;

public static class Renderer
{
    private static readonly Dictionary<string, SKEncodedImageFormat> ExtensionFormats = new()
    {
        [".bmp"] = SKEncodedImageFormat.Bmp,
        [".gif"] = SKEncodedImageFormat.Gif,
        [".ico"] = SKEncodedImageFormat.Ico,
        [".jpeg"] = SKEncodedImageFormat.Jpeg,
        [".jpg"] = SKEncodedImageFormat.Jpeg,
        [".png"] = SKEncodedImageFormat.Png
    };

    public static void Render(string inputPath, string outputPath, SKSize? size = null)
    {
        if (inputPath == null)
        {
            throw new ArgumentNullException(nameof(inputPath));
        }

        if (outputPath == null)
        {
            throw new ArgumentNullException(nameof(outputPath));
        }

        var detectedFormat = DetermineOutputFormat(Path.GetExtension(outputPath));
        if (detectedFormat is not {} format)
        {
            throw new ArgumentException(
                $"Cannot determine output file format based on the file extension {Path.GetExtension(outputPath)}",
                nameof(outputPath));
        }

        var outputDirectory = Path.GetDirectoryName(Path.GetFullPath(outputPath));
        if (outputDirectory == null)
        {
            throw new ArgumentException(
                $"""Cannot determine the parent path of path "{outputPath}".""",
                nameof(outputPath));
        }

        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        using var svg = new SKSvg();
        svg.Load(inputPath);

        var scaleX = 1.0f;
        var scaleY = 1.0f;
        if (size is {} targetSize)
        {
            var sourceSize = svg.Picture?.CullRect.Size ??
                             throw new Exception("Cannot determine the source image size");
            scaleX = targetSize.Width / sourceSize.Width;
            scaleY = targetSize.Height / sourceSize.Height;
        }

        svg.Save(outputPath, SKColors.Transparent, format, scaleX: scaleX, scaleY: scaleY);
    }

    private static SKEncodedImageFormat? DetermineOutputFormat(string extension)
    {
        return ExtensionFormats.TryGetValue(extension, out var format) ? format : null;
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Svg;
using SvgBuild.Savers;

namespace SvgBuild
{
    public static class Renderer
    {
        private static readonly Dictionary<string, ImageFormat> _extensionFormats = new Dictionary<string, ImageFormat>
        {
            [".bmp"] = ImageFormat.Bmp,
            [".gif"] = ImageFormat.Gif,
            [".ico"] = ImageFormat.Icon,
            [".jpeg"] = ImageFormat.Jpeg,
            [".jpg"] = ImageFormat.Jpeg,
            [".png"] = ImageFormat.Png,
            [".tiff"] = ImageFormat.Tiff,
        };

        public static void Render(string inputPath, string outputPath, Size? size = null)
        {
            if (inputPath == null)
            {
                throw new ArgumentNullException(nameof(inputPath));
            }

            if (outputPath == null)
            {
                throw new ArgumentNullException(nameof(outputPath));
            }

            var format = DetermineOutputFormat(Path.GetExtension(outputPath));
            if (format == null)
            {
                throw new ArgumentException(
                    $"Cannot determine output file format based on the file extension {Path.GetExtension(outputPath)}",
                    nameof(outputPath));
            }

            var outputDirectory = Path.GetDirectoryName(Path.GetFullPath(outputPath));
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            var document = SvgDocument.Open(inputPath);
            var processor = CreateImageSaver(format);
            var bitmapSize = size ?? Size.Ceiling(document.GetDimensions());
            using (var image = new Bitmap(bitmapSize.Width, bitmapSize.Height))
            {
                document.Draw(image);
                processor.Save(image, format, outputPath);
            }
        }

        private static ImageFormat DetermineOutputFormat(string extension)
        {
            return _extensionFormats.TryGetValue(extension, out var format) ? format : null;
        }

        private static IImageSaver CreateImageSaver(ImageFormat format)
        {
            if (Equals(format, ImageFormat.Icon))
            {
                return new IconSaver();
            }

            return new SystemDrawingSaver();
        }
    }
}

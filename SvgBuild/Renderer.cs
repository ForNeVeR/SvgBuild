using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.IconLib;
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

        public static void Render(string inputPath, string outputPath)
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

            var document = SvgDocument.Open(inputPath);
            var processor = CreateImageSaver(format);
            var size = document.GetDimensions().ToSize();
            using (var image = new Bitmap(size.Width, size.Height))
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
                return new IconLibSaver();
            }

            return new SystemDrawingSaver();
        }
    }
}

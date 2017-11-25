using System;
using System.Drawing;
using Svg;

namespace SvgBuild
{
    public static class Renderer
    {
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

            var document = SvgDocument.Open(inputPath);
            var size = document.GetDimensions().ToSize();
            using (var image = new Bitmap(size.Width, size.Height))
            {
                document.Draw(image);
                image.Save(outputPath);
            }
        }
    }
}

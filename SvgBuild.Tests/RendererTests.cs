using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using SkiaSharp;
using Xunit;

namespace SvgBuild.Tests
{
    public class RendererTests
    {
        [Fact]
        public void RendererThrowsArgumentNullExceptionIfInputOrOutputIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => Renderer.Render(null, "xxx"));
            Assert.Throws<ArgumentNullException>(() => Renderer.Render("xxx", null));
        }

        [Fact]
        public void RendererThrowsArgumentExceptionIfOutputHasInvalidFormat()
        {
            Assert.Throws<ArgumentException>(() => Renderer.Render("file.svg", "file.txt"));
        }

        [Fact]
        public async Task SavedImageHasRequiredSize()
        {
            var input = await SvgUtilities.CreateTempImage();
            var output = Path.ChangeExtension(Path.GetTempFileName(), "png");
            var size = new SKSize(32, 32);
            Renderer.Render(input, output, size);

            var image = Image.FromFile(output);
            Assert.Equal((size.Width, size.Width), (image.Size.Width, image.Size.Height));
        }

        [Fact]
        public Task RendererCreatesBmpImage() => AssertRenderedImageFormat("bmp", ImageFormat.Bmp);

        [Fact]
        public Task RendererCreatesGifImage() => AssertRenderedImageFormat("gif", ImageFormat.Gif);

        [Fact]
        public Task RendererCreatesIcoImage()
        {
            // Skip on Mono because their Image reader is buggy:
            if (!Environment.OSVersion.Platform.ToString().StartsWith("Win"))
            {
                return Task.CompletedTask;
            }

            return AssertRenderedImageFormat("ico", ImageFormat.Icon);
        }

        [Fact]
        public async Task RendererCreatesJpegImages()
        {
            await AssertRenderedImageFormat("jpg", ImageFormat.Jpeg);
            await AssertRenderedImageFormat("jpeg", ImageFormat.Jpeg);
        }

        [Fact]
        public Task RendererCreatesPngImage() => AssertRenderedImageFormat("png", ImageFormat.Png);

        [Fact]
        public async Task RendererCreatesNecessaryDirectories()
        {
            var input = await SvgUtilities.CreateTempImage();
            var output = Path.Combine(
                Path.GetDirectoryName(input)!,
                Guid.NewGuid().ToString(),
                Path.ChangeExtension(Path.GetFileNameWithoutExtension(input), ".png")!);
            Renderer.Render(input, output);
        }

        private async Task AssertRenderedImageFormat(string outputFileExtension, ImageFormat format)
        {
            var input = await SvgUtilities.CreateTempImage();
            var output = Path.ChangeExtension(Path.GetTempFileName(), outputFileExtension);
            Renderer.Render(input, output);

            var image = Image.FromFile(output);
            Assert.Equal(format, image.RawFormat);
        }
    }
}

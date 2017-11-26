using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace SvgBuild.Tests
{
    public class RendererTests
    {
        [Fact]
        public void RendererThorwsArgumentNullExceptionIfInputOrOutputIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => Renderer.Render(null, "xxx"));
            Assert.Throws<ArgumentNullException>(() => Renderer.Render("xxx", null));
        }

        [Fact]
        public void RendererThorwsArgumentExceptionIfOutputHasInvalidFormat()
        {
            Assert.Throws<ArgumentException>(() => Renderer.Render("file.svg", "file.txt"));
        }

        [Fact]
        public Task RendererCreatesBmpImage() => AssertRenderedImageFormat("bmp", ImageFormat.Bmp);

        [Fact]
        public Task RendererCreatesGifImage() => AssertRenderedImageFormat("gif", ImageFormat.Gif);

        [Fact]
        public Task RendererCreatesIcoImage() => AssertRenderedImageFormat("ico", ImageFormat.Icon);

        [Fact]
        public async Task RendererCreatesJpegImages()
        {
            await AssertRenderedImageFormat("jpg", ImageFormat.Jpeg);
            await AssertRenderedImageFormat("jpeg", ImageFormat.Jpeg);
        }

        [Fact]
        public Task RendererCreatesPngImage() => AssertRenderedImageFormat("png", ImageFormat.Png);

        [Fact]
        public Task RendererCreatesTiffImage() => AssertRenderedImageFormat("tiff", ImageFormat.Tiff);

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

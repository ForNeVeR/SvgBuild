using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace SvgBuild.Tests
{
    public class RendererTests
    {
        [Fact]
        public void RendererThorwsArgumentExceptionIfInputOrOutputIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => Renderer.Render(null, "xxx"));
            Assert.Throws<ArgumentNullException>(() => Renderer.Render("xxx", null));
        }

        [Fact]
        public async Task RendererRendersSimpleImage()
        {
            var path = await CreateTempImage();
            var output = Path.GetTempFileName();
            Renderer.Render(path, output);

            var image = Image.FromFile(output);
            Assert.IsType<Bitmap>(image);
        }

        private static async Task<string> CreateTempImage()
        {
            var fileName = Path.GetTempFileName();
            using (var file = new FileStream(fileName, FileMode.Create))
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                "SvgBuild.Tests.Resources.Image.svg"))
            {
                Assert.NotNull(stream);
                await stream.CopyToAsync(file);
            }

            return fileName;
        }
    }
}

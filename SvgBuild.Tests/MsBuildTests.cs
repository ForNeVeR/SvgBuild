using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using SvgBuild.MsBuild;
using Xunit;

namespace SvgBuild.Tests
{
    public class MsBuildTests
    {
        [Fact]
        public async Task SvgBuildTaskRendersTheFileWithDefaultSize()
        {
            var path = await SvgUtilities.CreateTempImage();
            var output = Path.ChangeExtension(Path.GetTempFileName(), "png");
            var task = new SvgBuildTask {InputPath = path, OutputPath = output};
            Assert.True(task.Execute());

            var image = Image.FromFile(output);
            Assert.IsType<Bitmap>(image);
            Assert.Equal(101, image.Height);
            Assert.Equal(101, image.Width);
        }

        [Fact]
        public async Task SvgBuildTaskThrowsArgumentExceptionIfOnlyOneDimensionIfDefined()
        {
            var path = await SvgUtilities.CreateTempImage();
            var output = Path.ChangeExtension(Path.GetTempFileName(), "png");
            var task = new SvgBuildTask {InputPath = path, OutputPath = output, Width = "123"};
            Assert.Throws<ArgumentException>(() => task.Execute());
        }

        [Fact]
        public async Task SvgBuildTaskRendersTheProperSize()
        {
            var path = await SvgUtilities.CreateTempImage();
            var output = Path.ChangeExtension(Path.GetTempFileName(), "png");
            var task = new SvgBuildTask {InputPath = path, OutputPath = output, Width = "32", Height = "32"};
            Assert.True(task.Execute());

            var image = Image.FromFile(output);
            Assert.Equal(32, image.Width);
            Assert.Equal(32, image.Height);
        }
    }
}

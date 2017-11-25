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
        public async Task SvgBuildTaskRendersTheFile()
        {
            var path = await SvgUtilities.CreateTempImage();
            var output = Path.GetTempFileName();
            var task = new SvgBuildTask {InputPath = path, OutputPath = output};
            Assert.True(task.Execute());

            var image = Image.FromFile(output);
            Assert.IsType<Bitmap>(image);
        }
    }
}

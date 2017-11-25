using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace SvgBuild.Tests
{
    internal static class SvgUtilities
    {
        public static async Task<string> CreateTempImage()
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

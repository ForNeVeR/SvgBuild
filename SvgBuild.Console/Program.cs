using System.Drawing;
using System.Globalization;

namespace SvgBuild.Console
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var input = args[0];
            var output = args[1];
            var size = args.Length > 2 ? (Size?)ParseSize(args[2]) : null;
            Renderer.Render(input, output, size);
        }

        private static Size ParseSize(string sizeString)
        {
            var components = sizeString.Split('x');
            var width = int.Parse(components[0], CultureInfo.InvariantCulture);
            var height = int.Parse(components[1], CultureInfo.InvariantCulture);
            return new Size(width, height);
        }
    }
}

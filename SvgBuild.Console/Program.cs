using System.Drawing;
using Svg;

namespace SvgBuild.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = args[0];
            var output = args[1];
            var d = SvgDocument.Open(input);
            var i = new Bitmap((int)d.Width.Value, (int)d.Height.Value);
            d.Draw(i);
            i.Save(output);
        }
    }
}

using System.Drawing;
using System.Drawing.Imaging;

namespace SvgBuild
{
    public interface IImageSaver
    {
        void Save(Bitmap image, ImageFormat format, string path);
    }
}

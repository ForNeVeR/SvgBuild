using System.Drawing;
using System.Drawing.Imaging;

namespace SvgBuild.Savers
{
    public class SystemDrawingSaver : IImageSaver
    {
        public void Save(Bitmap bitmap, ImageFormat format, string path) => bitmap.Save(path, format);
    }
}

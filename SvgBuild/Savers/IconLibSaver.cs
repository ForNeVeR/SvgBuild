using System.Drawing;
using System.Drawing.IconLib;
using System.Drawing.Imaging;
using System.IO;

namespace SvgBuild.Savers
{
    public class IconLibSaver : IImageSaver
    {
        public void Save(Bitmap bitmap, ImageFormat format, string path)
        {
            var multiIcon = new MultiIcon();
            var singleIcon = multiIcon.Add(Path.GetFileNameWithoutExtension(path));
            multiIcon.SelectedIndex = 0;

            singleIcon.CreateFrom(bitmap);
            multiIcon.Save(path, MultiIconFormat.ICO);
        }
    }
}

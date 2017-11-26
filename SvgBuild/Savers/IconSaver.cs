using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace SvgBuild.Savers
{
    public class IconSaver : IImageSaver
    {
        public void Save(Bitmap bitmap, ImageFormat format, string path)
        {
            // Code taken from Stack Overflow answer, https://stackoverflow.com/a/21389253/2684760
            // by Hans Passant (https://stackoverflow.com/users/17034/hans-passant).
            // Distributed under the terms of CC-BY-SA 3.0 license, https://creativecommons.org/licenses/by-sa/2.5/
            // Adapted to use with FileStream instead of MemoryStream.
            using (var stream = new FileStream(path, FileMode.Create))
            {
                var bw = new BinaryWriter(stream);
                // Header
                bw.Write((short)0);   // 0 : reserved
                bw.Write((short)1);   // 2 : 1=ico, 2=cur
                bw.Write((short)1);   // 4 : number of images
                // Image directory
                var w = bitmap.Width;
                if (w >= 256) w = 0;
                bw.Write((byte)w);    // 0 : width of image
                var h = bitmap.Height;
                if (h >= 256) h = 0;
                bw.Write((byte)h);    // 1 : height of image
                bw.Write((byte)0);    // 2 : number of colors in palette
                bw.Write((byte)0);    // 3 : reserved
                bw.Write((short)0);   // 4 : number of color planes
                bw.Write((short)0);   // 6 : bits per pixel
                var sizeHere = stream.Position;
                bw.Write((int)0);     // 8 : image size
                var start = (int)stream.Position + 4;
                bw.Write(start);      // 12: offset of image data
                // Image data
                bitmap.Save(stream, ImageFormat.Png);
                var imageSize = (int)stream.Position - start;
                stream.Seek(sizeHere, SeekOrigin.Begin);
                bw.Write(imageSize);
            }
        }
    }
}

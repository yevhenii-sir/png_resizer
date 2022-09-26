using System;
using System.IO;
using ImageMagick;

namespace ResizePNG
{
    class Program
    {
        static void Main(string[] args)
        {
            var size = 100;
            var quality = 100;
            bool saveOldImages = false;

            var path = AppDomain.CurrentDomain.BaseDirectory + "\\settingResize.ini";

            if (File.Exists(path))
            {
                try
                {
                    using (var streamReader = new StreamReader(path))
                    {
                        size = Convert.ToInt32(streamReader.ReadLine());
                        quality = Convert.ToInt32(streamReader.ReadLine());
                        saveOldImages = Convert.ToBoolean(streamReader.ReadLine());
                    }
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc);
                    Console.ReadLine();
                }
            }
            else
            {
                using (var streamWriter = new StreamWriter(path))
                {
                    streamWriter.WriteLine("100\r\n100\r\nfalse");
                }

                Console.WriteLine("File settings \"settingResize.ini\" initialized. \n100 - size percent\n100 - quality precent\nfalse - save old image");
                Console.ReadLine();
                Environment.Exit(-1);
            }

            foreach (var arg in args) { 
                using (var image = new MagickImage(arg))
                {
                    var coef = (size / 100.0);
                    
                    var newWidth = (int)(image.Width * coef);
                    var newHeight = (int)(image.Height * coef);

                    var sizeMagick = new MagickGeometry(newWidth, newHeight);
                    sizeMagick.IgnoreAspectRatio = true;

                    image.Resize(sizeMagick);
                    image.Quality = quality;

                    var exifProfile = image.GetExifProfile();
                    var iptcProfile = image.GetIptcProfile();
                    var xmpProfile = image.GetXmpProfile();

                    if (exifProfile != null) image.RemoveProfile(exifProfile);
                    if (iptcProfile != null) image.RemoveProfile(iptcProfile);
                    if (xmpProfile != null) image.RemoveProfile(xmpProfile);

                    if (saveOldImages)
                    {
                        image.Write(arg.Replace(".png", "") + $"_resize_{newWidth}x{newHeight}.png");
                    } else
                    {
                        image.Write(arg);
                    }
                }
            }
        }
    }
}

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using Path = System.IO.Path;

namespace NyaBackgroundTest
{
    class ImageHandler
    {
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SystemParametersInfo(uint uiAction, uint uiParam, String pvParam, uint fWinIni);

        private const uint SPI_SETDESKWALLPAPER = 0x14;


        public static void WallpaperSetter(string url)
        {
            string file_name = Path.Combine(Directory.GetCurrentDirectory(), "current.png");
            string background = Path.Combine(Directory.GetCurrentDirectory(), "bg.png");
            string _file_name;

            if (File.Exists(file_name)) File.Delete(file_name);
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(url, "current.png");
            }
            _file_name = MergeImages(file_name, background);
            if (_file_name.ToLower() == "false") Environment.Exit(0); else file_name = _file_name;
            uint flags = 0;
            if (!SystemParametersInfo(SPI_SETDESKWALLPAPER,
                    0, file_name, flags))
            {
                Console.WriteLine("Error");
            }
        }

        public static string MergeImages(string downloadImage, string backgroundImage)
        {
            Console.WriteLine("DEBUG: Merge called...");

            
            Configuration.Default.MemoryAllocator = ArrayPoolMemoryAllocator.CreateWithModeratePooling();
            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "current.png");
            Image<Rgba32> downloadImg = Image.Load<Rgba32>(downloadImage);
            Image<Rgba32> backgroundImg = Image.Load<Rgba32>(backgroundImage);
            int downloadImageX = downloadImg.Width;
            int downloadImageY = downloadImg.Height;
            int backgroundImageX = backgroundImg.Width;
            int backgroundImageY = backgroundImg.Height;                       
            downloadImg = Resizer(Corner.CornerImage(downloadImg, downloadImageX, downloadImageY), backgroundImg);
            downloadImageX = downloadImg.Width;
            downloadImageY = downloadImg.Height;
            float locXtemp = (backgroundImageX * .5f - downloadImageX * .5f);
            float locYtemp = (backgroundImageY * .5f - downloadImageY * .5f);
            int locX = Convert.ToInt32(locXtemp);
            int locY = Convert.ToInt32(locYtemp);
            using (Image<Rgba32> img1 = Image.Load<Rgba32>(backgroundImage))
            using (Image<Rgba32> img2 = Image.Load<Rgba32>(downloadImage))
            using (Image<Rgba32> outputImage = new(backgroundImageX, backgroundImageY))
            {
                outputImage.Mutate(o => o
                    .DrawImage(img1, new Point(0, 0), 1f) 
                    .DrawImage(img2, new Point(locX, locY), 1f)
                );
                outputImage.Save(imagePath);
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return imagePath;
            }
            else
            {
                Console.WriteLine("Manually change your wallpaper!");
                return "False";
            }
            Configuration.Default.MemoryAllocator.ReleaseRetainedResources();
            
        }

        static Image<Rgba32> Resizer(Image<Rgba32> dwnImage, Image<Rgba32> bgImage)
        {

            int dwnImageX = dwnImage.Width;
            int dwnImageY = dwnImage.Height;
            int bgImageX = bgImage.Width;
            int bgImageY = bgImage.Height;
            var resizeFactorX = bgImageX / dwnImageX * dwnImageX;
            var resizeFactorY = bgImageY / dwnImageY * dwnImageY;
            Console.WriteLine($"bg: {bgImageX},{bgImageY}; dwn: {dwnImageX},{dwnImageY}");
            Image<Rgba32> dwnImageResized;
            string tempLoc = Path.Combine(Directory.GetCurrentDirectory(), "current.png");

            if (bgImageX < dwnImageX)
            {
                File.Delete(tempLoc);
                dwnImage.Mutate(x => x.Resize(bgImageX, 0));
                dwnImage.Save(tempLoc);
                dwnImageResized = Image.Load<Rgba32>(tempLoc);
            }
            else if (bgImageY < dwnImageY)
            {
                File.Delete(tempLoc);
                dwnImage.Mutate(x => x.Resize(0, bgImageY));
                dwnImage.Save(tempLoc);
                dwnImageResized = Image.Load<Rgba32>(tempLoc);
            }
            dwnImageResized = dwnImage;
            return dwnImageResized;
        }

    }
}

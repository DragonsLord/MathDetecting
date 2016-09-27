using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace MathDetecting.Algorithms.Segmentation
{
    public static class FormatImage
    {
        public static void ChangeImage(Bitmap bm)
        {
            Color color;
            List<Point> sym = new List<Point>();
            for (int i = 0; i < bm.Width; i++)
            {
                for (int j = 0; j < bm.Height; j++)
                {
                    if (Segmentation.GetBrightness(bm.GetPixel(i, j)) > 150 && !sym.Contains(new Point(i, j)))
                    {
                        bm.SetPixel(i, j, Color.Black);
                        sym.Add(new Point(i, j));

                        if (i > 1)
                        {
                            if (Segmentation.GetBrightness(bm.GetPixel(i-1, j)) < 100)
                            {
                                bm.SetPixel(i - 1, j, Color.Black);
                                sym.Add(new Point(i - 1, j));
                            }
                        }
                        if (j > 1)
                        {
                            color = bm.GetPixel(i, j - 1);
                            if (Segmentation.GetBrightness(bm.GetPixel(i, j-1)) < 100)
                            {
                                bm.SetPixel(i, j - 1, Color.Black);
                                sym.Add(new Point(i, j - 1));
                            }
                        }
                    }
                    else
                    {
                        bm.SetPixel(i, j, Color.White);
                    }
                }
            }
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            return destImage;
        }
    }
}

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
                        //if (j > 1)
                        //{
                        //    color = bm.GetPixel(i, j - 1);
                        //    if (Segmentation.GetBrightness(bm.GetPixel(i, j-1)) < 100)
                        //    {
                        //        bm.SetPixel(i, j - 1, Color.Black);
                        //        sym.Add(new Point(i, j - 1));
                        //    }
                        //}
                    }
                    else
                    {
                        bm.SetPixel(i, j, Color.White);
                    }
                }
            }
        }

        public static Bitmap ResizeSmallImage(Image image, int width, int height)
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
        public static Bitmap ResizeImage(Image source, int width, int height)
        {

            Image dest = new Bitmap(width, height);
            using (Graphics gr = Graphics.FromImage(dest))
            {
                gr.FillRectangle(Brushes.White, 0, 0, width, height);  // Очищаем экран
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                float srcwidth = source.Width;
                float srcheight = source.Height;
                float dstwidth = width;
                float dstheight = height;

                if (srcwidth <= dstwidth && srcheight <= dstheight)  // Исходное изображение меньше целевого
                {
                     //Не изменяем размер картинки, а просто размещаем её по центру 
                    //int left = (width - source.Width) / 2;
                    //int top = (height - source.Height) / 2;
                    //gr.DrawImage(source, left, top, source.Width, source.Height);

                    return ResizeSmallImage(source, width, height); // растягиваем до нужного размера
                    
                }
                else if (srcwidth / srcheight > dstwidth / dstheight)  // Пропорции исходного изображения более широкие
                {
                    float cy = srcheight / srcwidth * dstwidth;
                    float top = ((float)dstheight - cy) / 2.0f;
                    if (top < 1.0f) top = 0;
                    gr.DrawImage(source, 0, top, dstwidth, cy);
                }
                else  // Пропорции исходного изображения более узкие
                {
                    float cx = srcwidth / srcheight * dstheight;
                    float left = ((float)dstwidth - cx) / 2.0f;
                    if (left < 1.0f) left = 0;
                    gr.DrawImage(source, left, 0, cx, dstheight);
                }

                return (Bitmap)dest;
            }
        }
    }
}

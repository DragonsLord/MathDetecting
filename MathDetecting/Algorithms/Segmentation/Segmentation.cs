using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MathDetecting.Algorithms.Segmentation
{
    public static class Segmentation
    {

        private const double left_k = 0;
        private const double right_k = 0;
        public static List<Bitmap> GetSymbolsByColumns(Bitmap image)
        {
            List<Bitmap> symbs = new List<Bitmap>();
            using (Bitmap copy = (Bitmap)image.Clone())
            {
                #region Spread Symbols
                //for (int i = 0; i < copy.Width; i += 2)
                //{
                //    for (int j = 0; j < copy.Height; j += 2)
                //    {
                //        Color color = copy.GetPixel(i, j);
                //        if (GetBrightness(color) > 150)
                //        {
                //            copy.SetPixel(i, j, Color.Black);
                //            copy.SetPixel(i, j - 1, Color.Black);
                //            copy.SetPixel(i, j + 1, Color.Black);
                //            copy.SetPixel(i - 1, j, Color.Black);
                //            copy.SetPixel(i - 1, j + 1, Color.Black);
                //            copy.SetPixel(i - 1, j - 1, Color.Black);
                //            copy.SetPixel(i + 1, j, Color.Black);
                //            copy.SetPixel(i + 1, j + 1, Color.Black);
                //            copy.SetPixel(i + 1, j - 1, Color.Black);
                //        }
                //        else
                //        {
                //            copy.SetPixel(i, j, Color.White);
                //        }
                //    }
                //}
                #endregion

                int[] ColumnBrightnes = new int[copy.Width];
                for (int i = 0; i < copy.Width; i++)
                {
                    ColumnBrightnes[i] = 0;
                    for (int j = 0; j < copy.Height; j++)
                    {
                        Color color = copy.GetPixel(i, j);
                        ColumnBrightnes[i] += GetBrightness(color);
                    }
                    ColumnBrightnes[i] /= copy.Height;
                }
                int average_btightnes = 0;
                for (int i = 0; i < ColumnBrightnes.Length; i++)
                    average_btightnes += ColumnBrightnes[i];
                average_btightnes /= copy.Width;
                int left_edge = (int)(left_k * average_btightnes) + 1;
                int right_edge = (int)(right_k * average_btightnes) + 1;
                List<int> bounds = new List<int>();
                for (int i = 1; i < ColumnBrightnes.Length - 1; i++)
                {
                    if (ColumnBrightnes[i - 1] <= left_edge && ColumnBrightnes[i] > left_edge && ColumnBrightnes[i + 1] > left_edge
                        && bounds.Count % 2 == 0)
                        bounds.Add(i);
                    else if (ColumnBrightnes[i - 1] > right_edge && ColumnBrightnes[i] <= right_edge
                        && ColumnBrightnes[i + 1] <= right_edge && bounds.Count % 2 == 1)
                        bounds.Add(i);
                }

                Bitmap b;

                for (int i = 0; i < ((bounds.Count % 2 == 0) ? bounds.Count : bounds.Count - 1); i += 2)
                {
                    b = new Bitmap(bounds[i + 1] - bounds[i] + 4, image.Height + 4);
                    using (Graphics g = Graphics.FromImage(b))
                    {
                        g.FillRectangle(Brushes.White, 0, 0, b.Width, b.Height);
                        g.DrawImage(image, new Rectangle(2, 2, b.Width - 2, b.Height - 2), new Rectangle(bounds[i], 0, b.Width - 4, b.Height - 4), GraphicsUnit.Pixel);
                    }
                    symbs.Add(CutText(b));
                }
            }
            return symbs;
        }

        public static List<Bitmap> GetSymbolsByRows(Bitmap image)
        {
            List<Bitmap> symbs;
            using (Bitmap rotate = (Bitmap)image.Clone())
            {
                rotate.RotateFlip(RotateFlipType.Rotate270FlipNone);
                symbs = GetSymbolsByColumns(rotate);
            }
            foreach (var s in symbs)
            {
                s.RotateFlip(RotateFlipType.Rotate90FlipNone);
            }
            return symbs;
        }

        public static Bitmap CutText(Bitmap image)
        {
            bool find = false;
            int left, right, bottom, top;
            int i = 0;
            while (!find)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    Color color = image.GetPixel(i, j);
                    if (GetBrightness(color) > 120)
                    {
                        find = true;
                        break;
                    }
                }
                i++;
            }
            left = i - 1;
            find = false;
            i = image.Width - 1;
            while (!find)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    Color color = image.GetPixel(i, j);
                    if (GetBrightness(color) > 120)
                    {
                        find = true;
                        break;
                    }
                }
                i--;
            }
            right = i + 1;
            find = false;
            i = 0;
            while (!find)
            {
                for (int j = 0; j < image.Width; j++)
                {
                    Color color = image.GetPixel(j, i);
                    if (GetBrightness(color) > 120)
                    {
                        find = true;
                        break;
                    }
                }
                i++;
            }
            bottom = i - 1;
            find = false;
            i = image.Height - 1;
            while (!find)
            {
                for (int j = 0; j < image.Width; j++)
                {
                    Color color = image.GetPixel(j, i);
                    if (GetBrightness(color) > 120)
                    {
                        find = true;
                        break;
                    }
                }
                i--;
            }
            top = i + 1;

            Bitmap b = new Bitmap(right - left  + 4, top - bottom + 4);
            using (Graphics g = Graphics.FromImage(b))
            {
                g.FillRectangle(Brushes.White, 0, 0, b.Width, b.Height);
                g.DrawImage(image, new Rectangle(2, 2, b.Width - 4, b.Height - 4), new Rectangle(left, bottom, right - left + 1, top - bottom + 1), GraphicsUnit.Pixel);
            }
            return b;
        }

        public static void CleanImage(Bitmap image)
        {
            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    Color color = image.GetPixel(i, j);
                    if (GetBrightness(color) > 200)
                    {
                        image.SetPixel(i, j, Color.Black);
                    }
                    else image.SetPixel(i, j, Color.White);
                }
            }
        }

        public static int GetBrightness(Color color)
        {
            int b = 0;
            b += (255 - color.R) * (255 - color.R);
            b += (255 - color.G) * (255 - color.G);
            b += (255 - color.B) * (255 - color.B);
            b = (int)Math.Sqrt(b);
            b *= (int)(0.004 * color.A);
            return b;
        }

        public static Bitmap GetRootSymbol(Bitmap bt)
        {
            Color color;
            Point start_point = new Point(2, bt.Height - 3);
            bool found = false;
            while (!found)
            {
                color = bt.GetPixel(start_point.X,start_point.Y);
                if (GetBrightness(color) > 150)
                    found = true;
                else {
                    start_point.X++;
                    start_point.Y--;
                }
            }
            List<Point> path = new List<Point>() { start_point };
            List<Point> symbol_pixels = new List<Point>();
            int right = int.MinValue, left = int.MaxValue, top = int.MinValue, bottom = int.MaxValue;
            #region Pathing
            while (path.Count != 0)
            {
                int x = path[0].X;
                int y = path[0].Y;
                bt.SetPixel(x, y, Color.White);
                if (x > right)
                    right = x;
                if (y > top)
                    top = y;
                if (y < bottom)
                    bottom = y;
                if (x < left)
                    left = x;
                Point p = new Point(x, y);
                symbol_pixels.Add(p);
                if (y + 1 < bt.Height)
                {
                    color = bt.GetPixel(x, y + 1);
                    p.Y = y + 1;
                    if (GetBrightness(color) > 150 && !path.Contains(p))
                        path.Add(new Point(x, y + 1));

                    if (x + 1 < bt.Width)
                    {
                        color = bt.GetPixel(x + 1, y + 1);
                        p.X = x + 1;
                        p.Y = y + 1;
                        if (GetBrightness(color) > 150 && !path.Contains(p))
                            path.Add(new Point(x + 1, y + 1));
                    }

                    if (x - 1 > 0)
                    {
                        color = bt.GetPixel(x - 1, y + 1);
                        p.X = x - 1;
                        p.Y = y + 1;
                        if (GetBrightness(color) > 150 && !path.Contains(p))
                            path.Add(new Point(x - 1, y + 1));
                    }
                }
                if (y - 1 > 0)
                {
                    color = bt.GetPixel(x, y - 1);
                    p.X = x;
                    p.Y = y - 1;
                    if (GetBrightness(color) > 150 && !path.Contains(p))
                        path.Add(new Point(x, y - 1));

                    if (x + 1 < bt.Width)
                    {
                        color = bt.GetPixel(x + 1, y - 1);
                        p.X = x + 1;
                        p.Y = y - 1;
                        if (GetBrightness(color) > 150 && !path.Contains(p))
                            path.Add(new Point(x + 1, y - 1));
                    }

                    if (x - 1 > 0)
                    {
                        color = bt.GetPixel(x - 1, y - 1);
                        p.X = x - 1;
                        p.Y = y - 1;
                        if (GetBrightness(color) > 150 && !path.Contains(p))
                            path.Add(new Point(x - 1, y - 1));
                    }
                }
                if (x + 1 < bt.Width)
                {
                    color = bt.GetPixel(x + 1, y);
                    p.X = x + 1;
                    p.Y = y;
                    if (GetBrightness(color) > 150 && !path.Contains(p))
                        path.Add(new Point(x + 1, y));
                }
                if (x - 1 > 0)
                {
                    color = bt.GetPixel(x - 1, y);
                    p.X = x - 1;
                    p.Y = y;
                    if (GetBrightness(color) > 150 && !path.Contains(p))
                        path.Add(new Point(x - 1, y));
                }
                path.RemoveAt(0);
            }
            #endregion 

            Bitmap b = new Bitmap(right - left + 5, top - bottom + 5);
            using (Graphics g = Graphics.FromImage(b))
            {
                g.FillRectangle(Brushes.White, 0, 0, b.Width, b.Height);
            }
            foreach (var point in symbol_pixels)
                b.SetPixel(point.X - left + 2, point.Y - bottom + 2, Color.Black);

            return b;
        }
    }
}

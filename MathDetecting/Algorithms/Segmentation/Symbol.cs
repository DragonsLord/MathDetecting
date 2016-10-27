using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MathDetecting.Algorithms.Segmentation
{
    public enum MergeDirection { Up, Down, Left, Right };

    public sealed class Symbol :IDisposable
    {
        private Bitmap _image;
        public Bitmap Image { get { return _image; } }

        public bool ImageDisposed { get; private set; } 

        private char _unicode_char;
        public char UnicodeCharacter { get { return _unicode_char; } }

        private int _x;
        public int X { get { return _x; } }
        private int _y;
        public int Y { get { return _y; } }

        private Size _original_size;
        public Size OriginalSize { get { return _original_size; } }

        public Symbol(Bitmap image, int x, int y, int width, int height)
        {
            _x = x;
            _y = y;

            _original_size.Width = width;
            _original_size.Height = height;

            _image = image;

            ImageDisposed = false;    
        }

        public void Recognize()
        {
            _image = FormatImage.ResizeImage(_image, 50, 50);
            FormatImage.ChangeImage(_image);
            _unicode_char = Algorithms.Recognition.NeuralNetwork.Recognize(_image);
        }

        public static Symbol Merge(Symbol s1, Symbol s2, MergeDirection direction)
        {
            if (s1.ImageDisposed || s2.ImageDisposed)
                throw new ObjectDisposedException("Bitmap", "Symbol image already disposed");

            Bitmap merged_image;
            int width = 0, height = 0;
            switch (direction)
            {
                case MergeDirection.Up:
                case MergeDirection.Down: { 
                    width = Math.Max(s1.OriginalSize.Width, s2.OriginalSize.Width) + 4;
                    height = s1.OriginalSize.Height + s2.OriginalSize.Height + 12;
                    break;
                }
                case MergeDirection.Left:
                case MergeDirection.Right: {
                    height = Math.Max(s1.OriginalSize.Height, s2.OriginalSize.Height) + 4;
                    width = s1.OriginalSize.Width + s2.OriginalSize.Width + 12;
                    break;
                }
            }
            merged_image = new Bitmap(width, height);

            switch (direction)
            {
                case MergeDirection.Up: {
                    using (Graphics g = Graphics.FromImage(merged_image))
                    {
                        g.DrawImageUnscaled(s2.Image, (width / 2) - (s2.OriginalSize.Width / 2) - 2, 0);
                        g.DrawImageUnscaled(s1.Image, 0, s2.OriginalSize.Height + 8);
                        break;
                    }
                }
                case MergeDirection.Down: {
                    using (Graphics g = Graphics.FromImage(merged_image))
                    {
                        g.DrawImageUnscaled(s1.Image, 0, 0);
                        g.DrawImageUnscaled(s2.Image, (width / 2) - (s2.OriginalSize.Width / 2) - 2, s2.OriginalSize.Height + 8);
                        break;
                    }
                }
                case MergeDirection.Left: {
                    using (Graphics g = Graphics.FromImage(merged_image))
                    {
                        g.DrawImageUnscaled(s2.Image, 0, height / 2 - s2.OriginalSize.Height / 2);
                        g.DrawImageUnscaled(s1.Image, s2.OriginalSize.Width + 8, 0);
                        break;
                    }
                }
                case MergeDirection.Right: {
                    using (Graphics g = Graphics.FromImage(merged_image))
                    {
                        g.DrawImageUnscaled(s1.Image, 0, 0);
                        g.DrawImageUnscaled(s2.Image, s2.OriginalSize.Width + 8, height / 2 - s2.OriginalSize.Height / 2);
                        break;
                    }
                }
            }
            return new Symbol(merged_image, (s1.X + s2.X) / 2, (s1.Y + s2.Y) / 2, width - 4, height - 4);
        }

        //private bool components = false;

        //public Symbol(Bitmap image, int d1, int d2, int d3, int d4, SymbolPosition position = SymbolPosition.Middle)
        //{
        //    _position[0] = d1;
        //    _position[1] = d2;
        //    _position[2] = d3;
        //    _position[3] = d4;

        //    _main_position = position;

        //    _image = Segmentation.CutText(image);
        //    //_image = FormatImage.ResizeImage(image, 50, 50);
        //    //FormatImage.ChangeImage(_image);

        //    _unicode_char = Algorithms.Recognition.NeuralNetwork.Recognize(_image);
        //}
        //
        //public static List<Symbol> GetSymbols(Bitmap formula)
        //{           
        //    List<Symbol> symbols = new List<Symbol>();
        //    List<Bitmap> s0 = new List<Bitmap>();
        //    Bitmap image = Segmentation.CutText(formula);
        //    {
        //        Segmentation.CleanImage(image);
        //        s0.AddRange(Segmentation.GetSymbolsByColumns(image, false));
        //    }
        //    SymbolPosition[] positions = new SymbolPosition[s0.Count];
        //    for (int i = 0; i < s0.Count; i++)
        //    {
        //        Bitmap temp = s0[i];
        //        positions[i] = Segmentation.CutAndGetPosition(ref temp);
        //        s0[i] = temp;
        //    }
        //    for (int i = 0; i < s0.Count; i++)
        //    {
        //        List<Bitmap> s1 = Segmentation.GetSymbolsByRows(s0[i]);
        //        if (s1.Count == 2)
        //        {
        //            symbols.Add(new Symbol(s0[i], i, 0, 0, 0, positions[i]) { components = true });
        //            continue;
        //        }
        //        for (int j = 0; j < s1.Count; j++)
        //        {
        //            List<Bitmap> s2 = Segmentation.GetSymbolsByColumns(s1[j]);
        //            if (s2.Count == 2)
        //            {
        //                symbols.Add(new Symbol(s1[j], i, j, 0, 0, positions[i]) { components = true });
        //                continue;
        //            }
        //            for (int k = 0; k < s2.Count; k++)
        //            {
        //                Segmentation.CleanImage(s2[k]);
        //                List<Bitmap> s3 = Segmentation.GetSymbolsByRows(s2[k]);
        //                if (s3.Count == 2)
        //                {
        //                    symbols.Add(new Symbol(s2[k], i, j, k, 0, positions[i]) { components = true });
        //                    continue;
        //                }
        //                for (int s = 0; s < s3.Count; s++)
        //                    symbols.Add(new Symbol(s3[s], i, j, k, s, positions[i]));
        //                s3.Clear();
        //            }
        //            s2.Clear();
        //        }
        //        s1.Clear();
        //    }

        //    s0.Clear();
        //    for (int i = 0; i < symbols.Count; i++)
        //    {
        //        if (!symbols[i].components)
        //        {
        //            Symbol temp = symbols[i];
        //            Bitmap b = (Bitmap)temp.Image.Clone();
        //            s0 = Segmentation.BruteSegmentation(b);
        //            if (s0.Count > 1)
        //            {
        //                symbols.RemoveAt(i);
        //                for (int d = 0; d < s0.Count; d++)
        //                {
        //                    symbols.Insert(i + d, new Symbol(s0[d], temp.GetPosition(0), temp.GetPosition(1), temp.GetPosition(2), d, temp.MainPosition));
        //                }
        //                i += s0.Count - 1;
        //            }
        //            s0.Clear();
        //        }
        //    }

        //    return symbols;
        //}

        public void Dispose()
        {
            ImageDisposed = true;
            _image.Dispose();
        }
    }
}

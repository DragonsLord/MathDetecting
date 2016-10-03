using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MathDetecting.Algorithms.Segmentation
{
    public enum SymbolPosition { Bottom, Middle, Top, None };

    public sealed class Symbol :IDisposable
    {
        private Bitmap _image;
        public Bitmap Image { get { return _image; } }

        private int[] _position = new int[4];
        public int GetPosition(int depth)
        {
            if (depth >= 0 && depth < 4)
                return _position[depth];
            else throw new IndexOutOfRangeException("Depth should be positive and less then 4");
        }

        private SymbolPosition _main_position;
        public SymbolPosition MainPosition
        {
            get { return _main_position; }
        }

        private char _unicode_char;
        public char UnicodeCharacter { get { return _unicode_char; } }

        private bool components = false;

        public Symbol(Bitmap image, int d1, int d2, int d3, int d4, SymbolPosition position = SymbolPosition.Middle)
        {
            _position[0] = d1;
            _position[1] = d2;
            _position[2] = d3;
            _position[3] = d4;

            _main_position = position;

            _image = Segmentation.CutText(image);
            _image = FormatImage.ResizeImage(image, 50, 50);
            FormatImage.ChangeImage(_image);

            _unicode_char = Algorithms.Recognition.NeuralNetwork.Recognize(_image);
        }

        public static List<Symbol> GetSymbols(Bitmap formula)
        {           
            List<Symbol> symbols = new List<Symbol>();
            List<Bitmap> s0 = new List<Bitmap>();
            Bitmap image = Segmentation.CutText(formula);
            {
                Segmentation.CleanImage(image);
                s0.AddRange(Segmentation.GetSymbolsByColumns(image, false));
            }
            SymbolPosition[] positions = new SymbolPosition[s0.Count];
            for (int i = 0; i < s0.Count; i++)
            {
                Bitmap temp = s0[i];
                positions[i] = Segmentation.CutAndGetPosition(ref temp);
                s0[i] = temp;
            }
            for (int i = 0; i < s0.Count; i++)
            {
                List<Bitmap> s1 = Segmentation.GetSymbolsByRows(s0[i]);
                if (s1.Count == 2)
                {
                    symbols.Add(new Symbol(s0[i], i, 0, 0, 0, positions[i]) { components = true });
                    continue;
                }
                for (int j = 0; j < s1.Count; j++)
                {
                    List<Bitmap> s2 = Segmentation.GetSymbolsByColumns(s1[j]);
                    if (s2.Count == 2)
                    {
                        symbols.Add(new Symbol(s1[j], i, j, 0, 0, positions[i]) { components = true });
                        continue;
                    }
                    for (int k = 0; k < s2.Count; k++)
                    {
                        Segmentation.CleanImage(s2[k]);
                        List<Bitmap> s3 = Segmentation.GetSymbolsByRows(s2[k]);
                        if (s3.Count == 2)
                        {
                            symbols.Add(new Symbol(s2[k], i, j, k, 0, positions[i]) { components = true });
                            continue;
                        }
                        for (int s = 0; s < s3.Count; s++)
                            symbols.Add(new Symbol(s3[s], i, j, k, s, positions[i]));
                        s3.Clear();
                    }
                    s2.Clear();
                }
                s1.Clear();
            }

            s0.Clear();
            for (int i = 0; i < symbols.Count; i++)
            {
                if (!symbols[i].components)
                {
                    Symbol temp = symbols[i];
                    Bitmap b = (Bitmap)temp.Image.Clone();
                    s0 = Segmentation.BruteSegmentation(b);
                    if (s0.Count > 1)
                    {
                        symbols.RemoveAt(i);
                        for (int d = 0; d < s0.Count; d++)
                        {
                            symbols.Insert(i + d, new Symbol(s0[d], temp.GetPosition(0), temp.GetPosition(1), temp.GetPosition(2), d, temp.MainPosition));
                        }
                        i += s0.Count - 1;
                    }
                    s0.Clear();
                }
            }

            return symbols;
        }

        public void Dispose()
        {
            _image.Dispose();
        }
    }
}

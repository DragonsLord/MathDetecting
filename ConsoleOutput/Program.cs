using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;

using MathDetecting.Algorithms.Segmentation;

namespace ConsoleOutput
{
    class Program
    {
        static void Main()
        {
            List<Symbol> symbols = new List<Symbol>();
            Bitmap image = Segmentation.CutText(new Bitmap("formula.jpeg"));
            {
                Segmentation.CleanImage(image);
                symbols = Segmentation.FullBruteSegmentation(image);
            }
            int small = (image.Width * image.Height) / 20000;
            for (int i = 1; i < symbols.Count; i++)
            {
                if (symbols[i].OriginalSize.Width < small && symbols[i].OriginalSize.Height < small)
                {
                    symbols[i - 1] = Symbol.Merge(symbols[i - 1], symbols[i], MergeDirection.Up);
                    symbols.RemoveAt(i);
                }
            }
            foreach (var symbol in symbols)
            {
                symbol.Image.Save(String.Format("Output\\x_{0}y_{1}({2}, {3}).png", symbol.X, symbol.Y, symbol.OriginalSize.Width, symbol.OriginalSize.Height));
            }
            //SymbolRecognition.Test();

            //Console.ReadKey();
        }
    }
}

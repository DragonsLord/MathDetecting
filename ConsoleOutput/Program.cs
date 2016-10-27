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
            Bitmap image = Segmentation.CutText(new Bitmap("input1.jpeg"));
            {
                image = Segmentation.CutText(image);
                Segmentation.CleanImage(image);
                symbols = Segmentation.FullBruteSegmentation(image);
            }

            for (int i = 0; i < symbols.Count; i++)
            {
                symbols[i].Image.Save(String.Format("Output\\{4}--x_{0}y_{1}({2}, {3}).png", symbols[i].X, symbols[i].Y, symbols[i].OriginalSize.Width, symbols[i].OriginalSize.Height, i));
            }

            //Console.ReadKey();
        }
    }
}

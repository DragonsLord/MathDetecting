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
            List<Bitmap> s0 = new List<Bitmap>();
            Bitmap image = Segmentation.CutText(new Bitmap("formula.jpeg"));
            {
                symbols = Symbol.GetSymbols(image);
            }

            foreach (var symbol in symbols)
                symbol.Image.Save(String.Format("Output\\s{0}_{1}_{2}_{3}.png", symbol.GetPosition(0), symbol.GetPosition(1), symbol.GetPosition(2), symbol.GetPosition(3)));
        }
    }
}

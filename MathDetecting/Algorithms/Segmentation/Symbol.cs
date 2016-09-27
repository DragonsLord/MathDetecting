using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MathDetecting.Algorithms.Segmentation
{
    public sealed class Symbol :IDisposable
    {
        private Bitmap _image;
        public Bitmap Image { get { return _image; } }

        private int[] position = new int[4];

        public int GetPosition(int depth)
        {
            if (depth >= 0 && depth < 4)
                return position[depth];
            else throw new IndexOutOfRangeException("Depth should be positive and less then 4");
        }

        public Symbol(Bitmap image, int d1, int d2, int d3, int d4)
        {
            position[0] = d1;
            position[1] = d2;
            position[2] = d3;
            position[3] = d4;
            _image = image;
            Segmentation.CleanImage(_image);
        }

        public void Dispose()
        {
            _image.Dispose();
        }
    }
}

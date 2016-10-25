using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Drawing;
using System.Threading.Tasks;

using MathDetecting.Algorithms.Segmentation;

namespace MathDetecting
{
    public enum TaskClass { }; // Add kinds of tasks
    public class Formula
    {
        private TaskClass _task;
        public TaskClass Task { get { return _task; } }

        private IEnumerable<double> _params;
        public IEnumerable<double> Parametrs { get { return _params; } }

        private Expression<Func<double,IEnumerable<double>>> _expresion;

        private Formula()
        {
        }

        /// <summary>
        /// Recognize Formula from image
        /// </summary>
        /// <param name="image">image</param>
        /// <returns>formula</returns>
        public static Formula FromImage(Bitmap image)
        {
            List<Symbol> symbols = new List<Symbol>();
            using (Bitmap b = Segmentation.CutText(image))
            {
                symbols = Segmentation.FullBruteSegmentation(b);
            }

            //TODO: Build expression

            return null;
        }
    }
}

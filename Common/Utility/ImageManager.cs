using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Common.Utility
{
    public class ImageManager
    {
        public double[] ConvertFromPngToInput(byte[] image, int inputSize)
        {
            var result = new double[inputSize];

            using (var stream = new MemoryStream(image))
            using (var bitmap = new Bitmap(stream))
            {
                var r = bitmap.Width/inputSize;
                for (int i_gl = 0; i_gl < inputSize; i_gl++)
                {
                    for (int i = i_gl * r; i < (i_gl + 1) *r; i++)
                    {
                        for (int j = 0; j < bitmap.Height; j++)
                        {
                            var color = bitmap.GetPixel(i, j);
                            result[i_gl] += color.G - color.B - color.R;
                        }
                    }
                }
                
            }

            return result;
        }

        public double[] Normalize(double[] data)
        {
            double dataMax = data.Max();
            double dataMin = data.Min();
            double range = Math.Abs(dataMax - dataMin);

            return data.Select(d => (d - dataMin) / range).ToArray();
        }
    }
}

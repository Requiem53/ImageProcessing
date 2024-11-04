using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Editor
{
    static class BasicDIP
    {
        public static void Sepia(ref Bitmap a, ref Bitmap b)
        {
            b = new Bitmap(a.Width, a.Height);
            for (int x = 0; x < a.Width; x++)
            {
                for (int y = 0; y < a.Height; y++)
                {
                    Color temp = a.GetPixel(x, y);
                    Color changed;
                    changed = Color.FromArgb(
                        (int)Math.Min((temp.R * .393) + (temp.G * .769) + (temp.B * .189), 255), 
                        (int)Math.Min((temp.R * .349) + (temp.G * .686) + (temp.B * .168), 255), 
                        (int)Math.Min((temp.R * .272) + (temp.G * .534) + (temp.B * .131), 255) );

                    b.SetPixel(x, y, changed);
                }
            }
        }

        public static void Brightness(ref Bitmap a, ref Bitmap b, int value)
        {
            b = new Bitmap(a.Width, a.Height);
            for (int x = 0; x < a.Width; x++)
            {
                for (int y = 0; y < a.Height; y++) 
                {
                    Color temp = a.GetPixel(x, y);
                    Color changed;
                    if (value > 0)
                        changed = Color.FromArgb(Math.Min(temp.R + value, 255), Math.Min(temp.G + value, 255), Math.Min(temp.B + value, 255));
                    else
                        changed = Color.FromArgb(Math.Max(temp.R + value, 0), Math.Max(temp.G + value, 0), Math.Max(temp.B + value, 0));
                
                    b.SetPixel(x, y, changed);
                }
            }
        }

        public static void Histogram(ref Bitmap a, ref Bitmap b)
        {
            Color sample;
            Color gray;
            Byte graydata;
            int[] histogramData = new int[256];
            for (int x = 0; x < a.Width; x++)
            {
                for (int y = 0; y < a.Height; y++)
                {
                    sample = a.GetPixel(x, y);
                    graydata = (byte)((sample.R + sample.G + sample.B) / 3);
                    gray = Color.FromArgb(graydata, graydata, graydata);
                    a.SetPixel(x, y, gray);

                    sample = a.GetPixel(x, y);
                    histogramData[sample.R]++;
                }
            }
           
            b = new Bitmap(256, 800);
            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < Math.Min(histogramData[x]/5, b.Height-1); y++)
                {
                    b.SetPixel(x, (b.Height - 1) - y, Color.Black);
                }
            }
        }
    }
}

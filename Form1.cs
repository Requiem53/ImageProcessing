using WebCamLib;
using ImageProcess2;
using System.Drawing.Imaging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Image_Editor
{
    public partial class Form1 : Form
    {
        Bitmap loaded, processed;
        public Form1()
        {
            InitializeComponent();
        }

        Device[] devices;
        private void Form1_Load(object sender, EventArgs e)
        {
            devices = DeviceManager.GetAllDevices();
        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            loaded = new Bitmap(openFileDialog1.FileName);
            pictureBox1.Image = loaded;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void saveToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            processed.Save(saveFileDialog1.FileName);
        }

        private void pixelCopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded.Width, loaded.Height);
            Color pixel;
            for (int x = 0; x < loaded.Width; x++)
            {
                for (int y = 0; y < loaded.Height; y++)
                {
                    pixel = loaded.GetPixel(x, y);
                    processed.SetPixel(x, y, pixel);
                }
            }
            pictureBox2.Image = processed;
        }

        private void grayscalingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded.Width, loaded.Height);
            Color pixel;
            int average;
            for (int x = 0; x < loaded.Width; x++)
            {
                for (int y = 0; y < loaded.Height; y++)
                {
                    pixel = loaded.GetPixel(x, y);
                    average = (int)(pixel.R + pixel.G + pixel.B) / 3;
                    Color grayscaled = Color.FromArgb(average, average, average);
                    processed.SetPixel(x, y, grayscaled);
                }
            }
            pictureBox2.Image = processed;
        }

        private void inversionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded.Width, loaded.Height);
            Color pixel;
            for (int x = 0; x < loaded.Width; x++)
            {
                for (int y = 0; y < loaded.Height; y++)
                {
                    pixel = loaded.GetPixel(x, y);
                    Color inverted = Color.FromArgb(255 - pixel.R, 255 - pixel.G, 255 - pixel.B);
                    processed.SetPixel(x, y, inverted);
                }
            }
            pictureBox2.Image = processed;
        }

        private void histogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BasicDIP.Histogram(ref loaded, ref processed);
            pictureBox2.Image = processed;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            BasicDIP.Brightness(ref loaded, ref processed, trackBar1.Value);
            pictureBox2.Image = processed;
        }

        private void sepiaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BasicDIP.Sepia(ref loaded, ref processed);
            pictureBox2.Image = processed;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog2.ShowDialog();
        }

        Bitmap greenScreened, background, resultImage;

        private void openFileDialog2_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            greenScreened = new Bitmap(openFileDialog2.FileName);
            loaded = new Bitmap(openFileDialog2.FileName);
            pictureBox1.Image = greenScreened;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog3.ShowDialog();
        }

        private void openFileDialog3_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            background = new Bitmap(openFileDialog3.FileName);
            pictureBox3.Image = background;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            resultImage = new Bitmap(greenScreened.Width, background.Height);

            Color mygreen = Color.FromArgb(0, 0, 255);
            int greygreen = (mygreen.R + mygreen.G + mygreen.B) / 3;
            int threshold = 5;

            for (int x = 0; x < greenScreened.Width; x++)
            {
                for (int y = 0; y < greenScreened.Height; y++)
                {
                    Color pixel = greenScreened.GetPixel(x, y);
                    Color backpixel = background.GetPixel(x, y);
                    int grey = (pixel.R + pixel.G + pixel.B) / 3;
                    int subtractValue = Math.Abs(grey - greygreen);
                    if (subtractValue > threshold)
                        resultImage.SetPixel(x, y, pixel);
                    else
                        resultImage.SetPixel(x, y, backpixel);
                }

            }
            pictureBox2.Image = resultImage;
        }

        private void webcamOnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            devices[0].ShowWindow(pictureBox1);
        }

        private void webcamOffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            devices[0].Stop();
        }

        private void grayscaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            IDataObject data;
            Image bmap;
            devices[0].Sendmessage();
            data = Clipboard.GetDataObject();
            bmap = (Image)data.GetData("System.Drawing.Bitmap", true);
            Bitmap b = new Bitmap(bmap);

            BitmapFilter.GrayScale(b);
            pictureBox2.Image = b;
        }

        private void replaceBackgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer2.Enabled = true;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (processed == null)
                return;

            IDataObject data = Clipboard.GetDataObject();
            Image bmap = (Image)data.GetData("System.Drawing.Bitmap", true);

            Bitmap subtracted;

            if (bmap != null)
            {
                loaded = new Bitmap(bmap);
                subtracted = new Bitmap(loaded.Width, loaded.Height);

                BitmapData bmLoaded = loaded.LockBits(
                    new Rectangle(0, 0, loaded.Width, loaded.Height),
                    ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb
                    );

                BitmapData bmProcessed = processed.LockBits(
                    new Rectangle(0, 0, processed.Width, processed.Height),
                    ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb
                    );

                BitmapData bmSubtracted = subtracted.LockBits(
                    new Rectangle(0, 0, subtracted.Width, subtracted.Height),
                    ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb
                    );

                int limitAve = 255 / 3;
                int threshold = 5;

                unsafe
                {
                    int paddingLoaded = bmLoaded.Stride - loaded.Width * 3;
                    int paddingProcessed = bmProcessed.Stride - processed.Width * 3;
                    int paddingSubtracted = bmSubtracted.Stride - subtracted.Width * 3;

                    byte* pLoaded = (byte*)bmLoaded.Scan0;
                    byte* pProcessed = (byte*)bmProcessed.Scan0;
                    byte* pSubtracted = (byte*)bmSubtracted.Scan0;

                    byte* start_p_processed = (byte*)bmProcessed.Scan0;

                    for (int i = 0;
                        i < loaded.Height;
                        i++, pLoaded += paddingLoaded, pSubtracted += paddingSubtracted)
                    {
                        for (int j = 0;
                            j < loaded.Width;
                            j++, pLoaded += 3, pSubtracted += 3)
                        {
                            if (Math.Abs(pLoaded[0] + pLoaded[1] + pLoaded[2] - limitAve) < threshold)
                            {
                                pSubtracted[0] = pProcessed[0];
                                pSubtracted[1] = pProcessed[1];
                                pSubtracted[2] = pProcessed[2];
                            }
                            else
                            {
                                pSubtracted[0] = pLoaded[0];
                                pSubtracted[1] = pLoaded[1];
                                pSubtracted[2] = pLoaded[2];
                            }

                            if (j < processed.Width)
                                pProcessed += 3;
                        }

                        if (i < processed.Height)
                            pProcessed = start_p_processed + i * 3;
                    }
                }

                loaded.UnlockBits(bmLoaded);
                processed.UnlockBits(bmProcessed);
                subtracted.UnlockBits(bmSubtracted);

                pictureBox3.Image = subtracted;
            }
        }
    }
}

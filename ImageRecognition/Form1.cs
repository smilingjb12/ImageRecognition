using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace ImageRecognition
{
    public partial class Form1 : Form
    {
        private string _image;

        public Form1()
        {
            InitializeComponent();
            this.CenterToScreen();

            this.DragEnter += (sender, e) =>
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop, autoConvert: false))
                {
                    e.Effect = DragDropEffects.All;
                }
            };

            this.DragDrop += (sender, e) =>
            {
                string[] fileList = e.Data.GetData(DataFormats.FileDrop) as string[];
                string bitmapName = fileList[0];
                _image = bitmapName;
                var bitmap = new Bitmap(bitmapName);
                picBoxOriginal.Image = bitmap;

                this.Parse(bitmap, int.Parse(txtThreshold.Text), int.Parse(txtClusters.Text));
            };


        }

        private void Parse(Bitmap pic, int threshold, int clusters)
        {
            lblTimes.Text = (int.Parse(lblTimes.Text) + 1).ToString();

            Bitmap negative = null;
            if (checkBox1.Checked)
            {
                negative = BitmapParser.Negative(new Bitmap(_image));
            }

            Bitmap thresholded = null;
            if (checkBox1.Checked)
            {
                thresholded = BitmapParser.DiscolorAndThreshold(negative, threshold);
            }
            else
            {
                thresholded = BitmapParser.DiscolorAndThreshold(new Bitmap(_image), threshold);
            }
            //picBoxThresholded.Image = thresholded;

            int[,] pixels = BitmapParser.ToBinary(thresholded);
            //this.DisplayPixels(pixels, "Pixels");

            int[,] labels = null;
            Dictionary<int, List<Point>> pointsByLabel = null;
            BitmapParser.ToLabels(pixels, out labels, out pointsByLabel);
            //this.DisplayPixels(labels, "Labels");
            //Bitmap labeled = (Bitmap)thresholded.Clone();
            //for (int y = 0; y < labeled.Height; ++y)
            //{
            //    for (int x = 0; x < labeled.Width; ++x)
            //    {
            //        if (labels[y, x] != -1)
            //        {
            //            labeled.SetPixel(x, y, GetColor(labels[y, x]));
            //        }
            //    }
            //}
            //picBoxLabeled.Image = labeled;

            List<ImageInfo> images = BitmapParser.CalculateStats(pixels, pointsByLabel);
            DisplayStats(images);

            Recognizer.Parse(images, clusters: clusters);

            Bitmap recognized = (Bitmap)thresholded.Clone();
            for (int y = 0; y < thresholded.Height; ++y)
            {
                for (int x = 0; x < thresholded.Width; ++x)
                {
                    int label = labels[y, x];
                    if (label != -1)
                    {
                        int cluster = images.Where(im => im.Label == label).Single().Cluster;
                        if (cluster == -1) Debugger.Break();
                        recognized.SetPixel(x, y, GetColor(cluster));
                    }
                }
            }
            picBoxRecognized.Image = recognized;
        }

        private List<Color> _colors = new List<Color>() {
            Color.Crimson, Color.Indigo, Color.CornflowerBlue,
            Color.Moccasin, Color.Violet, Color.Olive,
            Color.LightBlue, Color.DarkOrchid, Color.Orange,
            Color.OldLace, Color.ForestGreen, Color.Linen,
            Color.Navy, Color.Plum, Color.Purple,
            Color.RoyalBlue, Color.Salmon
        };
        private Color GetColor(int index)
        {
            if (index >= _colors.Count)
            {
                var rand = new Random(DateTime.Now.Millisecond);
                while (_colors.Count <= index)
                {
                    var color = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256), rand.Next(256));
                    if (!_colors.Contains(color)) _colors.Add(color);
                }
            }
            return _colors[index];
        }

        private Form _statsForm = null;
        private void DisplayStats(List<ImageInfo> images)
        {
            if (_statsForm != null && _statsForm.Visible) return;
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("{0,20}{1,20}{2,20}{3,20}{4,20}{5,20}",
                "Label", "Area", "Perimeter", "Compactness", "Elongation", "Orientation"));
            images.ForEach(im =>
            {
                sb.AppendLine(string.Format("{0,20}{1,20}{2,20}{3,20}{4,20}{5,20}",
                    im.Label, im.Area, im.Perimeter, im.Compactness.ToString("#.##"), im.Elongation.ToString("#.##"), im.Orientation.ToString("#.##")));
            });
            var text = new RichTextBox();
            text.Text = sb.ToString();
            text.Dock = DockStyle.Fill;
            text.Font = new Font("Courier New", 10.0f);

            _statsForm = new Form();
            _statsForm.Text = "Stats";
            _statsForm.Size = new Size(1024, 400);
            _statsForm.Controls.Add(text);
            _statsForm.Show();

            this.Focus();
        }

        private void DisplayPixels(int[,] bitmap, string title)
        {
            var sb = new StringBuilder();

            for (int y = 0; y < bitmap.GetLength(0); ++y)
            {
                for (int x = 0; x < bitmap.GetLength(1); ++x)
                {
                    sb.Append(bitmap[y, x] == -1 ? "-" : bitmap[y, x] == 0 ? "#" : bitmap[y, x].ToString());
                    //Debug.Write(bitmap[y, x]);
                }
                sb.Append(Environment.NewLine);
            }

            var textBox = new RichTextBox();
            textBox.Text = sb.ToString();
            textBox.Font = new Font("Courier New", 6.0f);
            textBox.Dock = DockStyle.Fill;

            var f = new Form();
            f.Text = title;
            f.Size = new Size(1024, 600);
            f.Controls.Add(textBox);
            f.Show();

            this.Focus();
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            this.Parse(new Bitmap(_image), int.Parse(txtThreshold.Text), int.Parse(txtClusters.Text));
        }

        private void checkBox1_CheckStateChanged(object sender, EventArgs e)
        {

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < Application.OpenForms.Count; ++i)
            {
                if (Application.OpenForms[i] != this)
                {
                    Application.OpenForms[i].Close();
                }
            }
        }
    }
}

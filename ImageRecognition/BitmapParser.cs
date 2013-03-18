using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;


namespace ImageRecognition
{
    public static class BitmapParser
    {
        public static Bitmap Negative(Bitmap pic)
        {
            for (int y = 0; y < pic.Height; ++y)
            {
                for (int x = 0; x < pic.Width; ++x)
                {
                    Color old = pic.GetPixel(x, y);
                    pic.SetPixel(x, y, Color.FromArgb(1, 255 - old.R, 255 - old.G, 255 - old.B));
                }
            }

            return pic;
        }

        public static void ToLabels(int[,] pic,
            out int[,] labels, out Dictionary<int, List<Point>> pointsByLabel)
        {

            int height = pic.GetLength(0);
            int width = pic.GetLength(1);
            labels = new int[height, width];

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    labels[y, x] = pic[y, x];
                }
            }

            int regionListSize = 0;
            pointsByLabel = new Dictionary<int, List<Point>>();
            for (int j = 0; j < height; ++j)
            {
                for (int i = 0; i < width; ++i)
                {
                    if (labels[j, i] == 0)
                    { 
                        regionListSize += 1;
                        labels[j, i] = regionListSize; // start point to label connected points
                        Point point = new Point(i, j);
                        int current = pic[j, i];
                        pointsByLabel[regionListSize] = new List<Point>();
                        pointsByLabel[regionListSize].Add(point);
                        var stack = new Stack<Point>();
                        stack.Push(point);
                        while (stack.Any())
                        {
                            point = stack.Pop();
                            var points = new Point[] { 
                                new Point { X = point.X, Y = point.Y - 1 },
                                new Point { X = point.X, Y = point.Y + 1 },
                                new Point { X = point.X - 1, Y = point.Y },
                                new Point { X = point.X + 1, Y = point.Y }
                            };

                            foreach (var p in points)
                            {
                                if (p.X >= width || p.X < 0 || p.Y >= height || p.Y < 0) continue;
                                if (pic[p.Y, p.X] == current && labels[p.Y, p.X] == 0)
                                { // if 0 and not already labeled
                                    labels[p.Y, p.X] = regionListSize;
                                    point = new Point(p.X, p.Y);
                                    pointsByLabel[regionListSize].Add(point);
                                    stack.Push(point);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static int[,] ToBinary(Bitmap pic)
        {
            int[,] matrix = new int[pic.Height, pic.Width];

            for (int y = 0; y < pic.Height; ++y)
            {
                for (int x = 0; x < pic.Width; ++x)
                {
                    matrix[y, x] = pic.GetPixel(x, y).R > 0 ? -1 : 0; // white color means nothing (-1)
                }
            }

            return matrix;
        }

        public static Bitmap DiscolorAndThreshold(Bitmap pic, int limit)
        {
            for (int y = 0; y < pic.Height; ++y)
            {
                for (int x = 0; x < pic.Width; ++x)
                {
                    var color = pic.GetPixel(x, y);
                    int @new = (int)(0.3 * color.R + 0.59 * color.G + 0.11 * color.B);
                    @new = @new < limit ? 0 : 255;
                    //Debug.WriteLine("Old color: {0}, new: {1}", color.R, @new);
                    pic.SetPixel(x, y, Color.FromArgb(1, @new, @new, @new));
                }
            }

            return pic;
        }

        public static List<ImageInfo> CalculateStats(int[,] pic, Dictionary<int, List<Point>> pointsByLabel)
        {
            Func<double, double> sqrt = Math.Sqrt;
            Func<double, double> square = x => Math.Pow(x, 2);

            int height = pic.GetLength(0);
            int width = pic.GetLength(1);

            var images = new List<ImageInfo>();

            foreach (KeyValuePair<int, List<Point>> imagePoints in pointsByLabel)
            {
                var info = new ImageInfo();
                info.Label = imagePoints.Key;
                List<Point> points = imagePoints.Value;

                foreach (Point p in points)
                {
                    info.Area += 1;
                    if (p.X - 1 < 0 || p.X + 1 >= width || p.Y - 1 < 0 || p.Y + 1 >= height)
                    { // border
                        info.Perimeter += 1;
                    }
                    else if (pic[p.Y, p.X + 1] == -1 || pic[p.Y, p.X - 1] == -1 ||
                             pic[p.Y + 1, p.X] == -1 || pic[p.Y - 1, p.X] == -1)
                    {
                        info.Perimeter += 1;
                    }
                }

                Debug.WriteLine("Area: {0}", info.Area);
                Debug.WriteLine("Perimeter: {0}", info.Perimeter);
                info.Compactness = square(info.Perimeter) / info.Area;
                Debug.WriteLine("Compactness: {0}", info.Compactness);

                info.XAverage = points.Sum(p => p.X) / info.Area;
                info.YAverage = points.Sum(p => p.Y) / info.Area;

                Debug.WriteLine("XAverage: {0}", info.XAverage);
                Debug.WriteLine("YAverage: {0}", info.YAverage);

                info.M20 = 0;
                info.M11 = 0;
                info.M02 = 0;
                foreach (Point p in points)
                {
                    info.M20 += (p.X - info.XAverage) * (p.X - info.XAverage);
                    info.M11 += (p.X - info.XAverage) * (p.Y - info.YAverage);
                    info.M02 += (p.Y - info.YAverage) * (p.Y - info.YAverage);
                }

                info.Elongation = (info.M20 + info.M02 + sqrt(square(info.M20 - info.M02) + 4 * info.M11 * info.M11))
                    / (info.M20 + info.M02 - sqrt(square(info.M20 - info.M02) + 4 * info.M11 * info.M11));

                Debug.WriteLine("Elongation: {0}", info.Elongation);

                Debug.WriteLine("M11: " + info.M11);
                Debug.WriteLine("M20: " + info.M20);
                Debug.WriteLine("M02: " + info.M02);
                info.Orientation = 0.5 * Math.Atan2(2 * info.M11, info.M20 - info.M02);
                Debug.WriteLine("Orientation: " + info.Orientation);

                images.Add(info);
            }

            foreach (ImageInfo image in images)
            {
                if (double.IsInfinity(image.Elongation))
                {
                    Debugger.Break();
                    image.Elongation = images
                        .Where(im => !double.IsInfinity(im.Elongation))
                        .Max(im => im.Elongation);
                }
                else if (double.IsNaN(image.Elongation))
                {
                    Debugger.Break();
                }
            }

            return images;
        }
    }
}

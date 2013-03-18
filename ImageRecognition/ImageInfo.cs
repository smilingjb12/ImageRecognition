using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageRecognition
{
    public class ImageInfo
    {
        public int Label { get; set; }

        public double Area { get; set; }
        public double Perimeter { get; set; }
        public double Compactness { get; set; }
        public double Elongation { get; set; }
        public double Orientation { get; set; }
        public double M20 { get; set; }
        public double M11 { get; set; }
        public double M02 { get; set; }

        public double XAverage { get; set; }
        public double YAverage { get; set; }

        public int Cluster { get; set; }

        public double GetPropertyByIndex(int index)
        {
            switch (index)
            {
                case 0: return Area;
                case 1: return Perimeter;
                case 2: return Compactness;
                case 3: return Elongation;
                case 4: return Orientation;
            }

            System.Diagnostics.Debugger.Break();
            return -1;
        }

        public string ToSpecialString()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < PropertiesCount; ++i)
            {
                sb.AppendFormat("Property #{0} = {1}\n", i, GetPropertyByIndex(i));
            }
            return sb.ToString();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Label: " + Label);
            sb.AppendLine("Area: " + Area);
            sb.AppendLine("Perimeter: " + Perimeter);
            sb.AppendLine("Compactness: " + Compactness);
            sb.AppendLine("Elongation: " + Elongation);
            sb.AppendLine("Orientation: " + Orientation);

            return sb.ToString();
        }

        public const int PropertiesCount = 5;
    }
}

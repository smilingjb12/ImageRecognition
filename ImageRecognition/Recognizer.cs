using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

namespace ImageRecognition
{
    public static class CentroidExtensions
    {
        public static double DistanceTo(this Recognizer.Centroid first, Recognizer.Centroid second)
        {
            double distance = 0.0;
            for (int i = 0; i < ImageInfo.PropertiesCount; ++i)
            {
                distance += Math.Pow(first.PropertyValues[i] - second.PropertyValues[i], 2);
            }
            
            distance = Math.Sqrt(distance);
            return distance;
        }

        public static double DistanceTo(this Recognizer.Centroid first, ImageInfo info)
        {
            double distance = 0.0;
            for (int i = 0; i < ImageInfo.PropertiesCount; ++i)
            {
                distance += Math.Pow(first.PropertyValues[i] - info.GetPropertyByIndex(i), 2);
            }
            distance = Math.Sqrt(distance);
            return distance;
        }
    }

    public static class Recognizer
    {
        private static Random random = new Random(DateTime.Now.Millisecond);

        public class Centroid
        {
            public double[] PropertyValues { get; set; }
            public int Cluster { get; set; }

            public Centroid()
            {
                PropertyValues = new double[ImageInfo.PropertiesCount];
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                for (int i = 0; i < PropertyValues.Length; ++i)
                {
                    string line = string.Format("Property #{0} = {1}", i, PropertyValues[i]);
                    sb.AppendLine(line);
                }
                return sb.ToString();
            }
        }

        private class MinMax
        {
            public double Max { get; set; }
            public double Min { get; set; }
        }

        private static MinMax[] GeneratePropertyMinMaxes(List<ImageInfo> images)
        {
            var minMaxes = new List<MinMax>();
            for (int i = 0; i < ImageInfo.PropertiesCount; ++i)
            {
                double min = images.Select(im => im.GetPropertyByIndex(i)).Min();
                double max = images.Select(im => im.GetPropertyByIndex(i)).Max();
                minMaxes.Add(new MinMax { Min = min, Max = max });
            }

            return minMaxes.ToArray();
        }

        private static Centroid GenerateRandomCentroidWithinRange(MinMax[] ranges)
        {
            Centroid centroid = new Centroid();
            Random rand = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < ImageInfo.PropertiesCount; ++i)
            {
                MinMax minMax = ranges[i];
                centroid.PropertyValues[i] = rand.NextDouble() * (minMax.Max - minMax.Min) + minMax.Min;
            }
            return centroid;
        }

        private static Centroid GenerateRandomCentroid(List<ImageInfo> images)
        {
            Centroid centroid = new Centroid();
            int randomIndex = new Random(DateTime.Now.Millisecond).Next(images.Count);
            ImageInfo randomImage = images[randomIndex];
            for (int i = 0; i < ImageInfo.PropertiesCount; ++i)
            {
                centroid.PropertyValues[i] = randomImage.GetPropertyByIndex(i);
            }
            return centroid;
        }

        private static int FindIndexOfNearestClusterMean(ImageInfo image, Centroid[] means)
        {
            Debug.WriteLine("Image's nearest mean:");
            int nearestMeanIndex = -1;
            double minDistance = double.MaxValue;
            for (int i = 0; i < means.Length; ++i)
            {
                double distanceToMean = means[i].DistanceTo(image);
                Debug.WriteLine("Distance to {0} = {1}", i, distanceToMean);
                if (distanceToMean < minDistance)
                {
                    nearestMeanIndex = i;
                    minDistance = distanceToMean;
                }
            }
            if (nearestMeanIndex == -1) Debugger.Break();
            Debug.WriteLine("Nearest mean index: {0}", nearestMeanIndex);
            return nearestMeanIndex;
        }

        private static double[] CalculateClusterMean2(int clusterIndex, List<ImageInfo> images)
        {
            ImageInfo[] imagesForCluster = images.Where(im => im.Cluster == clusterIndex).ToArray();
            int len = imagesForCluster.Length;
            if (len == 0) return null;
            double[] propertyMeans = new double[ImageInfo.PropertiesCount];
            for (int i = 0; i < ImageInfo.PropertiesCount; ++i)
            {
                double[] propertyValues = imagesForCluster.Select(im => im.GetPropertyByIndex(i)).ToArray();
                propertyMeans[i] = propertyValues.Sum() / (double)propertyValues.Length;
            }

            return propertyMeans;
        }

        private static double[] CalculateClusterMean(int clusterIndex, List<ImageInfo> images)
        {
            ImageInfo[] imagesForCluster = images.Where(im => im.Cluster == clusterIndex).ToArray();
            int len = imagesForCluster.Count();

            Debug.WriteLine("Images for cluster {0}", clusterIndex);
            imagesForCluster
                .Select((im, i) => new { im, i })
                .ToList()
                .ForEach(el =>
                    {
                        Debug.WriteLine("Image #{0}", el.i);
                        Debug.WriteLine(el.im.ToSpecialString());
                    });

            if (len == 0) return null; // centroid has no game

            List<double[]> sortedPropertyValues = new List<double[]>();
            for (int i = 0; i < ImageInfo.PropertiesCount; ++i)
            {
                double[] propertyValues = imagesForCluster.Select(im => im.GetPropertyByIndex(i))
                    .OrderBy(x => x)
                    .ToArray();
                sortedPropertyValues.Add(propertyValues);
            }
            double[] propertyMedians = new double[ImageInfo.PropertiesCount];
            if (len == 1)
            {
                for (int i = 0; i < ImageInfo.PropertiesCount; ++i)
                {
                    propertyMedians[i] = imagesForCluster[0].GetPropertyByIndex(i);
                }
            }
            else if (len % 2 == 0) // even count
            {
                for (int i = 0; i < ImageInfo.PropertiesCount; ++i)
                {
                    double[] sortedValues = sortedPropertyValues[i];
                    propertyMedians[i] = (sortedValues[(len / 2) - 1] + sortedValues[len / 2]) / 2;
                }
            }
            else if (len % 2 != 0) // odd count
            {
                for (int i = 0; i < ImageInfo.PropertiesCount; ++i)
                {
                    double[] sortedValues = sortedPropertyValues[i];
                    propertyMedians[i] = sortedValues[len / 2];
                }
            }

            Debug.WriteLine("Propety medians:");
            Debug.WriteLine(propertyMedians.Select(x => x.ToString()).Aggregate((acc, x) => acc + ", " + x));
            return propertyMedians;
        }

        public static void Parse(List<ImageInfo> images, int clusters)
        {
            MinMax[] propertyRanges = GeneratePropertyMinMaxes(images);

            Centroid[] clusterMeans = new Centroid[clusters];
            for (int i = 0; i < clusters; ++i)
            {
                //clusterMeans[i] = GenerateRandomCentroid(images);
                clusterMeans[i] = GenerateRandomCentroidWithinRange(propertyRanges);
            }

            int iteration = 1;
            while (true)
            {
                Centroid[] prevClusterMeans = new Centroid[clusters];
                for (int i = 0; i < clusterMeans.Length; ++i)
                {
                    prevClusterMeans[i] = new Centroid();
                    Centroid oldMean = prevClusterMeans[i];
                    clusterMeans[i].PropertyValues.CopyTo(oldMean.PropertyValues, 0);
                }

                foreach (ImageInfo image in images)
                {
                    int oldCluster = image.Cluster;
                    image.Cluster = FindIndexOfNearestClusterMean(image, clusterMeans);
                    Debug.WriteLine("Image was {0} now {1} cluster", oldCluster, image.Cluster);
                }

                for (int i = 0; i < clusterMeans.Length; ++i)
                {
                    double[] propertyMedians = CalculateClusterMean(clusterIndex: i, images: images);
                    if (propertyMedians == null) // has no images
                    {
                        Debug.WriteLine("NO GAME FOR {0}", i);
                        //clusterMeans[i] = GenerateRandomCentroid(images);
                        clusterMeans[i] = GenerateRandomCentroidWithinRange(propertyRanges);
                        continue;
                    }
                    clusterMeans[i].PropertyValues = propertyMedians;
                }

                bool pointsAreStill = true;
                for (int i = 0; i < clusters; ++i)
                {
                    double distance = prevClusterMeans[i].DistanceTo(clusterMeans[i]);
                    if (distance > double.Epsilon)
                    {
                        pointsAreStill = false;
                        break;
                    }
                }

                Debug.WriteLine("Iteration: {0}", iteration++);
                if (pointsAreStill) break;
            }
        }
    }
}

using LiveCharts.Wpf;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ModBusTPU.Models.Data;
using ModBusTPU.ViewModels;
using LiveCharts.Defaults;
using System.Collections.ObjectModel;
using System.Runtime.Remoting.Channels;
using LiveCharts;

namespace ModBusTPU.Models.ModbusSensor
{
    //Класс для работы с "окном"

    /* 
     * Так как бесплатная версия библиотеки LiveCharts очень плохо оптимизирована для часто обнавляющихся данных
     * программа отрисовывает только часть массива данных. Это и называется "окном"
    */

    public static class WindowingDataHelper
    {
        //Находит первый индекс в массиве данных, чья абсцисса(в данном случае время),
        //практически совпдает с заданной
        private static int BinFind(double time, Collection<Point> channelData)
        {
            int left = 0, right = channelData.Count - 1;

            while (left <= right)
            {
                int middle = (right + left) / 2;

                if (time > channelData[middle].X)
                    left = middle + 1;
                else
                    right = middle - 1;
            }

            return left;
        }

        //Находит начальный индекс "окна"
        private static int GetStartOfSubarray(double time, Collection<Point> channelData)
        {
            if (time < 0)
                return 0;

            int index = BinFind(time, channelData);

            //"Окно" должно немного выходить за границы отображаемого "окна", чтобы график не обрывался в начале и в конце 
            return index > 0 ? index - 1 : 0;
        }

        //Находит конечный индекс "окна"
        private static int GetEndOfSubarray(double time, Collection<Point> channelData)
        {
            int index = BinFind(time, channelData);

            if (index == channelData.Count)
                return index - 1;

            //"Окно" должно немного выходить за границы отображаемого "окна", чтобы график не обрывался в начале и в конце 
            return index == channelData.Count - 1 ? index : index + 1; 
        }

        public static (int, int) GetDataWindowIndex(double EndTime, Collection<Point> channelData)
        {
            return GetDataWindowIndex(EndTime - Chart.MaxWindowWidth * 1000, EndTime, channelData);
        }

        public static (int, int) GetDataWindowIndex(double StartTime, double EndTime, Collection<Point> channelData)
        {
            int startIndex = GetStartOfSubarray(StartTime, channelData);
            int endIndex = GetEndOfSubarray(EndTime, channelData);
            return (startIndex, endIndex);
        }

        public static ObservablePoint[] GetWindowData(double EndTime, Collection<Point> channelData)
        {
            var Edges = GetDataWindowIndex(EndTime, channelData);
            int leftEdge = Edges.Item1, rightEdge = Edges.Item2;
            return GetWindowData(leftEdge, rightEdge, channelData);
        }

        public static double GetMaxValueOfArray(ObservablePoint[] Points)
        {
            double max = Double.MinValue;

            foreach(var point in Points)
                max = Math.Max(point.Y, max);

            return max;
        }

        public static double GetMinValueOfArray(ObservablePoint[] Points)
        {
            double min = Double.MaxValue;

            foreach (var point in Points)
                min = Math.Max(point.Y, min);

            return min;
        }

        //Получаем подмассив исходных данных - "окно"
        public static ObservablePoint[] GetWindowData(int leftEdge, int rightEdge, Collection<Point> channelData) 
        {
            if (leftEdge < 0 || leftEdge > rightEdge)
                throw new ArgumentOutOfRangeException(nameof(leftEdge));
            if (rightEdge >= channelData.Count)
                throw new ArgumentOutOfRangeException(nameof(rightEdge));

            int size = rightEdge - leftEdge + 1;
            var NewPoints = new ObservablePoint[size];
            int index = 0;
            for (int pointIndex = leftEdge; pointIndex <= rightEdge; pointIndex++)
            {
                var Point = channelData[pointIndex];
                NewPoints[index] = new ObservablePoint(Point.X, Point.Y);
                index++;
            }

            return NewPoints;
        }
    }
}

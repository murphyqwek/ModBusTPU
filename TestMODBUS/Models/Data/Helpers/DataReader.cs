using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMODBUS.Models.Data.Helpers
{
    public static class DataReader
    {
        public static DataStorage ReadLog(string Logs)
        {
            string[] lines = Logs.Split('\n');

            List<ObservableCollection<Point>> ChannelsPoints = new List<ObservableCollection<Point>>();
            Dictionary<string, ObservableCollection<Point>> ExtraDataPoints = new Dictionary<string, ObservableCollection<Point>>();

            int index = 0;
            ChannelsPoints = GetChannelPoints(ref index, lines);
            ExtraDataPoints = GetExtraData(ref  index, lines);

            return new DataStorage(ChannelsPoints, ExtraDataPoints);
        }

        private static Dictionary<string, ObservableCollection<Point>> GetExtraData(ref int index, string[] lines)
        {
            Dictionary<string, ObservableCollection<Point>> ExtraPoints = new Dictionary<string, ObservableCollection<Point>>();

            for (int i = index; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    break;

                var temp = lines[i].Split(' ');
                /*
                if (temp[0] == DataSaver.EXTRADATAPOINT)
                {
                    index++;
                    break;
                }
                */
                var Name = temp[0].Trim().Remove(temp[0].Length - 1);
                ExtraPoints.Add(Name, ParsePoints(temp[1]));
                index++;
            }

            return ExtraPoints;
        }

        private static List<ObservableCollection<Point>> GetChannelPoints(ref int index, string[] lines)
        {
            List<ObservableCollection<Point>> ChannelsPoints = new List<ObservableCollection<Point>>();

            for (int i = index; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    break;

                var temp = lines[i].Split(new char[] { ' ' }, 2);
                if (temp[0] == DataSaver.EXTRADATAPOINT)
                {
                    index++;
                    break;
                }

                ChannelsPoints.Add(ParsePoints(temp[1]));
                index++;
            }

            return ChannelsPoints;
        }

        private static ObservableCollection<Point> ParsePoints(string PointLine)
        {
            var points = new ObservableCollection<Point>();
            PointLine = PointLine.Trim();

            foreach(var Point in PointLine.Split(DataSaver.SEPARATOR))
            {
                var PointCleared = Point;
                PointCleared = PointCleared.Trim();
                PointCleared = PointCleared.Remove(PointCleared.Length - 1, 1);
                PointCleared = PointCleared.Remove(0, 1);
                var PointCoord = PointCleared.Split(',');
                points.Add(new Point(Convert.ToDouble(PointCoord[0]), Convert.ToDouble(PointCoord[1])));
            }

            return points;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TestMODBUS.Models.Data;

namespace TestMODBUS.Models.Services.Settings.Data
{
    public static class DataSaver
    {
        public const char SEPARATOR = ';';
        public const string EXTRADATAPOINT = "EXTRADATA";

        public static string GetDataLog(DataStorage DataStorage)
        {
            string log = "";
            log += GetChannelData(DataStorage);
            log += EXTRADATAPOINT + "\n";
            log += GetExtraData(DataStorage);

            return log;
        }

        private static string GetExtraData(DataStorage DataStorage)
        {
            string outputData = "";

            foreach (var Key in DataStorage.ExtraData.Keys)
                outputData += GetCollectionData(Key, DataStorage.ExtraData[Key]);

            return outputData;
        }

        private static string GetChannelData(DataStorage DataStorage)
        {
            string outputData = "";

            for (int i = 0; i < DataStorage.GetMaxChannelsCount(); i++)
                outputData += GetCollectionData($"CH_{i}", DataStorage.GetChannelData(i));

            return outputData;
        }

        private static string GetCollectionData(string Name, ICollection<Point> Points)
        {
            return Name + ": " + GetCollectionData(Points) + "\n";
        }

        private static string GetCollectionData(ICollection<Point> Points)
        {
            string outputData = "";
            foreach (var point in Points)
            {
                outputData += GetPointData(point) + SEPARATOR;
            }

            if (outputData.Length > 0)
                outputData = outputData.Remove(outputData.Length - 1);

            return outputData;
        }

        private static string GetPointData(Point Point) => $"{{{Point.X}, {Point.Y.ToString().Replace(',', '.')}}}";
    }
}

using LiveCharts.Defaults;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TestMODBUS.Models.Data;
using TestMODBUS.Models.MathModules;
using TestMODBUS.Models.Services;

namespace TestMODBUS.Models.ModbusSensor.ChartDataPrepatations
{
    public class ChartDataPreparationEnergy : ChartDataPrepatationBase
    {
        public override IList<string> GetCurrentValues(IList<int> Channels, DataStorage DataStorage)
        {
            var EnergyPoints = DataStorage.GetExtraData("energy", Channels);

            double value = EnergyPoints.Last().Y;
            value = Math.Round(value, 2);

            IList<string> values = new List<string>
            {
                $"Энергия: {value} кВт"
            };

            return values;
        }

        protected override IList<SerieData> GetPoints(IList<int> ChannelsToUpdate, DataStorage DataStorage, int left, int right)
        {
            IList<SerieData> SeriesToUpdate = new List<SerieData>();

            var EnergyPoints = DataStorage.GetExtraData("energy", ChannelsToUpdate);

            if (EnergyPoints.Count - 1 < right)
            {
                if(!UpdateEnergyPoints(EnergyPoints, DataStorage, ChannelsToUpdate))
                    return SeriesToUpdate;
            }

            var WindowPoints = GetSubArray(EnergyPoints, left, right);

            SerieData serieData = new SerieData();
            serieData.SerieTitle = $"Энергия";
            serieData.Points = ToArray(WindowPoints);

            SeriesToUpdate.Add(serieData);

            return SeriesToUpdate;
        }

        private bool UpdateEnergyPoints(ObservableCollection<Point> EnergyPoints, DataStorage DataStorage, IList<int> Channels)
        {
            if(DataStorage.GetChannelLength() == 0)
                return false;

            var Volt = GetVoltData(DataStorage, Channels);
            if (Volt.Count == 0)
                return false;
            if (EnergyPoints.Count == 0)
            {
                double sumTok = SumTokByIndex(DataStorage, Channels, 0);
                double Energy = EnergyMathModule.CountWithMilliseconds(sumTok, Volt[0].Y, Volt[0].X);
                EnergyPoints.Add(new Point(Volt[0].X, Energy));
            }

            for(int i = EnergyPoints.Count; i < DataStorage.GetChannelLength(); i++)
            {
                double sumTok = SumTokByIndex(DataStorage, Channels, i);
                double Energy = EnergyMathModule.CountWithMilliseconds(sumTok, Volt[i].Y, Volt[i].X - Volt[i - 1].X);
                Energy += EnergyPoints[i - 1].Y;
                EnergyPoints.Add(new Point(Volt[i].X, Energy));
            }

            return true;
        }

        private List<ObservablePoint> GetVoltData(DataStorage DataStorage, IList<int> Channels)
        {
            foreach(var channel in Channels)
            {
                if (ChannelTypeList.VoltChannels.Contains(channel))
                    return ConvertToObservablePoints(DataStorage.GetChannelData(channel));
            }

            return new List<ObservablePoint>();
        }

        private List<ObservablePoint> ConvertToObservablePoints(ObservableCollection<Point> Points)
        {
            var points = new List<ObservablePoint>();

            foreach(var point in Points)
                points.Add(new ObservablePoint(point.X, point.Y));

            return points;
        }

        private double SumTokByIndex(DataStorage DataStorage, IList<int> ChannelsToSum, int index)
        {
            double sumTok = 0;
            foreach (var ChannelNumber in ChannelsToSum)
            {
                if (!ChannelTypeList.TokChannels.Contains(ChannelNumber))
                    continue;

                sumTok += DataStorage.GetChannelData(ChannelNumber)[index].Y;
            }

            return sumTok;
        }

        private List<ObservablePoint> SumTok(DataStorage DataStorage, IList<int> ChannelsToSum)
        {
            List<ObservablePoint> SumTokPoints = new List<ObservablePoint>();
            foreach(var ChannelNumber in ChannelsToSum)
            {
                if (!ChannelTypeList.TokChannels.Contains(ChannelNumber))
                    continue;
                var Channel = DataStorage.GetChannelData(ChannelNumber);
                
                for(int i = 0; i < Channel.Count; i++)
                {
                    if (SumTokPoints.Count == i)
                        SumTokPoints.Add(new ObservablePoint(Channel[0].X, Channel[0].Y));
                    else
                        SumTokPoints[i].Y += Channel[i].Y;
                }
            }

            return SumTokPoints;
        }

        private ObservableCollection<Point> GetSubArray(ObservableCollection<Point> EnergyPoints, int left, int right)
        {
            ObservableCollection<Point> subPoints = new ObservableCollection<Point>();
            for(int i = left; i <= right; i++)
            {
                subPoints.Add(EnergyPoints[i]);
            }

            return subPoints;
        }

        private ObservablePoint[] ToArray(ObservableCollection<Point> EnergyPoints)
        {
            ObservablePoint[] array = new ObservablePoint[EnergyPoints.Count];
            for(int i = 0; i < EnergyPoints.Count; i++)
                array[i] = new ObservablePoint(EnergyPoints[i].X, EnergyPoints[i].Y);
            return array;   
        }
    }
}

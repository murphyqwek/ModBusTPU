using LiveCharts.Defaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ModBusTPU.Models.Data;
using ModBusTPU.Models.Modbus;
using ModBusTPU.Models.Services;
using System.Collections.ObjectModel;

namespace ModBusTPU.Models.ModbusSensor.ChartDataPrepatations
{
    public abstract class ChartDataPreparationBase
    {
        public virtual double GetNewCurrentPosition(DataStorage DataStorage)
        {
            return DataStorage.GetLastTime() / 1000;
        }

        public virtual IList<SerieData> GetNewPoints(IList<int> ChannelsToUpdate, DataStorage DataStorage)
        {
            int LastChannel = ChannelsToUpdate.Last();
            double CurrentTime = DataStorage.GetLastTime();
            var WindowDataEdges = WindowingDataHelper.GetDataWindowIndex(CurrentTime, DataStorage.GetChannelData(LastChannel));
            return GetPoints(ChannelsToUpdate, DataStorage, WindowDataEdges.Item1, WindowDataEdges.Item2);
        }

        public virtual IList<SerieData> GetPointsByCurrentX(IList<int> ChannelsToUpdate, DataStorage DataStorage, double CurrentX)
        {
            CurrentX *= 1000;
            int LastChannel = ChannelsToUpdate.Last();
            (int, int) WindowDataEdges;
            if (DataStorage.GetLastTime() - CurrentX < 5000)
                CurrentX = DataStorage.GetLastTime() - 5000;
            WindowDataEdges = WindowingDataHelper.GetDataWindowIndex(CurrentX, CurrentX + Chart.MaxWindowWidth * 1000, DataStorage.GetChannelData(LastChannel));
            int leftEdge = WindowDataEdges.Item1, rightEdge = WindowDataEdges.Item2;
            return GetPoints(ChannelsToUpdate, DataStorage, leftEdge, rightEdge);
        }

        public virtual IList<SerieData> GetAllPoints(IList<int> Channels, DataStorage DataStorage)
        {
            int left = 0, right = DataStorage.GetChannelLastPointIndex();
            return GetPoints(Channels, DataStorage, left, right);
        }

        protected abstract IList<SerieData> GetPoints(IList<int> ChannelsToUpdate, DataStorage DataStorage, int left, int right);

        public abstract IList<string> GetCurrentValues(IList<int> Channels, DataStorage DataStorage);

        protected ObservablePoint[] ConvertTimeByCoeff(ObservablePoint[] Points, int coeff)
        {
            int PointsLength = Points.Length;
            ObservablePoint[] newPoints = new ObservablePoint[PointsLength];
            for(int i = 0; i < PointsLength; i++)
            {
                var point = Points[i];
                newPoints[i] = new ObservablePoint(point.X / coeff, point.Y);
            }

            return newPoints;
        }

        protected ObservablePoint[] ConvertMillisecondsToSeconds(ObservablePoint[] Points) => ConvertTimeByCoeff(Points, 1000);

        protected ObservablePoint[] Convert(ObservablePoint[] Points, int Channel)
        {
            if (ChannelTypeList.GetChannelType(Channel) == ChannelType.Tok)
            {
                for (int i = 0; i < Points.Length; i++)
                {
                    Points[i].Y = ModBusValueConverter.ConvertToAmperValue(Points[i].Y);
                }
            }
            else if (ChannelTypeList.GetChannelType(Channel) == ChannelType.Volt)
            {
                for (int i = 0; i < Points.Length; i++)
                {
                    Points[i].Y = ModBusValueConverter.ConvertToVoltValue(Points[i].Y);
                }
            }

            return Points;
        }

        protected double GetLastConvertedValue(DataStorage DataStorage, int Channel)
        {
            double Value = DataStorage.GetChannelData(Channel).Last().Y;

            if (ChannelTypeList.GetChannelType(Channel) == ChannelType.Tok)
            {
                Value = ModBusValueConverter.ConvertToAmperValue(Value);
            }
            else if (ChannelTypeList.GetChannelType(Channel) == ChannelType.Volt)
            {
                Value = ModBusValueConverter.ConvertToVoltValue(Value);
            }

            Value = Math.Round(Value, 1);

            return Value;
        }

        public virtual void DeleteExtraData(IList<int> UsingChannels, DataStorage DataStorage) { }
    }
}

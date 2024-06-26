﻿using LiveCharts.Defaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ModBusTPU.Models.Data;
using ModBusTPU.Models.Modbus;
using ModBusTPU.Models.Services;

namespace ModBusTPU.Models.ModbusSensor.ChartDataPrepatations
{
    public abstract class ChartDataPreparationBase
    {
        public virtual double GetNewCurrentPosition(DataStorage DataStorage)
        {
            return DataStorage.GetLastTime();
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
            int LastChannel = ChannelsToUpdate.Last();
            (int, int) WindowDataEdges;
            WindowDataEdges = WindowingDataHelper.GetDataWindowIndex(CurrentX, CurrentX + Chart.MaxWindowWidth, DataStorage.GetChannelData(LastChannel));
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

            return Value;
        }

        public virtual void DeleteExtraData(IList<int> UsingChannels, DataStorage DataStorage) { }
    }
}

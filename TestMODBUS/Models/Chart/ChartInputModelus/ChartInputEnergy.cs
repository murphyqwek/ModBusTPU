using LiveCharts.Defaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TestMODBUS.Models.MathModules;
using TestMODBUS.Models.Services;
using TestMODBUS.ViewModels;

namespace TestMODBUS.Models.Chart.ChartInputModelus
{
    public class ChartInputEnergy : ChartInputModuleBase
    {
        private int _channelTok = -1;
        private int _channelVolt = -1;

        public ChartInputEnergy(ChartModel Chart) : base(Chart)
        {
        }

        public override void RemoveChannel(int Channel)
        {
            if (Channel == _channelTok)
                _channelTok = -1;
            if (Channel == _channelVolt)
                _channelVolt = -1;

            _chart.Channels.Remove(Channel);

            if (_chart.Series.Count == 0)
                _chart.Series.Remove(_chart.Series[0]);
        }

        public override bool AddNewChannel(int Channel)
        {
            bool IsChannelTokOrVolt = false;
            if (ChannelTypeList.TokChannels.Contains(Channel))
            {
                if (_channelTok != -1)
                    _chart.Channels.Remove(_channelTok);
                _channelTok = Channel;
                IsChannelTokOrVolt = true;
            }
            if (ChannelTypeList.VoltChannels.Contains(Channel))
            {
                if (_channelVolt != -1)
                    _chart.Channels.Remove(_channelVolt);
                _channelVolt = Channel;
                IsChannelTokOrVolt = true;
            }

            if (!IsChannelTokOrVolt)
                return false;

            if (_chart.Series.Count == 0)
                _chart.AddNewSerieOnChart(Channel, $"Энергия", new SolidColorBrush(Color.FromRgb(151, 53, 68)));

            _chart.Channels.Add(Channel);

            return true;
        }

        private ObservablePoint[] GetPoints(int LeftIndex, int RightIndex, Data.Data DataStorage)
        {
            if (_channelTok == -1 || _channelVolt == -1)
                return null;
            if(LeftIndex == 0 && RightIndex == 0)
            {
                return null;
            }

            var _TokPoints = WindowingDataHelper.GetWindowData(LeftIndex, RightIndex, DataStorage.GetChannelData(_channelTok));
            var _VoltPoints = WindowingDataHelper.GetWindowData(LeftIndex, RightIndex, DataStorage.GetChannelData(_channelVolt));

            ObservablePoint[] PowerPoints = EnergyMathModule.Apply(_TokPoints.ToList(), _VoltPoints.ToList()).ToArray();

            return PowerPoints;
        }

        public override void UpdateSeries(int LeftIndex, int RightIndex, Data.Data DataStorage)
        {
            var Points = GetPoints(LeftIndex, RightIndex, DataStorage);

            if (Points == null)
                return;

            _chart.Series[0].Values.Clear();
            _chart.Series[0].Values.AddRange(Points);
        }

        public override double GetYMax(int LeftIndex, int RightIndex, Data.Data DataStorage, double YMaxGap)
        {
            var Points = GetPoints(LeftIndex, RightIndex, DataStorage);

            if (Points == null)
                return Double.MinValue;

            return WindowingDataHelper.GetMaxValueOfArray(Points) + YMaxGap;
        }

        public override double GetYMin(int LeftIndex, int RightIndex, Data.Data DataStorage, double YMinGap)
        {
            var Points = GetPoints(LeftIndex, RightIndex, DataStorage);

            if (Points == null)
                return Double.MaxValue;

            return WindowingDataHelper.GetMinValueOfArray(Points) + YMinGap;
        }
    }
}

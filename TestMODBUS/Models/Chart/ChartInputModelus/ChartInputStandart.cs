using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMODBUS.ViewModels;
using TestMODBUS.Models.Services;
using LiveCharts.Defaults;
using TestMODBUS.Models.MathModules;

namespace TestMODBUS.Models.Chart.ChartInputModelus
{
    public class ChartInputStandart : ChartInputModuleBase
    {
        private Dictionary<int, int> ChannelToSeriesDictionary = new Dictionary<int, int>();

        public ChartInputStandart(ChartModel Chart) : base(Chart)
        {
        }

        public override bool AddNewChannel(int Channel)
        {
            _chart.Channels.Add(Channel);
            _chart.AddNewSerieOnChart(Channel, $"CH_{Channel}", ChannelColors.Colors[Channel]);
            ChannelToSeriesDictionary[Channel] = _chart.Series.Count - 1;
            return true;
        }

        public override void RemoveChannel(int Channel)
        {
            _chart.Channels.Remove(Channel);
            var Serie = _chart.Series[ChannelToSeriesDictionary[Channel]];
            _chart.Series.Remove(Serie);
            _chart.SeriesByChannel.Remove(Channel);
            ChannelToSeriesDictionary.Remove(Channel);
        }

        private ObservablePoint[] GetPoints(int LeftIndex, int RightIndex, Data.DataStorage DataStorage, int Channel)
        {
            var ChannelPoints = WindowingDataHelper.GetWindowData(LeftIndex, RightIndex, DataStorage.GetChannelData(Channel));

            return ChannelPoints;
        }

        public override double GetYMax(int LeftIndex, int RightIndex, Data.DataStorage DataStorage, double YMaxGap)
        {
            List<ObservablePoint> Points = new List<ObservablePoint>();
            foreach (int Channel in ChannelToSeriesDictionary.Keys)
            {
                foreach (var Point in GetPoints(LeftIndex, RightIndex, DataStorage, Channel))
                    Points.Add(Point);
            }

            return WindowingDataHelper.GetMaxValueOfArray(Points.ToArray()) + YMaxGap;
        }

        public override double GetYMin(int LeftIndex, int RightIndex, Data.DataStorage DataStorage, double YMinGap)
        {
            List<ObservablePoint> Points = new List<ObservablePoint>();
            foreach (int Channel in ChannelToSeriesDictionary.Keys)
            {
                foreach (var Point in GetPoints(LeftIndex, RightIndex, DataStorage, Channel))
                    Points.Add(Point);
            }

            return WindowingDataHelper.GetMinValueOfArray(Points.ToArray()) + YMinGap;
        }

        public override void UpdateSeries(int LeftIndex, int RightIndex, Data.DataStorage DataStorage)
        {
            foreach (int Channel in ChannelToSeriesDictionary.Keys)
            {
                var Points = GetPoints(LeftIndex, RightIndex, DataStorage, Channel);

                int SerieIndex = ChannelToSeriesDictionary[Channel];

                _chart.Series[SerieIndex].Values.Clear();
                _chart.Series[SerieIndex].Values.AddRange(Points);
            }
        }
    }
}

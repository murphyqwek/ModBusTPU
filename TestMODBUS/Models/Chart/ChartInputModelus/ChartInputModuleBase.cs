using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TestMODBUS.ViewModels;

namespace TestMODBUS.Models.Chart.ChartInputModelus
{
    abstract public class ChartInputModuleBase
    {
        protected ChartModel _chart;
        public ChartInputModuleBase(ChartModel Chart) 
        { 
            _chart = Chart;
        }

        abstract public void RemoveChannel(int Channel);
        virtual public void AddNewChannels(int[] Channels)
        {
            _chart.Channels.Clear();
            foreach (int Channel in Channels)
                AddNewChannel(Channel);
        }
        virtual public void SignToChannels(int[] Channels)
        {
            _chart.Series.Clear();
            _chart.SeriesByChannel.Clear();
            _chart.Channels.Clear();
            AddNewChannels(Channels);

            _chart.ResignDataStorageLastUpdateChannel();
        }
        abstract public bool AddNewChannel(int Channel);
        abstract public void UpdateSeries(int LeftIndex, int RightIndex, Data.Data DataStorage);
        abstract public double GetYMax(int LeftIndex, int RightIndex, Data.Data DataStorage, double YMaxGap);
        abstract public double GetYMin(int LeftIndex, int RightIndex, Data.Data DataStorage, double YMinGap);

    }
}

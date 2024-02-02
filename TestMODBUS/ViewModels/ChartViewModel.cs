using LiveCharts.Wpf.Charts.Base;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMODBUS.ViewModels.Base;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TestMODBUS.Commands;

namespace TestMODBUS.ViewModels
{
    public class ChartViewModel : BaseViewModel
    {

        #region Properties
        public SeriesCollection Series => _chart.Series;

        public string Title => _chart.Title;

        public double XMax => _chart.XMax;
        public double XMin => _chart.XMin;
        public double YMax => _chart.YMax;
        public double YMin => _chart.YMin;

        public bool IsScrollVisible => !_chart.IsDrawing && _chart.MaxWindowTime > 0;
        public bool IsDrawing => _chart.IsDrawing;
        public int MaxWindowTime => _chart.MaxWindowTime;

        public int StartPoint
        {
            set
            {
                _chart.ChangeWindowStartPoint(value);
            }
        }

        public ObservableCollection<bool> Channels { get; }
        #endregion

        #region Commands

        #region Change Channel List

        public ICommand ChangeChannelListCommand { get; }

        private void ChaneChannelListHandler(object Channel)
        {
            //if (IsDrawing)
                //return;

            int channel = 0;
            if (!Int32.TryParse(Channel.ToString(), out channel))
                throw new Exception("Channel must be Interger");

            if (Channels[channel])
                _chart.RemoveChannel(channel);
            else
                _chart.AddNewChannel(channel);
        }

        #endregion

        #endregion

        private ChartModel _chart;

        public ChartViewModel(ChartModel Chart) 
        {
            Channels = GetChannelsFromChart(Chart);

            ChangeChannelListCommand = new RemoteCommandWithParameter(ChaneChannelListHandler);

            _chart = Chart;

            _chart.Series.CollectionChanged += (s, e) => OnPropertyChanged(nameof(Series));
            _chart.PropertyChanged += (s, e) => OnPropertyChanged(e.PropertyName);

            _chart.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_chart.IsDrawing) ||
                    e.PropertyName == nameof(_chart.MaxWindowTime))
                    OnPropertyChanged(nameof(IsScrollVisible));
            };

            _chart.Channels.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Remove)
                    Channels[(int)e.OldItems[0]] = false;
                else if (e.Action == NotifyCollectionChangedAction.Add)
                    Channels[(int)e.NewItems[0]] = true;
                else if (e.Action == NotifyCollectionChangedAction.Reset)
                    SetChannelsAllToFalse();
            };
        }

        private ObservableCollection<bool> GetChannelsFromChart(ChartModel chart)
        {
            ObservableCollection<bool> channels = new ObservableCollection<bool>();
            for(int i = 0; i < 8; i++)
            {
                if (chart.Channels.Contains(i))
                    channels.Add(true);
                else
                    channels.Add(false);
            }

            return channels;
        }

        private void SetChannelsAllToFalse()
        {
            for(int i = 0; i < Channels.Count; i++)
                Channels[i] = false;
        }
    }
}

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
using TestMODBUS.ViewModels.ChartViewModels;
using TestMODBUS.Models.Services;
using System.Runtime.Remoting.Channels;
using TestMODBUS.Models.Chart.ChartInputModelus.Factories;

namespace TestMODBUS.ViewModels
{
    public class ChartViewModel : BaseViewModel
    {

        #region Properties
        public SeriesCollection Series => _chart.Series;
        public ObservableCollection<CurrentChannelValueViewModel> CurrentValues { get; }

        public List<ChartInputTypeViewModel> ChartInputTypes { get; }
        public ChartInputTypeViewModel CurrentChartInputType 
        { 
            get => _currentChartInputType;
            set
            {
                _currentChartInputType = value;
                OnPropertyChanged();
                ChangeChartInputType(_currentChartInputType);
            } 
        }

        public string Title => _chart.Title;

        public double XMax => _chart.XMax;
        public double XMin => _chart.XMin;
        public double YMax => _chart.YMax;
        public double YMin => _chart.YMin;

        public bool IsScrollVisible => !_chart.IsDrawing && _chart.MaxWindowTime > 0;
        public bool IsDrawing => _chart.IsDrawing;
        public int MaxWindowTime => _chart.MaxWindowTime;

        public double CurrentTime => _chart.CurrentTime / 1000;

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
            if (!Int32.TryParse(Channel.ToString(), out int channel))
                throw new Exception("Channel must be Interger");

            if (Channels[channel])
                _chart.RemoveChannel(channel);
            else
                _chart.AddNewChannel(channel);
        }

        #endregion

        #endregion

        private ChartModel _chart;
        private ChartInputTypeViewModel _currentChartInputType;

        public ChartViewModel(ChartModel Chart) 
        {
            Channels = GetChannelsFromChart(Chart);
            CurrentValues = new ObservableCollection<CurrentChannelValueViewModel>();

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

            _chart.Channels.CollectionChanged += OnChannelsCollectionChanged;

            foreach(var channel in _chart.Channels)
            {
                AddNewCurrentChannelValue(channel);
            }

            ChartInputTypes = new List<ChartInputTypeViewModel>();
            foreach (var type in ChartInputTypeName.GetValues())
            {
                var ChartInputVM = new ChartInputTypeViewModel(type);
                ChartInputTypes.Add(ChartInputVM);
                if(type == _chart.ChartInputType)
                    CurrentChartInputType = ChartInputVM;
            }
        }

        private void ChangeChartInputType(ChartInputTypeViewModel chartInputType)
        {
            if (chartInputType.ChartInputType == _chart.ChartInputType)
                return;

            _chart.ChangeChartInputModule(new ChartInputSimpleFactory(), chartInputType.ChartInputType);
        }

        private void AddNewCurrentChannelValue(int Channel)
        {
            string ValueType = "";

            if (ChannelTypeList.TokChannels.Contains(Channel))
                ValueType = "А";
            if (ChannelTypeList.VoltChannels.Contains(Channel))
                ValueType = "В";
            CurrentValues.Add(new CurrentChannelValueViewModel(_chart.GetSeriesByChannel(Channel), ValueType));
        }

        private void OnChannelsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    Channels[(int)e.OldItems[0]] = false;
                    CurrentValues.Remove(CurrentValues.Where(i => i.Title == $"CH_{(int)e.OldItems[0]}").First());
                }
                else if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    int channel = (int)e.NewItems[0];
                    Channels[channel] = true;
                    AddNewCurrentChannelValue(channel);
                }
                else if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    SetChannelsAllToFalse();
                    CurrentValues.Clear();
                }
            }
            catch { }
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

using LiveCharts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ModBusTPU.Commands;
using ModBusTPU.Models.INotifyPropertyBased;
using ModBusTPU.Models.ModbusSensor;
using ModBusTPU.Models.ModbusSensor.Factories;
using ModBusTPU.Models.Services;

namespace ModBusTPU.ViewModels.ChartViewModels
{
    public class ModbusSensorViewModel : INotifyBase
    {
        #region Properties
        public SeriesCollection Series => _chart.Series;
        public ObservableCollection<string> CurrentValues { get; }

        public List<SensorTypeViewModel> SensorTypes { get; }
        public SensorTypeViewModel CurrentSensorType
        {
            get => _currentSensorType;
            set
            {
                _currentSensorType = value;
                OnPropertyChanged();
                ChangeChartInputType(_currentSensorType);
            }
        }

        public string Title => _chart.Title;

        public double XMax => _chart.XMax;
        public double XMin => _chart.XMin;
        public double YMax => _chart.YMax;
        public double YMin => _chart.YMin;

        public bool IsScrollVisible => !_chart.IsDrawing && _chart.MaxWindowTime > 0;
        public bool IsDrawing => _chart.IsDrawing;

        public double CurrentTime => _chart.CurrentX;

        public int MaxWindowTime => _chart.MaxWindowTime;

        public int StartPoint
        {
            set
            {
                _sensor.ChangeWindowPosition(value);
            }
        }

        public ObservableCollection<ChannelViewModel> Channels { get; }
        #endregion

        #region Commands

        #region Change Channel List

        public ICommand ChangeChannelListCommand { get; }

        private void ChangeChannelListHandler(object Channel)
        {
            if (!Int32.TryParse(Channel.ToString(), out int channel))
                throw new Exception("Channel must be Interger");

            if (Channels[channel].IsChosen)
                _sensor.RemoveChannel(channel);
            else
                _sensor.AddNewChannel(channel);

            SetChannelsIsChosenBySensorData();
        }

        #endregion

        #endregion

        private ModbusSensor _sensor;
        private ModbusSensorData _sensorData;
        private Chart _chart;
        private SensorTypeViewModel _currentSensorType;

        public ModbusSensorViewModel(ModbusSensor Sensor)
        {
            _sensor = Sensor;
            _chart = _sensor.Chart;
            _sensorData = _sensor.SensorData;

            Channels = new ObservableCollection<ChannelViewModel>();

            SetUsingChannels();
            CurrentValues = _sensor.SensorData.CurrentValues;

            ChangeChannelListCommand = new RemoteCommandWithParameter(ChangeChannelListHandler);

            _chart.Series.CollectionChanged += (s, e) => OnPropertyChanged(nameof(Series));
            _chart.PropertyChanged += (s, e) => OnPropertyChanged(e.PropertyName);
            _chart.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_chart.IsDrawing) ||
                    e.PropertyName == nameof(_chart.MaxWindowTime))
                    OnPropertyChanged(nameof(IsScrollVisible));
                if (e.PropertyName == nameof(_chart.CurrentX))
                    OnPropertyChanged(nameof(CurrentTime));
            };

            _sensorData.UsingChannels.CollectionChanged += (s, e) =>
            {
                SetChannelsIsChosenBySensorData();
                OnPropertyChanged(nameof(Channels));
            };
            _sensorData.CurrentValues.CollectionChanged += (s, e) => OnPropertyChanged(nameof(CurrentValues));
            _sensor.ChannelsTypeChangedEvent += ChannelsTypeChangedHander;

            SensorTypes = new List<SensorTypeViewModel>();
            foreach (SensorType type in Enum.GetValues(typeof(SensorType)))
            {
                var ChartInputVM = new SensorTypeViewModel(type);
                SensorTypes.Add(ChartInputVM);
                if (type == _sensor.SensorType)
                    CurrentSensorType = ChartInputVM;
            }
        }

        private void ChannelsTypeChangedHander()
        {
            foreach (var Channel in Channels)
                Channel.UpdateColor();
        }

        private void ChangeChartInputType(SensorTypeViewModel sensorType)
        {
            if (sensorType.SensorType == _sensor.SensorType)
                return;

            _sensor.SetInputMode(new ModbusSensorSimpleFactory(), sensorType.SensorType);
        }

        private void SetUsingChannels()
        {
            Channels.Clear();
            for (int i = 0; i < _sensor.SensorData.UsingChannels.Count; i++)
            {
                Channels.Add(new ChannelViewModel(new Models.Data.ChannelModel(i, _sensor.SensorData.UsingChannels[i], $"CH_{i}")));
            }
        }

        private void SetChannelsIsChosenBySensorData()
        {
            for (int i = 0; i < _sensorData.UsingChannels.Count; i++)
                Channels[i].IsChosen = _sensorData.UsingChannels[i];
        }
    }
}

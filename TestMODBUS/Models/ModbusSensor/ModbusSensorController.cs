using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TestMODBUS.Models.Data;
using TestMODBUS.Models.ModbusSensor.ChartDataPrepatations;
using TestMODBUS.Models.ModbusSensor.ModBusInputs;

namespace TestMODBUS.Models.ModbusSensor
{
    public class ModbusSensorController
    {
        private ModbusSensorData _sensorData;
        private DataStorage _dataStorage;
        private Chart _chart;
        private ChartDataPreparationBase _dataPreparation;

        public ModbusSensorController(ModbusSensorData SensorData, DataStorage DataStorage, Chart Chart, ChartDataPreparationBase DataPreparation)
        {
            _sensorData = SensorData;
            _dataStorage = DataStorage;
            _chart = Chart;
            _dataPreparation = DataPreparation;
        }

        public void ChangeChartDataPreparation(ChartDataPreparationBase DataPreparation) => _dataPreparation = DataPreparation;

        public void DetachInputModule(NotifyCollectionChangedEventHandler Handler)
        {
            _chart.ClearChannels();
            _sensorData.UnusedAllChannels();
            _sensorData.ClearCurrentValues();

            DetachImputModuleFromDataStorage(Handler);

            _chart.RemoveAllSeries();
        }

        public void DetachImputModuleFromDataStorage(NotifyCollectionChangedEventHandler Handler)
        {
            if (_dataStorage != null)
            {
                _dataStorage.UnsingToAllChannels(Handler);
            }
        }

        public void ChangeDataStorage(DataStorage NewDataStorage)
        {
            _dataStorage = NewDataStorage;
        }

        public void StartDrawing()
        {
            _chart.StartDrawing();
        }

        public void StopDrawing()
        {
            _chart.ClearChannels();
            _chart.StopDrawing();
        }

        public void MoveToStart()
        {
            _chart.MoveToStart();
            _chart.MaxWindowTime = _dataStorage.GetChannelLength() != 0 ? Convert.ToInt32(_dataStorage.GetLastTime()) - Convert.ToInt32(Chart.MaxWindowWidth) : 0;
            UpdateChartByCurrentX(0);
        }

        public void StopDrawingAndMoveToStart()
        {
            StopDrawing();
            MoveToStart();
        }

        public void SetUsingChannel(int Channel, bool State) => _sensorData.SetUsingChannel(Channel, State);
        public int GetLastChannel()
        {
            List<int> UsingChannels = GetUsingChannels().ToList();
            if (UsingChannels.Count == 0)
                return -1;
            else
                return UsingChannels.Max();
        }

        public void AddNewLineSerie(string Title, Brush Color) => _chart.AddNewLineSerie(Title, Color);
        public void RemoveSerie(string Title) => _chart.RemoveSerie(Title);
        public void ClearChannels() => _chart.ClearChannels();

        public IList<int> GetUsingChannels() => _sensorData.GetUsingChannels();
        public bool GetChannelUsingState(int Channel) => _sensorData.GetChannelUsingState(Channel); 
        public IList<bool> GetAllChannels() => _sensorData.UsingChannels;

        public void UpdateChartAfterNewChannelAdded()
        {
            if (_dataStorage.GetChannelLength() == 0 || _chart.IsDrawing)
                return;

            var UsingChannels = _sensorData.GetUsingChannels();
            if (UsingChannels.Count == 0)
                return;

            var NewSeriesPoints = _dataPreparation.GetPointsByCurrentX(UsingChannels, _dataStorage, _chart.CurrentX);

            if (NewSeriesPoints.Count == 0)
                return;

            _chart.UpdateAllSeriesPoints(NewSeriesPoints);
        }

        public void UpdateChart(bool IsUpdateCurrentValues)
        {
            if(_dataStorage.GetChannelLength() == 0) 
                return;

            IList<int> ChannelsToUpdate = _sensorData.GetUsingChannels();
            if (ChannelsToUpdate.Count == 0)
                return;

            var NewSeriesPoints = _dataPreparation.GetNewPoints(ChannelsToUpdate, _dataStorage);
            double NewCurrentPosition = _dataPreparation.GetNewCurrentPosition(_dataStorage);

            if (NewSeriesPoints.Count == 0)
                return;

            _chart.ChangeWindowPosition(NewCurrentPosition);
            _chart.UpdateAllSeriesPoints(NewSeriesPoints);

            if (IsUpdateCurrentValues)
            {
                UpdateCurrentValues(ChannelsToUpdate);
            }
        }
        public void UpdateChartByCurrentX(double CurrentX)
        {
            if (_dataStorage.GetChannelLength() == 0)
                return;

            IList<int> ChannelsToUpdate = _sensorData.GetUsingChannels();

            if (ChannelsToUpdate.Count == 0) return;

            var NewSeriesPoints = _dataPreparation.GetPointsByCurrentX(ChannelsToUpdate, _dataStorage, CurrentX);

            if(NewSeriesPoints.Count == 0) 
                return;

            _chart.ChangeWindowPosition(CurrentX);
            _chart.UpdateAllSeriesPoints(NewSeriesPoints);
        }

        public void MoveWindow(double StartPoint)
        {
            if (_dataStorage.GetChannelLength() == 0)
                return;

            IList<int> ChannelsToUpdate = _sensorData.GetUsingChannels();
            if(ChannelsToUpdate.Count == 0) return;

            var NewSeriesPoints = _dataPreparation.GetPointsByCurrentX(ChannelsToUpdate, _dataStorage, StartPoint);
            _chart.ChangeWindowStartPoint(StartPoint, _dataStorage.GetLastTime());
            _chart.UpdateAllSeriesPoints(NewSeriesPoints);
        }

        public void UpdateCurrentValues(IList<string> CurrentValues)
        {
            _sensorData.SetCurrentValues(CurrentValues);
        }

        public void UpdateCurrentValues(IList<int> ChannelToUpdate)
        {
            var CurrentValues = _dataPreparation.GetCurrentValues(ChannelToUpdate, _dataStorage);

            UpdateCurrentValues(CurrentValues);
        }

        public void SignToChannelUpdation(int Channel, NotifyCollectionChangedEventHandler Handler) => _dataStorage.SignToChannel(Channel, Handler);

        public void UnsignToChannelUpdation(int Channel, NotifyCollectionChangedEventHandler Handler) => _dataStorage.UnsingToChannel(Channel, Handler);

    }
}

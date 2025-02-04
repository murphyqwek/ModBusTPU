using LiveCharts.Wpf.Charts.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModBusTPU.Exceptions;
using ModBusTPU.Models.Data;
using ModBusTPU.Models.INotifyPropertyBased;
using ModBusTPU.Models.ModbusSensor.ChartDataPrepatations;
using ModBusTPU.Models.ModbusSensor.Factories;
using ModBusTPU.Models.ModbusSensor.ModBusInputs;

namespace ModBusTPU.Models.ModbusSensor
{
    public class ModbusSensor : INotifyBase
    {
        public Chart Chart { get; }
        public ModbusSensorData SensorData { get; }

        public delegate void ChannelsTypeHandler();
        public event ChannelsTypeHandler ChannelsTypeChangedEvent;

        public SensorType SensorType
        {
            get => _sensorType;

            private set
            {
                _sensorType = value;
                OnPropertyChanged();
            }
        }

        private ModBusInputBase Input;
        private ModbusSensorController Controller;
        private DataStorage DataStorage;

        private SensorType _sensorType;

        public ModbusSensor(Chart Chart, DataStorage DataStorage, AbstractModbusSensorFactory Factory, SensorType SensorType, IEnumerable<int> Channels)
        {
            this.Chart = Chart;
            this.SensorData = new ModbusSensorData();
            this.DataStorage = DataStorage;
            SetInputMode(Factory, SensorType, Channels);
        }

        public ModbusSensor(Chart Chart, DataStorage DataStorage, AbstractModbusSensorFactory Factory, SensorType SensorType, int[] Channels)
        {
            this.Chart = Chart;
            this.SensorData = new ModbusSensorData();
            this.DataStorage = DataStorage;
            SetInputMode(Factory, SensorType, Channels.ToList());
        }

        public void SetInputMode(AbstractModbusSensorFactory Factory, SensorType SensorType)
        {
            PresetInputMode(Factory, SensorType);

            Input = Factory.GetInputModule(SensorType, Controller);
        }

        public void SetInputMode(AbstractModbusSensorFactory Factory, SensorType SensorType, IEnumerable<int> Channels)
        {
            PresetInputMode(Factory, SensorType);

            Input = Factory.GetInputModule(SensorType, Controller, Channels);
        }

        private void PresetInputMode(AbstractModbusSensorFactory Factory, SensorType SensorType)
        {
            if (Input != null)
                Input.DetachFromController();

            ChangeChartDataPreparation(Factory, SensorType);

            this.SensorType = SensorType;
        }

        public void ChangeDataStorage(DataStorage NewDataStorage)
        {
            this.DataStorage = NewDataStorage;

            if (this.DataStorage == null)
                this.DataStorage = new DataStorage();

            Input.DetachFromDataStorage();
            Controller.ChangeDataStorage(NewDataStorage);
            Input.ResignToAllUsingChannels();
            Controller.MoveToStart();
        }

        private void ChangeChartDataPreparation(AbstractModbusSensorFactory Factory, SensorType SensorType)
        {
            var NewChartDataPreparation = Factory.GetChartDataPrepatation(SensorType);

            if (Controller == null)
                Controller = new ModbusSensorController(SensorData, DataStorage, Chart, NewChartDataPreparation);
            else
                Controller.ChangeChartDataPreparation(NewChartDataPreparation);
        }

        public void AddNewChannel(int Channel) => Input.AddNewChannel(Channel);

        public void RemoveChannel(int Channel) => Input.RemoveChannel(Channel);

        public void StartWorking() => Input.Start();

        public bool AllNeededChannelsChonsen() => Input.CheckAllChannelsChosen();

        public void StopWorking() => Input.Stop();

        public void StopWorkingAndMoveToStart() => Input.StopAndMoveToStart();

        public void ChangeWindowPosition(double CurrentX) => Input.ChangeWindowPosition(CurrentX);

        public void CheckNewChannelsTypes()
        {
            Input.CheckNewChannelsTypes();
            ChannelsTypeChangedEvent?.Invoke();
        }
    }
}

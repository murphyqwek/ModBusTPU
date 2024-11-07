using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.ComponentModel;
using ModBusTPU.Commands;
using ModBusTPU.Exceptions;
using ModBusTPU.Models.Data;
using ModBusTPU.Models.MessageBoxes;
using ModBusTPU.Models.Services;
using ModBusTPU.ViewModels.Base;
using System.Windows;
using ModBusTPU.Models.Port.Interfaces;
using ModBusTPU.Models.ModbusSensor;
using ModBusTPU.Models.ModbusSensor.Factories;
using System;
using Microsoft.Win32;
using ModBusTPU.Models;
using ModBusTPU.Models.Services.Settings.Data;
using ModBusTPU.Views;
using ModBusTPU.ViewModels.ExportViewModels;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;

namespace ModBusTPU.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Public Attributes
        public ObservableCollection<string> Ports => ListAvailablePorts.AvailablePorts;
        public List<int> Speeds => ListAvailableSpeeds.ListPortSpeeds;

        public IEnumerable<CommentaryExportElementViewModel> Commentaries => (from c in _exportViewModel.Commentaries where c.IsShownOnMainWindow select c);

        public DataStorage Data { get => _data; }

        public ModbusSensor Sensor1 { get => sensor1; }
        public ModbusSensor Sensor2 { get => sensor2; }
        public ModbusSensor Sensor3 { get => sensor3; }
        public ModbusSensor Sensor4 { get => sensor4; }

        public bool IsDrawing => sensor1.Chart.IsDrawing;

        public FileNameViewModel FileNameViewModel { get; }

        public string TokMode
        {
            //Поменять названия. Не забыть поменять стандартный режим тока в главном меню
            //в выпадающем списке
            set
            {
                if (value == "AC")
                    _deviceAdress = 0x02;
                else if (value == "DC")
                    _deviceAdress = 0x03;

                else
                    throw new Exception("Uncatched tok mode");
            }
        }
        public string PortName
        {
            get => port.PortName;
            set
            {
                port.SetPortName(value);
                OnPropertyChanged();
            }
        }
        public int PortSpeed
        {
            get => port.PortSpeed;
            set
            {
                port.SetPortSpeed(value);
            }
        }
        public bool IsWorking => port.IsPortOpen || _isWorking;
        public bool IsNotWorking => !IsWorking;

        public int MeasureDelay { get; set; } = 300;

        public bool Debug 
        { 
            get => _debug; 
            set 
            {
                if (IsDrawing)
                    return;

                _debug = value;
                var _dataConnector = new DataConnector(_data);
                if (_debug)
                    _portListener = new TestPortListener(_dataConnector);
                else
                    _portListener = new PortListener(port, _dataConnector, StopCommandHandler);
            } 
        }

        #endregion

        #region Private Fields
        private IPortListener _portListener;
        private ObservablePort port;
        private DataStorage _data;

        private ModbusSensor sensor1;
        private ModbusSensor sensor2;
        private ModbusSensor sensor3;
        private ModbusSensor sensor4;
        private bool _debug = false;
        private bool _isWorking = false;

        private byte _deviceAdress = 0x02;

        private readonly ExportViewModel _exportViewModel;
        #endregion

        #region Commands

        #region Start Command

        //Запуск считывания с датчика
        public ICommand StartCommand { get; }

        private void StartCommandHandler()
        {
            if(!sensor1.AllNeededChannelsChonsen() ||
                !sensor2.AllNeededChannelsChonsen() ||
                !sensor3.AllNeededChannelsChonsen() ||
                !sensor4.AllNeededChannelsChonsen())
            {
                ErrorMessageBox.Show("Не все нужные каналы были выбраны");
                return;
            }

            if(_data.GetChannelLength() > 0)
            {
                if (RequestYesNoMessageBox.Show("Перед запуском проверьте, что сохранилы предыдущие данные") != MessageBoxResult.Yes)
                    return;
            }

            try
            {
                _portListener.StartListen(MeasureDelay, _deviceAdress);
                sensor1.StartWorking();
                sensor2.StartWorking();
                sensor3.StartWorking();
                sensor4.StartWorking();

                _isWorking = true;
                OnPropertyChanged(nameof(IsWorking));
                OnPropertyChanged(nameof(IsNotWorking));
            }
            catch (NoPortAvailableException ex)
            {
                ErrorMessageBox.Show(ex.Message);
            }
            catch(NotAllChannelsChosen ex)
            {
                StopCommandHandler();
                ErrorMessageBox.Show(ex.Message);
            }
            catch (ChosenPortUnavailableException ex)
            {
                ListAvailablePorts.UpdateAvailablePortList();
                ErrorMessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region Stop Command

        //Команда для отсановки считывания данных
        public ICommand StopCommand { get; }

        private void StopCommandHandler()
        {
            _portListener.StopListen();
            _isWorking = false;
            OnPropertyChanged(nameof(IsWorking));
            OnPropertyChanged(nameof(IsNotWorking));
            sensor1.StopWorkingAndMoveToStart();
            sensor2.StopWorkingAndMoveToStart();
            sensor3.StopWorkingAndMoveToStart();
            sensor4.StopWorkingAndMoveToStart();
        }
        #endregion

        #region Clear Data Command

        //Очистка графика
        public ICommand ClearCommand { get; }

        private void ClearCommandHandler()
        {
            if (IsDrawing)
            {
                ErrorMessageBox.Show("Остановите считывание порта прежде чем очистить графики");
                return;
            }


            if (RequestYesNoMessageBox.Show("Вы уверены, что хотите очистить график?") != MessageBoxResult.Yes)
                return;

            _data.Clear();
        }

        #endregion

        #region BackUp Command 
        
        public ICommand BackUpCommand { get; }
        private void BackUpCommandHandler()
        {
            string FilePath = FileHelper.GetSaveFilePath($"*{DataFileManager.DataLogEXTENSION}|*{DataFileManager.DataLogEXTENSION};", DataFileManager.DataLogEXTENSION);
            if (FilePath == null)
                return;

            try
            {
                DataFileManager.SaveLogs(Data, FilePath);
                SuccessMessageBox.Show("Эксперимент успешно сохранены");
            }
            catch(Exception ex)
            {
                ErrorMessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region Export Data Command

        //Экспорт данных в Excel
        public ICommand ExportDataCommand { get; }

        private void ExportDataCommandHandler()
        {
            _exportViewModel.SetNewDataStorage(_data);
            ExportWindow _exportWindow = new ExportWindow(_exportViewModel);
            _exportWindow.ShowDialog();
            OnPropertyChanged(nameof(Commentaries));
        }

        #endregion

        #region Upload DataStorage

        public ICommand UploadDataStorageCommand { get; }

        private void UploadDataStorageCommandHandler()
        {
            if (IsDrawing)
            {
                ErrorMessageBox.Show("Остановите считывание порта прежде чем загрузить данные");
                return;
            }


            if (RequestYesNoMessageBox.Show("Вы уверены, что загрузить новые данные?") != MessageBoxResult.Yes)
                return;

            var NewData = DataFileManager.ReadLog();

            if (NewData == null)
                return;

            _data = NewData;
            var _dataConnector = new DataConnector(_data);
            _portListener = new PortListener(port, _dataConnector, OnPortErrorClosedByError);
            sensor1.ChangeDataStorage(_data);
            sensor2.ChangeDataStorage(_data);
            sensor3.ChangeDataStorage(_data);
            sensor4.ChangeDataStorage(_data);
        }

        #endregion

        #region Open ChangeChannelsType Menu

        public ICommand ChangeChannelsTypeCommand { get; }

        private void ChangeChannelsTypeCommandHandler()
        {
            var Types = ChannelTypeList.GetChannelsType();
            var temp = new ChannelType[Types.Count];
            Types.CopyTo(temp);


            var ChannelsTypesWindow = new ChannelsTypeWindow();
            ChannelsTypesWindow.ShowDialog();

            for (int i = 0; i < ChannelTypeList.ChannelCounts; i++)
            {
                if (temp[i] != ChannelTypeList.GetChannelType(i))
                {
                    sensor1.CheckNewChannelsTypes();
                    sensor2.CheckNewChannelsTypes();
                    sensor3.CheckNewChannelsTypes();
                    sensor4.CheckNewChannelsTypes();
                    return;
                }
            }
        }

        #endregion

        #endregion

        #region Events

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            if (_portListener != null)
                _portListener.StopListen();

            sensor1?.StopWorking();
            sensor2?.StopWorking();
            sensor3?.StopWorking();
            sensor4?.StopWorking();
        }

        #region On Port Closed By Error

        public void OnPortErrorClosedByError()
        {
            ErrorMessageBox.Show("Возникли проблемы с портом. Проверьте соединение");
            sensor1?.StopWorking();
            sensor2?.StopWorking();
            sensor3?.StopWorking();
            sensor4?.StopWorking();
        }

        #endregion

        #endregion

        public MainViewModel()
        {
            //Перехват необработанных исключений
            Application.Current.DispatcherUnhandledException += ApplicationClosedByErrorHandler;

            //Инициализируем объекты
            port = new ObservablePort();
            _data = new DataStorage();

            var _dataConnector = new DataConnector(_data);
            _portListener = new PortListener(port, _dataConnector, OnPortErrorClosedByError);

            //Инициализируем модули
            ModbusSensorSimpleFactory Factory = new ModbusSensorSimpleFactory();
            sensor1 = new ModbusSensor(new Chart(), _data, Factory, SensorType.Standart, new int[] { 0, 5 });
            sensor2 = new ModbusSensor(new Chart(), _data, Factory, SensorType.Standart, new int[] { 0, 5 });
            sensor3 = new ModbusSensor(new Chart(), _data, Factory, SensorType.Standart, new int[] { 0, 5 });
            sensor4 = new ModbusSensor(new Chart(), _data, Factory, SensorType.Standart, new int[] { 0, 5 });

            //Создаем класс, который будет хранить имя текущего эксперимента
            FileNameViewModel = new FileNameViewModel();

            //Иницилизируем команды
            StartCommand = new RemoteCommand(StartCommandHandler);
            StopCommand = new RemoteCommand(StopCommandHandler);
            ClearCommand = new RemoteCommand(ClearCommandHandler);
            ExportDataCommand = new RemoteCommand(ExportDataCommandHandler);
            UploadDataStorageCommand = new RemoteCommand(UploadDataStorageCommandHandler);
            ChangeChannelsTypeCommand = new RemoteCommand(ChangeChannelsTypeCommandHandler);
            BackUpCommand = new RemoteCommand(BackUpCommandHandler);

            //Подписиваем объекты на OnPropertyChanged других объектов
            ListAvailablePorts.AvailablePorts.CollectionChanged += (s, e) => OnPropertyChanged(nameof(Ports));
            port.PropertyChanged += (s, e) => OnPropertyChanged(e.PropertyName);

            //Инциалзириуем окно экспорта данных
            _exportViewModel = new ExportViewModel(_data);
        }

        private void ApplicationClosedByErrorHandler(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (Data.GetChannelLength() > 0)
                DataFileManager.SaveLogs(Data);
        }
    }
}
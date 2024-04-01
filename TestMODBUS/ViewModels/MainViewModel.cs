using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.ComponentModel;
using TestMODBUS.Commands;
using TestMODBUS.Exceptions;
using TestMODBUS.Models.Data;
using TestMODBUS.Models.MessageBoxes;
using TestMODBUS.Models.Services;
using TestMODBUS.ViewModels.Base;
using System.Windows;
using TestMODBUS.Models.Port.Interfaces;
using TestMODBUS.Models.Chart.ChartInputModelus;
using TestMODBUS.Models.Chart.ChartInputModelus.Factories;

namespace TestMODBUS.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Public Attributes
        public ObservableCollection<string> Ports => ListAvailablePorts.AvailablePorts;
        public List<int> Speeds => ListAvailableSpeeds.ListPortSpeeds;

        public Data Data { get => _data; }
        public ChartModel ChartTok1 { get => chart1; }
        public ChartModel ChartTok2 { get => chart2; }
        public ChartModel ChartTok3 { get => chart3; }
        public ChartModel ChartTok4 { get => chart4; }

        public ChartViewModel TestChartModel { get; }

        public bool IsDrawing => chart1.IsDrawing;
        public int MaxStartTime => chart1.MaxWindowTime;

        public FileNameViewModel FileNameViewModel { get; }

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
        public bool IsPortOpen => port.IsPortOpen || _isWorking;
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

        private IPortListener _portListener;
        private ObservablePort port;
        private Data _data;
        private ChartModel chart1;
        private ChartModel chart2;
        private ChartModel chart3;
        private ChartModel chart4;
        private bool _debug = false;
        private bool _isWorking = false;

        #region Commands

        #region Start Command

        //Запуск считывания с датчика
        public ICommand StartCommand { get; }

        private void StartCommandHandler()
        {
            try
            {
                _portListener.StartListen(MeasureDelay);
                _isWorking = true;
                OnPropertyChanged(nameof(IsPortOpen));
                chart1.StartDrawing();
                chart2.StartDrawing();
                chart3.StartDrawing();
                chart4.StartDrawing();
            }
            catch (NoPortAvailableException ex)
            {
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
            OnPropertyChanged(nameof(IsPortOpen));
            chart1.StopDrawingAndMoveToStart();
            chart2.StopDrawingAndMoveToStart();
            chart3.StopDrawingAndMoveToStart();
            chart4.StopDrawingAndMoveToStart();
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

        #region Export Data Command

        //Экспорт данных в Excel
        public ICommand ExportDataCommand { get; }

        private void ExportDataCommandHandler()
        {
            string name = FileNameViewModel.GetFileName();

            if (FileNameViewModel.isFileNameEmpty())
            {
                MessageBox.Show("Вы не заполнили поле «Название эксперимента»", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            ExportWindow exportWindow = new ExportWindow(_data, name);
            exportWindow.ShowDialog();
        }

        #endregion

        #endregion

        #region Events

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            if (_portListener != null)
                _portListener.StopListen();
            chart1.StopDrawing();
            chart2.StopDrawing();
            chart3.StopDrawing();
            chart4.StopDrawing();
        }

        #region On Port Closed By Error

        public void OnPortErrorClosedByError()
        {
            ErrorMessageBox.Show("Возникли проблемы с портом. Проверьте соединение");
            chart1.StopDrawingAndMoveToStart();
            chart2.StopDrawingAndMoveToStart();
            chart3.StopDrawingAndMoveToStart();
            chart4.StopDrawingAndMoveToStart();
        }

        #endregion

        #endregion

        public MainViewModel()
        {
            //Инициализируем объекты
            port = new ObservablePort();
            _data = new Data();

            var _dataConnector = new DataConnector(_data);
            _portListener = new PortListener(port, _dataConnector, OnPortErrorClosedByError);

            //Инициализируем Чарты
            ChartInputSimpleFactory ChartInputFactory = new ChartInputSimpleFactory();
            chart1 = new ChartModel(_data, new int[] { 0, 5 }, "Мощность", ChartInputFactory, ChartInputType.Power);
            chart2 = new ChartModel(_data, new int[] { 1 }, "Ток 2", ChartInputFactory, ChartInputType.Standart);
            chart3 = new ChartModel(_data, new int[] { 6 }, "Напряжение", ChartInputFactory, ChartInputType.Standart);
            chart4 = new ChartModel(_data, new int[] { 0, 1 }, "Ток 1, Ток 2", ChartInputFactory, ChartInputType.Standart);
            //TestChartModel = new ChartViewModel(chart1);

            //Создаем класс, который будет хранить имя текущего эксперимента
            FileNameViewModel = new FileNameViewModel();
            
            //Иницилизируем команды
            StartCommand = new RemoteCommand(StartCommandHandler);
            StopCommand = new RemoteCommand(StopCommandHandler);
            ClearCommand = new RemoteCommand(ClearCommandHandler);
            ExportDataCommand = new RemoteCommand(ExportDataCommandHandler);

            //Подписиваем объекты на OnPropertyChanged других объектов
            ListAvailablePorts.AvailablePorts.CollectionChanged += (s, e) => OnPropertyChanged(nameof(Ports));
            port.PropertyChanged += (s, e) => OnPropertyChanged(e.PropertyName);
        }
    }
}
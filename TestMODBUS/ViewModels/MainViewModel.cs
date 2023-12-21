using LiveCharts;
using LiveCharts.Defaults;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel;
using TestMODBUS.Commands;
using TestMODBUS.Exceptions;
using TestMODBUS.Models;
using TestMODBUS.Models.Data;
using TestMODBUS.Models.Excel;
using TestMODBUS.Models.MessageBoxes;
using TestMODBUS.Models.Port;
using TestMODBUS.ViewModels.Base;

namespace TestMODBUS.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Public Attributes
        public ObservableCollection<string> Ports => ListAvailablePorts.AvailablePorts;
        public List<int> Speeds => ListAvailableSpeeds.ListPortSpeeds;
        public SeriesCollection Series => chart.Series;
        public double XMax => chart.XMax;
        public double XMin => chart.XMin;
        public bool IsDrawing => chart.IsDrawing;
        public int MaxStartTime => chart.MaxStartTime;

        public bool IsScrollVisible 
        {
            get => isScrollVisible;
            private set
            {
                isScrollVisible = value;
                OnPropertyChanged();
            }
        }
        public int StartPoint
        {
            set
            {
                if(!IsDrawing)
                    chart.ChangeWindowStartPoint(value);
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
        public bool IsPortOpen => port.IsPortOpen;
        public int MeasureDelay { get; set; } = 300;
        #endregion

        private PortListener portListener;
        private ObservablePort port;
        private Data data;
        private ChartModel chart;
        private bool isScrollVisible = false; //Определяет видимость ScrollBar, который сдвигает график данных

        #region Commands

        #region Start Command

        //Запуск считывания с датчика
        public ICommand StartCommand { get; }

        private void StartCommandHandler()
        {
            IsScrollVisible = false; //Во время считывания данных пользователь не должен скроллить график

            //bool isStarted = true; //Если не удалось открыть порт, мы должны отменить предыдущие действия

            try
            {
                portListener.StartListen(MeasureDelay);
                chart.StartDrawing();
            }
            catch (NoPortAvailableException ex)
            {
                ErrorMessageBox.Show(ex.Message);
                //isStarted = false;
            }
            catch (ChosenPortUnavailableException ex)
            {
                ListAvailablePorts.UpdateAvailablePortList();
                ErrorMessageBox.Show(ex.Message);
                //isStarted = false;
            }
        }
        #endregion

        #region Stop Command

        //Команда для отсановки считывания данных
        public ICommand StopCommand { get; }

        private void StopCommandHandler()
        {
            IsScrollVisible = true;
            portListener.StopListen();
            chart.StopDrawing();
            chart.PutAllDataToChart();
        }
        #endregion

        #region Clear Data Command

        //Очистка графика
        public ICommand ClearCommand { get; }

        private void ClearCommandHandler()
        {
            if (RequestYesNoMessageBox.Show("Вы уверены, что хотите очистить график?") != System.Windows.MessageBoxResult.Yes)
                return;

            IsScrollVisible = false;
            data.Clear();
        }

        #endregion

        #region Export Data Command

        //Экспорт данных в Excel
        public ICommand ExportDataCommand { get; }

        private void ExportDataCommandHandler()
        {
            string path = OpenFileHelper.GetSaveFile();
            if (path == null)
                return;

            try
            {
                ExportExcel.SaveData(data, path);
                if(RequestYesNoMessageBox.Show("Отчёт сохранён. Открыть его?", "Успешно", System.Windows.MessageBoxImage.Information) == System.Windows.MessageBoxResult.Yes)
                {
                    string folder = Path.GetDirectoryName(path);
                    Process.Start("explorer", folder);
                }

            }
            catch(Exception e)
            {
                ErrorMessageBox.Show(e.Message);
            }
        }

        #endregion

        #endregion

        #region Events

        public void WindowClosing(object sender, CancelEventArgs e)
        {
            portListener.StopListen();
            chart.StopDrawing();
        }

        #region On Port Closed By Error

        public void OnPortErrorClosedByError()
        {
            ErrorMessageBox.Show("Возникли проблемы с портом. Проверьте соединение");
            chart.StopDrawing();
            chart.PutAllDataToChart();
        }

        #endregion

        #endregion

        public MainViewModel()
        {
            //Инициализируем объекты
            port = new ObservablePort();
            data = new Data();

            var dataConnector = new DataConnector(data);
            portListener = new PortListener(port, dataConnector, OnPortErrorClosedByError);
            chart = new ChartModel(data);
            
            //Иницилизируем команды
            StartCommand = new RemoteCommand(StartCommandHandler);
            StopCommand = new RemoteCommand(StopCommandHandler);
            ClearCommand = new RemoteCommand(ClearCommandHandler);
            ExportDataCommand = new RemoteCommand(ExportDataCommandHandler);

            //Подписиыаем объекты на OnPropertyChanged других объектов
            ListAvailablePorts.AvailablePorts.CollectionChanged += (s, e) => OnPropertyChanged(nameof(Ports));
            port.PropertyChanged += (s, e) => OnPropertyChanged(e.PropertyName);
            port.PropertyChanged += (s, e) => OnPropertyChanged(nameof(isScrollVisible));
            chart.Series.CollectionChanged += (s, e) => OnPropertyChanged(nameof(Series));
            chart.PropertyChanged += (s, e) => OnPropertyChanged(e.PropertyName);
        }
    }
}

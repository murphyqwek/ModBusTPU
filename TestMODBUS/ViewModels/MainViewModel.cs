using LiveCharts;
using LiveCharts.Defaults;
using ScottPlot;
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
using TestMODBUS.Commands;
using TestMODBUS.Exceptions;
using TestMODBUS.Models;
using TestMODBUS.Models.Chart;
using TestMODBUS.Models.Data;
using TestMODBUS.Models.Excel;
using TestMODBUS.Models.MessageBoxes;
using TestMODBUS.Models.Port;
using TestMODBUS.ViewModels.Base;

namespace TestMODBUS.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
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

        #region Private Fields

        private PortListener portListener;
        private ObservablePort port;
        private Data data;
        private ChartModel chart;
        private bool isScrollVisible = false;

        #endregion

        #region Commands

        #region Start Command
        public ICommand StartCommand { get; }

        private void StartCommandHandler()
        {
            IsScrollVisible = false;
            chart.StartDrawing();
            portListener.StartListen(MeasureDelay);
        }
        #endregion

        #region Stop Command
        public ICommand StopCommand { get; }

        private void StopCommandHandler()
        {
            IsScrollVisible = true;
            portListener.StopListen();
            chart.StopDrawing();
        }
        #endregion

        #region Clear Data Command

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

        public MainViewModel()
        {
            port = new ObservablePort();
            data = new Data();

            var dataConnector = new DataConnector(data);
            portListener = new PortListener(port, dataConnector);
            chart = new ChartModel(data);

            StartCommand = new RemoteCommand(StartCommandHandler);
            StopCommand = new RemoteCommand(StopCommandHandler);
            ClearCommand = new RemoteCommand(ClearCommandHandler);
            ExportDataCommand = new RemoteCommand(ExportDataCommandHandler);

            ListAvailablePorts.AvailablePorts.CollectionChanged += (s, e) => OnPropertyChanged(nameof(Ports));
            port.PropertyChanged += (s, e) => OnPropertyChanged(e.PropertyName);
            port.PropertyChanged += (s, e) => OnPropertyChanged(nameof(isScrollVisible));
            //port.PropertyChanged += (s, e) => { OnPropertyChanged(nameof(XMax)); OnPropertyChanged(nameof(XMin)); };
            chart.Series.CollectionChanged += (s, e) => OnPropertyChanged(nameof(Series));
            chart.PropertyChanged += (s, e) => OnPropertyChanged(e.PropertyName);
        }
    }
}

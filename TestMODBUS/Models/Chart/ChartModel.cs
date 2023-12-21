using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TestMODBUS.Models.Chart;
using TestMODBUS.Models.Data;
using TestMODBUS.Models.INotifyPropertyBased;
using TestMODBUS.ViewModels.Base;

namespace TestMODBUS.ViewModels
{
    public class ChartModel : INotifyBase
    {
        #region Public Attributes
        public SeriesCollection Series { get; }

        public double XMin
        {
            get
            {
                return _xMin;
            }
            private set
            {
                _xMin = value;
                OnPropertyChanged();
            }
        }

        public double XMax
        {
            get
            {
                return _xMax;
            }
            private set
            {
                _xMax = value;
                OnPropertyChanged();
            }
        }

        public bool IsDrawing
        {
            get => isDrawing;
            private set
            {
                isDrawing = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(XMin));
            }
        }

        public int MaxStartTime => _dataStorage.GetChannelLength() != 0 ? Convert.ToInt32(_dataStorage.GetLastTime()) - MaxTimeWidth : 0;
        #endregion

        public const int MaxTimeWidth = 5000; //Размер видимой пользователем области

        private double _currentrTime = 0; 
        private Data _dataStorage;
        private bool isDrawing = false;
        private double _xMin = 0; //Началальная точка, с которой будет отрисовываться график
        private double _xMax = MaxTimeWidth; //Крайняя точка, до которой будет отрисовываться графика

        private double CurrentTime
        {
            get => _currentrTime;

            set
            {
                _currentrTime = value;
                XMin = _currentrTime < MaxTimeWidth ? 0 : _currentrTime - MaxTimeWidth;
                XMax = _currentrTime < MaxTimeWidth ? MaxTimeWidth : _currentrTime;
            }
        } //Крайняя точка, до которой будет отрисовываться графика во время считывания данных

        public ChartModel(Data DataStorage)
        {
            Series = new SeriesCollection();
            _dataStorage = DataStorage;

            for (int i = 0; i < DataStorage.ChannelsData.Count; i++)
            {
                LineSeries lineSeries = InitializeChannelDataSerie("CH_" + i.ToString());
                Series.Add(lineSeries);
            }

            /*
             * Для оптимизации, график должен обновляться тогда, когда программа считала данные со всех 8 каналов
             * Подписавишсь на событие измения 7 канала, грантируется, что программа считала данные с 0 по 7 канал
             * И нужно отрисовать все полученные данные
            */
            _dataStorage.GetChannelData(7).CollectionChanged += DataStorageCollectionChangedHandler;
        }

        //Создание серии для одного канала данных
        private LineSeries InitializeChannelDataSerie(string Title) 
        {
            LineSeries lineSeries = new LineSeries() 
            {
                Title = Title,
                Values = new ChartValues<ObservablePoint>(),
                Fill = Brushes.Transparent,
                PointGeometry = null,
                LineSmoothness = 0,
                Width = 1
            };
            return lineSeries;
        }

        //Функция, которая обнавляет видмое "окно" данных, когда пользователь двигает ползунок
        public void ChangeWindowStartPoint(double newStartPoint)
        {
            if (IsDrawing || _dataStorage.GetChannelLength() == 0)
                return;

            double MaxTime = _dataStorage.GetLastTime();
            if (newStartPoint < 0)
                newStartPoint = 0;
            if (newStartPoint + MaxTimeWidth > MaxTime)
                newStartPoint = MaxTime - MaxTimeWidth;

            //Границы отображения "окна"
            var WindowDataEdges = WindowingDataHelper.GetDataWindowIndex(newStartPoint, newStartPoint + MaxTimeWidth, _dataStorage.GetChannelData(0));
            int leftEdge = WindowDataEdges.Item1, rightEdge = WindowDataEdges.Item2;

            AddWindowPointsToChart(leftEdge, rightEdge);
            XMin = newStartPoint;
            XMax = newStartPoint + MaxTimeWidth;
        }

        public void StartDrawing()
        {
            IsDrawing = true;
            ClearChannels();
            CurrentTime = 0;
        }

        public void StopDrawing() 
        { 
            IsDrawing = false;
        }

        //Обновляет видимое "окно" во время считывания данных
        private void AddRecentPoints()
        {
            CurrentTime = _dataStorage.GetLastTime();
            var WindowDataEdges = WindowingDataHelper.GetDataWindowIndex(CurrentTime, _dataStorage.GetChannelData(0));
            int leftEdge = WindowDataEdges.Item1, rightEdge = WindowDataEdges.Item2;

            AddWindowPointsToChart(leftEdge, rightEdge);
        }

        //Загружает все данные в Chart
        public void PutAllDataToChart()
        {
            XMin = 0;
            XMax = MaxTimeWidth;
            OnPropertyChanged(nameof(MaxStartTime));

            int lastPointIndex = _dataStorage.GetChannelLastPointIndex();

            if (lastPointIndex == 0)
                return;

            AddWindowPointsToChart(0, lastPointIndex);
        }

        //Отображает видимое "окно" данных
        private void AddWindowPointsToChart(int leftIndex, int rightIndex)
        {
            if (leftIndex > rightIndex)
                throw new ArgumentException("Left index bigger than right index");

            for (int Channel = 0; Channel < 8; Channel++)
            {
                var NewPoints = WindowingDataHelper.GetWindowData(leftIndex, rightIndex, _dataStorage.GetChannelData(Channel));

                Series[Channel].Values.Clear();
                Series[Channel].Values.AddRange(NewPoints);
            }
        }

        private void ClearChannels()
        {
            CurrentTime = 0;
            for (int i = 0; i < Series.Count; i++)
                ClearChannel(i);
            OnPropertyChanged(nameof(MaxStartTime));
        }

        private void ClearChannel(int Channel)
        {
            Series[Channel].Values.Clear();
        }

        private void DataStorageCollectionChangedHandler(object sedner, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                //Когда в массив были добавлены новые данные
                case NotifyCollectionChangedAction.Add:
                    AddRecentPoints();
                    break;
                //Когда массив был очищен
                case NotifyCollectionChangedAction.Reset:
                    ClearChannels();
                    break;
            }
        }
    }
}

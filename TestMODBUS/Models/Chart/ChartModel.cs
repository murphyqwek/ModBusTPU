using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TestMODBUS.Models.Services;
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
        
        public double YMax
        {
            get
            {
                return _yMax;
            }
            private set
            {
                if (_yMax == value)
                    return;
                if(_yMax < YMaxStandart)
                    _yMax = YMaxStandart;
                else
                    _yMax = value;
                OnPropertyChanged();
            }
        }
        public double YMin
        {
            get
            {
                return _yMin;
            }
            private set
            {
                if (_yMin == value)
                    return;
                if (value > YMinStandart)
                    _yMin = YMinStandart;
                else
                    _yMin = value;
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
                OnPropertyChanged(nameof(XMax));
                OnPropertyChanged(nameof(MaxWindowTime));
            }
        }

        public int MaxWindowTime => _dataStorage.GetChannelLength() != 0 ? Convert.ToInt32(_dataStorage.GetLastTime()) - MaxTimeWidth : 0;
        
        public string Title
        {
            private set
            {
                _title = value;
                OnPropertyChanged();
            }

            get => _title;
        }
        #endregion

        public const int MaxTimeWidth = 5000; //Размер видимой пользователем области
        private const double YMaxGap = 20.0; //Зазоры для графика
        private const double YMinGap = -20.0;

        private const double YMaxStandart = 220.0;
        private const double YMinStandart = -10.0;

        private double _currentrTime = 0; 
        private Data _dataStorage;
        private bool isDrawing = false;

        private string _title;

        private double _xMax = MaxTimeWidth; //Крайняя точка, до которой будет отрисовываться графика
        private double _xMin = 0; //Началальная точка, с которой будет отрисовываться график

        private double _yMax = YMaxStandart;
        private double _yMin = YMinStandart;

        private int _lastPointIndex => _dataStorage.GetChannelLastPointIndex();

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
        public ObservableConcurrentList Channels { get; }//public ObservableCollection<int> Channels { get; }//Данные с каких каналов отображает чарт
        private Dictionary<int, LineSeries> SeriesByChannel = new Dictionary<int, LineSeries>();

        public ChartModel(Data DataStorage, int[]Channels, string Title)
        {
            if (DataStorage == null)
                throw new ArgumentNullException(nameof(DataStorage));
            if (Channels == null)
                throw new ArgumentNullException(nameof(Channels));
            if (Title == null)
                throw new ArgumentNullException(nameof(Title));

            Series = new SeriesCollection();
            _dataStorage = DataStorage;
            _title = Title;
            this.Channels = new ObservableConcurrentList();//new ObservableCollection<int>();

            SignToChannels(Channels);
        }

        public void SetNewChannels(int[] NewChannels) 
        {
            Channels.Clear();
            foreach(int channel in NewChannels)
            {
                Channels.Add(channel);
            }
        }

        public void AddNewChannel(int Channel)
        {
            int oldLastSignedChannel = GetLastChannel();
            Channels.Add(Channel);
            AddNewChannelOnChart(Channel);
            ResignDataStorageLastUpdateChannel(oldLastSignedChannel);

            //Если мы ничего не считываем, то новые графики не отображатся на чарте
            //Загружаем новые графики
            ChangeWindowStartPoint(YMin);
        }

        private int GetLastChannel()
        {
            if (Channels.Count == 0)
                return -1;

            return Channels.Max();
        }

        public void RemoveChannel(int Channel)
        {
            int oldLastSignedChannel = GetLastChannel();
            if (!Channels.Contains(Channel))
                return;
            Channels.Remove(Channel);
            Series.Remove(SeriesByChannel[Channel]);
            SeriesByChannel.Remove(Channel);
            ResignDataStorageLastUpdateChannel(oldLastSignedChannel);

            //Если мы ничего не считываем, то оставшиеся графики не отображатся на чарте
            //Загружаем оставшиеся графики
            ChangeWindowStartPoint(YMin);
        }

        private void AddNewChannelOnChart(int Channel)
        {
            LineSeries lineSeries = InitializeChannelDataSerie("CH_" + Channel.ToString(), Channel);
            SeriesByChannel.Add(Channel, lineSeries);
            Series.Add(lineSeries);
        }

        private void ResignDataStorageLastUpdateChannel(int LastSignChannel = -1)
        {
            /*
             * Подписываясь на изменение последнего канала, мы гарантируем, что все предыдещие каналы, которые отображает данный чарт, были обновлены
             * тем самым оптимизируя отрисовку, отоборажая только тогда, когда изменился последний канал
            */
            if (LastSignChannel != -1)
                _dataStorage.GetChannelData(LastSignChannel).CollectionChanged -= DataStorageCollectionChangedHandler;
            if(Channels.Count > 0)
                _dataStorage.GetChannelData(Channels.Max()).CollectionChanged += DataStorageCollectionChangedHandler;
        }

        //Обновляем список каналов, которые будут отрисовываться
        public void SignToChannels(int[] NewChannels)
        {
            Series.Clear();
            SeriesByChannel.Clear();
            SetNewChannels(NewChannels);


            foreach (var Channel in Channels)
            {
                AddNewChannelOnChart(Channel);
            }


            ResignDataStorageLastUpdateChannel();
        }

        //Создание серии для одного канала данных
        private LineSeries InitializeChannelDataSerie(string Title, int Channel) 
        {
            LineSeries lineSeries = new LineSeries()
            {
                Title = Title,
                Values = new ChartValues<ObservablePoint>(),
                Fill = Brushes.Transparent,
                Stroke = ChannelColors.Colors[Channel],
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

            UpdateYMinAndMax(leftEdge, rightEdge);
            AddWindowPointsToChart(leftEdge, rightEdge);
        }

        public void StopDrawingAndMoveToStart()
        {
            StopDrawing();
            ChangeWindowStartPoint(0);
        }

        //Загружает все данные в Chart
        public void PutAllDataToChart()
        {
            int lastPointIndex = _lastPointIndex;

            if (lastPointIndex == 0)
                return;

            AddWindowPointsToChart(0, lastPointIndex);

            XMin = 0;
            XMax = MaxTimeWidth;
            OnPropertyChanged(nameof(MaxWindowTime));
        }

        private void UpdateYMinAndMax(int start, int end)
        {
            YMax = Math.Max(WindowingDataHelper.GetMaxValueOfArray(_dataStorage, start, end, Channels.ToArray()) + YMaxGap, YMax);
            YMin = Math.Min(WindowingDataHelper.GetMinValueOfArray(_dataStorage, start, end, Channels.ToArray()) + YMinGap, YMin); //YMinGap < 0!!!!
            //UpdateYAbsoluteMinAdnMax(YMax, YMin);
        }

        //Отображает видимое "окно" данных
        private void AddWindowPointsToChart(int leftIndex, int rightIndex)
        {
            if (leftIndex > rightIndex)
                throw new ArgumentException("Left index bigger than right index");

            try
            {
                foreach (var Channel in Channels)
                {
                    var NewPoints = WindowingDataHelper.GetWindowData(leftIndex, rightIndex, _dataStorage.GetChannelData(Channel));

                    SeriesByChannel[Channel].Values.Clear();
                    SeriesByChannel[Channel].Values.AddRange(NewPoints);
                }
            }
            catch
            {

            }
        }

        private void ClearChannels()
        {
            CurrentTime = 0;
            for (int i = 0; i < Series.Count; i++)
                ClearChannel(i);

            YMax = YMaxStandart;
            YMin = YMinStandart;

            OnPropertyChanged(nameof(MaxWindowTime));
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

using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Definitions.Series;
using LiveCharts.Wpf;
using LiveCharts.Wpf.Charts.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TestMODBUS.Models.Data;
using TestMODBUS.Models.INotifyPropertyBased;

namespace TestMODBUS.Models.ModbusSensor
{
    public struct SerieData
    {
        public string SerieTitle;
        public ObservablePoint[] Points;
    }

    public class Chart : INotifyBase
    {
        #region Constants

        private const double YMaxStandart = 220.0;
        private const double YMinStandart = -10.0;

        public const double MaxWindowWidth = 5000;

        #endregion

        #region Public attribuites

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
                _yMin = value;
                OnPropertyChanged();
            }
        }

        public double CurrentX
        {
            get => _currentX;

            private set
            {
                _currentX = value;
                OnPropertyChanged();
            }
        }

        public bool IsDrawing
        {
            get => _isDrawing;
            private set
            {
                _isDrawing = value;
                OnPropertyChanged();
            }
        }

        public string Title
        {
            get => _title;

            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public int MaxWindowTime
        {
            get => _maxWindowX;
            set
            {
                _maxWindowX = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Private
        private double _xMax = MaxWindowWidth; //Крайняя точка, до которой будет отрисовываться графика
        private double _xMin = 0; //Началальная точка, с которой будет отрисовываться график

        private double _yMax = YMaxStandart;
        private double _yMin = YMinStandart;

        private double _currentX = 0;

        private int _maxWindowX;

        private bool _isDrawing = false;

        private string _title;

        private Dictionary<string, ISeriesView> SerieByTitle = new Dictionary<string, ISeriesView>();

        #endregion

        public Chart(string Title = "") 
        {
            this.Title = Title;
            Series = new SeriesCollection();
        }

        public void StartDrawing()
        {
            ClearChannels();
            MoveToStart();
            IsDrawing = true;
        }

        public void StopDrawing()
        {
            IsDrawing = false;
        }

        public void MoveToStart()
        {
            ChangeWindowPosition(0, MaxWindowWidth);
        }

        public void UpdateAllSeriesPoints(IEnumerable<SerieData> UpdatingSeries)
        {
            List<ObservablePoint> AllNewPoints = new List<ObservablePoint>();
            foreach (var UpdatingSerie in UpdatingSeries)
            {
                UpdateSeriePoints(UpdatingSerie.SerieTitle, UpdatingSerie.Points, true);
                AllNewPoints.AddRange(UpdatingSerie.Points);
            }

            UpdateEdgesY(AllNewPoints);
        }

        public void UpdateSeriePoints(string SerieTitle, IEnumerable<ObservablePoint> Points, bool IsUpdatingEdgesY = false)
        {
            if(!SerieByTitle.ContainsKey(SerieTitle))
                throw new ArgumentException($"Didn't find Serie named {SerieTitle}");

            var serie = SerieByTitle[SerieTitle];

            serie.Values.Clear();
            serie.Values.AddRange(Points);

            if (IsUpdatingEdgesY)
                UpdateEdgesY(Points);
        }

        private void UpdateEdgesY(IEnumerable<ObservablePoint> Points)
        {
            double newYMax = double.NegativeInfinity;
            double newYMin = double.PositiveInfinity;

            foreach(var point in Points)
            {
                newYMax = Math.Max(newYMax, point.Y);
                newYMin = Math.Min(newYMin, point.Y);
            }

            YMax = Math.Max(newYMax, YMax);
            YMin = Math.Min(newYMin, YMin);
        }

        public void ChangeWindowPosition(double RightEdge)
        {
            XMin = RightEdge < MaxWindowWidth ? 0 : RightEdge - MaxWindowWidth; //Нужно, чтобы в начале считывания данных отображаемое окно не двигалось, пока не пройдёт больше MaxTimeWidth
            XMax = RightEdge < MaxWindowWidth ? MaxWindowWidth : RightEdge; //Крайняя точка, до которой будет отрисовываться графика во время считывания данных

            CurrentX = RightEdge;
        }

        public void ChangeWindowStartPoint(double newStartPoint, double MaxTime)
        {
            if (newStartPoint < 0)
                newStartPoint = 0;
            if (newStartPoint + MaxWindowWidth >= MaxTime && MaxTime >= MaxWindowWidth)
                newStartPoint = MaxTime - MaxWindowWidth;

            ChangeWindowPosition(newStartPoint, newStartPoint + MaxWindowWidth);
        }

        public void ChangeWindowPosition(double StartPoint, double EndPoint)
        {
            XMin = StartPoint;
            XMax = EndPoint;
        }

        public void AddNewLineSerie(string Title, Brush Color)
        {
            var NewLineSerie = CreateNewLineSeries(Title, Color);

            Series.Add(NewLineSerie);
            SerieByTitle.Add(Title, NewLineSerie);
        }

        public void RemoveAllSeries()
        {
            Series.Clear();
            SerieByTitle.Clear();
        }

        public void RemoveSerie(string Title)
        {
            foreach (var Serie in Series) 
            {
                if (Serie.Title == Title)
                {
                    Series.Remove(Serie);
                    SerieByTitle.Remove(Title);
                    break;
                }
            }

        }

        //Создание серии
        private LineSeries CreateNewLineSeries(string Title, Brush Color)
        {
            LineSeries lineSeries = new LineSeries()
            {
                Title = Title,
                Values = new ChartValues<ObservablePoint>(),
                Fill = Brushes.Transparent,
                Stroke = Color,
                PointGeometry = null,
                LineSmoothness = 0,
                Width = 1
            };
            return lineSeries;
        }

        public void ClearChannels()
        {
            CurrentX = 0;

            for (int i = 0; i < Series.Count; i++)
                ClearChannel(i);

            YMax = YMaxStandart;
            YMin = YMinStandart;
            //OnPropertyChanged(nameof(MaxWindowTime));
        }

        private void ClearChannel(int index)
        {
            Series[index].Values.Clear();
        }
    }
}

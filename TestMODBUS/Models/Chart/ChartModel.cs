﻿using LiveCharts;
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
        #region Public Fields
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

        private double CurrentTime
        {
            get => _currentrTime;

            set
            {
                _currentrTime = value;
                XMin = _currentrTime < MaxTimeWidth ? 0 : _currentrTime - MaxTimeWidth;
                XMax = _currentrTime < MaxTimeWidth ? MaxTimeWidth : _currentrTime;
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

        public const int MaxTimeWidth = 5000;

        private double _currentrTime = 0;
        private Data _dataStorage;
        private bool isDrawing = false;
        private double _xMin = 0;
        private double _xMax = MaxTimeWidth;
        //private int TimeWidth = Convert.ToInt32(MaxTimeWidth);

        public ChartModel(Data DataStorage)
        {
            Series = new SeriesCollection();
            _dataStorage = DataStorage;

            for (int i = 0; i < DataStorage.ChannelsData.Count; i++)
            {
                LineSeries lineSeries = new LineSeries()
                {
                    Title = "CH_" + i.ToString(),
                    Values = new ChartValues<ObservablePoint>(),
                    Fill = Brushes.Transparent,
                    PointGeometry = null,
                    LineSmoothness = 0,
                    Width = 1
                };
                Series.Add(lineSeries);
            }
            _dataStorage.GetChannelData(7).CollectionChanged += DataStorageCollectionChangedHandler;
        }

        public void ChangeWindowStartPoint(double newStartPoint)
        {
            if (IsDrawing || _dataStorage.GetChannelLength() == 0)
                return;

            double MaxTime = _dataStorage.GetLastTime();
            if (newStartPoint < 0)
                newStartPoint = 0;
            if (newStartPoint + MaxTimeWidth > MaxTime)
                newStartPoint = MaxTime - MaxTimeWidth;

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
            PutAllDataToChart();
            XMin = 0;
            XMax = MaxTimeWidth;
            OnPropertyChanged(nameof(MaxStartTime));
        }

        private void AddRecentPoints()
        {
            CurrentTime = _dataStorage.GetLastTime();
            var WindowDataEdges = WindowingDataHelper.GetDataWindowIndex(CurrentTime, _dataStorage.GetChannelData(0));
            int leftEdge = WindowDataEdges.Item1, rightEdge = WindowDataEdges.Item2;

            AddWindowPointsToChart(leftEdge, rightEdge);
        }

        public void PutAllDataToChart()
        {
            int lastPointIndex = _dataStorage.GetChannelLastPointIndex();

            AddWindowPointsToChart(0, lastPointIndex);
        }

        private void AddWindowPointsToChart(int leftIndex, int rightIndex)
        {
            //if (leftIndex == rightIndex)
                //throw new ArgumentException("Left index equal right index");

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
                case NotifyCollectionChangedAction.Add:

                    AddRecentPoints();
                    break;
                case NotifyCollectionChangedAction.Reset:
                    ClearChannels();
                    break;
            }
        }
    }
}
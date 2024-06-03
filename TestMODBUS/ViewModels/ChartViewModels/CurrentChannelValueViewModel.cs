using LiveCharts.Wpf;
using ModBusTPU.ViewModels.Base;
using System.Collections.Specialized;
using System.Linq;
using System;
using LiveCharts.Definitions.Series;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows;

namespace ModBusTPU.ViewModels.ChartViewModels
{
    public class CurrentChannelValueViewModel : BaseViewModel
    {
        #region Public Properties

        public double ChannelValue => _value;

        public string ValueType => _valueType;

        public string Title { get => _title; }

        #endregion

        private string _title;
        private string _valueType;
        private double _value;
        private ISeriesView _series;

        public CurrentChannelValueViewModel(ISeriesView Series, string ValueType) 
        {
            if (Series == null)
                throw new ArgumentNullException(nameof(Series));

            _series = Series;
            _title = Series.Title;
            _valueType = ValueType;

            _series.Values.PropertyChanged += AddNewRangeHandler;
        }

        private void AddNewRangeHandler(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName == "Item[]" && _series.Values.Count > 0)
                {
                    Application.Current.Dispatcher.BeginInvoke(new Action(() => _value = _series.Values.GetPoints(_series).Last().Y));
                    OnPropertyChanged(nameof(ChannelValue));
                }
            }
            catch { }
        }
    }
}

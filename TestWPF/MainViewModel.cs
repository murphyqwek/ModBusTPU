using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace TestWPF
{
    public class MainViewModel
    {
        double i = 0;
        private LineSeries testLine;

        public ICommand addPointCommand { get; }

        private void AddPointCommandHandler()
        {
            for (int j = 0; j < 4800; j++)
            {
                testLine.Values.Add(new ObservablePoint(i, i * 2));
                i++;
            }
        }

        public SeriesCollection SeriesCollection { get; set; }

        public MainViewModel() 
        {
            testLine = new LineSeries()
            {
                Fill = new SolidColorBrush(color: System.Windows.Media.Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF)),
                Values = new ChartValues<ObservablePoint>(), 
                PointGeometry = null
            };

            SeriesCollection = new SeriesCollection()
            {
                testLine
            };

            addPointCommand = new Command(AddPointCommandHandler);
        }
    }
}

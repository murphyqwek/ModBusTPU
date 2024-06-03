using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ModBusTPU.Models.Services
{
    public static class ChannelSeriesColors
    {
        public static Dictionary<int, SolidColorBrush> Colors = new Dictionary<int, SolidColorBrush>()
        {
            {0, new SolidColorBrush(Color.FromRgb(245, 202, 12)) },
            {1, new SolidColorBrush(Color.FromRgb(241, 81, 67)) },
            {2, new SolidColorBrush(Color.FromRgb(119, 182, 68)) },
            {3, new SolidColorBrush(Color.FromRgb(240, 144, 58)) },
            {4, new SolidColorBrush(Color.FromRgb(52, 74, 150)) },
            {5, new SolidColorBrush(Color.FromRgb(151, 53, 68)) },
            {6, new SolidColorBrush(Color.FromRgb(136, 134, 119)) },
            {7, new SolidColorBrush(Color.FromRgb(58, 70, 58)) },
        };
    }
}

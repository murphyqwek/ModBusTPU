using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMODBUS.Models.Data;

namespace TestMODBUS.Models.Modbus
{
    public static class ModBusValueConverter
    {
        const double koeffValueChanne = 10.0 / 32768.0; //Коэффициент перевода данных в значение напряжение на выходе датчика
        const double VoltKoeff = 37.05881; //Коэффициент напряжения(получен экспериментально)
        const double AmperKoeff = 200; //Коэффициент перевода значения напряжение на выходе датчика в значение тока(нужно подобрать)
        const double HolostMove = 2.039; //Значение холостого хода с датчика
        

        public static double ConvertFromHexToDoubleFromChannelData(string ChannelData)
        {
            if(ChannelData == null)
                throw new ArgumentNullException(nameof(ChannelData));
            if (ChannelData.Length != 4)
                throw new ArgumentException("Channel Data must contains only 2 bytes");

            short Value = Convert.ToInt16("0x" + ChannelData, 16);
            double ConvertedValue = Value * koeffValueChanne;
            double RoundedValue = Math.Round(ConvertedValue, 3);
            return RoundedValue;
        }

        public static double ConvertHexToAmperValue(string ChannelData)
        {
            double Value = ConvertFromHexToDoubleFromChannelData(ChannelData);
            return ConvertToAmperValue(Value);
        }

        public static double ConvertToAmperValue(double Value) => Value * AmperKoeff;

        public static ObservableCollection<Point> ConvertCollectionToAmperValues(ObservableCollection<Point> ValueCollection)
        {
            ObservableCollection<Point> result = new ObservableCollection<Point>();
            foreach (var Point in ValueCollection) {
                double Value = ConvertToAmperValue(Point.Y);
                result.Add(new Point(Point.X, Value));
            }

            return result;
        }

        public static List<Point> ConvertListToAmperValues(List<Point> ValueList)
        {
            List<Point> result = new List<Point>();
            foreach (var Point in ValueList)
            {
                double Value = ConvertToAmperValue(Point.Y);
                result.Add(new Point(Point.X, Value));
            }

            return result;
        }

        public static double ConvertHexToVoltValue(string ChannelData)
        {
            double Value = Math.Abs(ConvertFromHexToDoubleFromChannelData(ChannelData));
            return ConvertToVoltValue(Value);
        }

        public static double ConvertToVoltValue(double Value) => (Value - HolostMove) * VoltKoeff;// / 16 * 580 * 1.022312;

        public static ObservableCollection<Point> ConvertCollectionToVoltValues(ObservableCollection<Point> ValueCollection)
        {
            ObservableCollection<Point> result = new ObservableCollection<Point>();
            foreach (var Point in ValueCollection)
            {
                double Value = ConvertToVoltValue(Point.Y);
                result.Add(new Point(Point.X, Value));
            }

            return result;
        }

        public static List<Point> ConvertListToVoltValues(List<Point> ValueList)
        {
            List<Point> result = new List<Point>();
            foreach (var Point in ValueList)
            {
                double Value = ConvertToVoltValue(Point.Y);
                result.Add(new Point(Point.X, Value));
            }

            return result;
        }
    }
}

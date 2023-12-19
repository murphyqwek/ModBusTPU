using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMODBUS.Models.Data;

namespace TestMODBUS.Models.ModbusCommands
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
            return ConvertFromHexToDoubleFromChannelData(ChannelData) * AmperKoeff;
        }


        public static double ConvertHexToVoltValue(string ChannelData)
        {
            return (Math.Abs(ConvertFromHexToDoubleFromChannelData(ChannelData)) - HolostMove) * VoltKoeff;// / 16 * 580 * 1.022312;
        }
    }
}

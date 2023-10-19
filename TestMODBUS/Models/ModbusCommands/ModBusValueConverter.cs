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
        const double koeffValueChanne = 10 / 32768;

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


    }
}

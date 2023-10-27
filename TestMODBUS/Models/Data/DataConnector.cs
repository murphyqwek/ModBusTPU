using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMODBUS.Models.ModbusCommands;
using static System.Net.Mime.MediaTypeNames;

namespace TestMODBUS.Models.Data
{
    public class DataConnector
    {
        private Data _data;

        public DataConnector(Data Data) 
        { 
            if(Data == null) 
                throw new ArgumentNullException(nameof(Data));

            _data = Data;
        }

        private double ParseData(byte[] data)
        {
            try
            {
                byte[] channelData = new byte[2];
                channelData[0] = data[3];
                channelData[1] = data[4];

                string ChannelDataString = BitConverter.ToString(channelData).Replace("-", "");

                return ModBusValueConverter.ConvertHexToAmperValue(ChannelDataString);
            }
            catch
            {
                return 0;
            }

        }

        public void SaveData(byte[][] ChannelsData, int Time)
        {
            for (int Channel = 0; Channel < _data.ChannelsData.Count; Channel++) {
                double x = Convert.ToDouble(Time);
                double y = ParseData(ChannelsData[Channel]);

                _data.AddNewPoint(new Point(x, y), Channel);
            }
        }
    }
}

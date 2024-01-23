using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMODBUS.Models.ModbusCommands;

namespace TestMODBUS.Models.Data
{
    //Используется для взаимодействия других модулей с хранилищем данных
    public class DataConnector
    {
        //Хранилще данных, в которое мы будем сохранять данные
        private Data _data;

        public DataConnector(Data Data) 
        { 
            if(Data == null) 
                throw new ArgumentNullException(nameof(Data));

            _data = Data;
        }

        //Функция переводит полученные байты в значение с датчиков
        private double ParseData(byte[] data, int channel)
        {
            try
            {
                byte[] channelData = new byte[2];
                channelData[0] = data[3]; //Вытаскиваем из всей команды байты с данными
                channelData[1] = data[4];
                
                string ChannelDataString = BitConverter.ToString(channelData).Replace("-", ""); //Переводим байты в hex и в строку

                if (channel == 0 || channel == 1 || channel == 2 || channel == 3) //Каналы тока
                    return ModBusValueConverter.ConvertHexToAmperValue(ChannelDataString);
                if (channel == 5 || channel == 6 || channel == 7) //Каналы напряжения
                    return ModBusValueConverter.ConvertHexToVoltValue(ChannelDataString);

                //Неиспользуемые каналы
                return ModBusValueConverter.ConvertFromHexToDoubleFromChannelData(ChannelDataString);
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
                double y = ParseData(ChannelsData[Channel], Channel);

                _data.AddNewPoint(new Point(x, y), Channel);
            }
        }
    }
}

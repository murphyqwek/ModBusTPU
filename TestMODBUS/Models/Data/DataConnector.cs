﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModBusTPU.Models.Modbus;
using ModBusTPU.Models.Services;

namespace ModBusTPU.Models.Data
{
    //Используется для взаимодействия других модулей с хранилищем данных
    public class DataConnector
    {
        //Хранилще данных, в которое мы будем сохранять данные
        private DataStorage _data;

        public DataConnector(DataStorage Data) 
        { 
            if(Data == null) 
                throw new ArgumentNullException(nameof(Data));

            _data = Data;
        }

        //Создает тестовые данные
        private double GetRandomDouble(Random rand)
        {
            return (double)rand.Next(-10, 300);
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

                return ModBusValueConverter.ConvertFromHexToDoubleFromChannelData(ChannelDataString);
            }
            catch
            {
                return 0;
            }

        }

        public void SaveData(byte[][] ChannelsData, int Time)
        {
            for (int Channel = 0; Channel < _data.GetMaxChannelsCount(); Channel++) {
                double x = Convert.ToDouble(Time);
                double y = ParseData(ChannelsData[Channel], Channel);

                _data.AddNewPoint(new Point(x, y), Channel);
            }
        }


        //Сохранение тестовых случайных данных
        public void SaveDataTest(int Time)
        {
            Random rand = new Random();
            for (int Channel = 0; Channel < _data.GetMaxChannelsCount(); Channel++)
            {
                double x = Convert.ToDouble(Time);

                //double y = GetRandomDouble(rand);

                double y;
                if (ChannelTypeList.GetChannelType(Channel) == ChannelType.Tok)
                    y = (double)rand.Next(270, 290) / 200;
                else if (ChannelTypeList.GetChannelType(Channel) == ChannelType.Volt)
                    y = (double)rand.Next(30, 45) / 37.05881234123 + 2.0391233421323;
                else
                    y = (double)rand.Next(30, 45) / 37.05881 + 2.039;

                _data.AddNewPoint(new Point(x, y), Channel);
            }
        }
    }
}

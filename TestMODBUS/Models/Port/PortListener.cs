using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TestMODBUS.Models.Data;
using TestMODBUS.Models.INotifyPropertyBased;
using TestMODBUS.Models.MessageBoxes;
using TestMODBUS.Models.ModbusCommands;

namespace TestMODBUS.Models.Port
{
    //Класс, который считывает данные с порт и сохраняет черз DataConnector в хранилище данных новые данные
    public class PortListener : INotifyBase
    {
        private ObservablePort _port = new ObservablePort();
        private Thread listenningThread;
        private DataConnector _connector;

        public PortListener(ObservablePort Port, DataConnector Connector)
        {
            if(Port == null)
                throw new ArgumentNullException(nameof(Port));
            
            if(Connector == null)
                throw new ArgumentNullException(nameof(Connector));

            _port = Port;
            _connector = Connector;
        }

        public void StartListen(int delay)
        {
            if(_port == null)
                throw new ArgumentNullException(nameof(Port));

            _port.Open();

            // Проверка на модуль (пока не нужно)

            //Запуск потока
            listenningThread = new Thread(() => Listen(_port, _connector, delay));
            listenningThread.Start();
        }

        public void StopListen()
        {
            if (_port != null)
                _port.Close();
        }

        private void Listen(ObservablePort Port, DataConnector Connector, int delay)
        {
            if (delay <= 100)
                throw new ArgumentException("Delay must be more than 50 milliseconds");

            Stopwatch timer = new Stopwatch();
            timer.Start();
            const int measureTime = 50; //Это время, за котрое программа считает все данные со всех каналов. Пока подбирается вручную
            delay = delay - measureTime; //Таким образом мы учитываем время на считывания, и промежутки измерения будут примерно такими же, какими их задал пользователь

            while (Port.IsPortOpen)
            {
                try
                {
                    int time = Convert.ToInt32(timer.ElapsedMilliseconds);
                    byte[][] ChannelData = new byte[8][];
                    for (int channel = 0; channel < 8; channel++)
                    {
                        byte[] sendCommand = ModBusCommandsList.GetReadChannelCommand(channel); //Получаем команду, чтобы считать данные с конкретного канала
                        Port.Send(sendCommand);

                        var recieved = Port.ReadCommand();

                        if (recieved == null)
                            return;

                        ChannelData[channel] = recieved;
                    }
                    Connector.SaveData(ChannelData, time); //Сохраняем данные в хранилище через промежуточный класс-конектор
                    Thread.Sleep(delay);
                }
                catch(System.IO.IOException)
                {
                    ErrorMessageBox.Show("Возникли проблемы с портом. Проверьте соединение");
                    if (_port != null)
                        _port.Close();
                    break;
                }
            }
        }

    }
}
using System;
using System.Collections.Generic;
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

            if (!_port.Open())
                return;

            // Проверка на модуль

            //Запуск потока
            listenningThread = new Thread(() => Listen(_port, _connector, delay));
            listenningThread.Start();
        }

        private void Listen(ObservablePort Port, DataConnector Connector, int delay = 100)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            Random random = new Random();
            while (Port.IsPortOpen)
            {
                try
                {
                    int time = Convert.ToInt32(timer.ElapsedMilliseconds);
                    byte[][] ChannelData = new byte[8][];
                    for (int channel = 0; channel < 8; channel++)
                    {
                        byte[] sendCommand = ModBusCommandsList.GetReadChannelCommand(channel);
                        Port.Send(sendCommand);

                        var recieved = Port.ReadAll();

                        if (recieved == null)
                            return;

                        //var recieved = random.Next(0, 50).ToString();
                        ChannelData[channel] = recieved;
                    }
                    Connector.SaveData(ChannelData, time);
                    Thread.Sleep(delay);
                }
                catch(System.IO.IOException)
                {
                    if (_port != null)
                        _port.Close();
                    break;
                }
            }
        }

        public void StopListen()
        {
            if(_port != null)
                _port.Close();
        }
    }
}

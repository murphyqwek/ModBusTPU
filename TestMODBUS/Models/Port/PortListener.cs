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
using ModBusTPU.Models.Data;
using ModBusTPU.Models.INotifyPropertyBased;
using ModBusTPU.Models.MessageBoxes;
using ModBusTPU.Models.Modbus;
using ModBusTPU.Models.Port.Interfaces;

namespace ModBusTPU.Models.Services
{
    //Класс, который считывает данные с порт и сохраняет черз DataConnector в хранилище данных новые данные
    public class PortListener : INotifyBase, IPortListener
    {
        private ObservablePort _port = new ObservablePort();
        private Thread listenningThread;
        private DataConnector _connector;
        private Action stopByErrorAction; //Событие, когда порт закрывается из-за System.IO Exception

        public PortListener(ObservablePort Port, DataConnector Connector, Action StopByErrorAction)
        {
            if (Port == null)
                throw new ArgumentNullException(nameof(Port));
            
            if(Connector == null)
                throw new ArgumentNullException(nameof(Connector));

            if (StopByErrorAction == null)
                throw new ArgumentException(nameof(StopByErrorAction));

            stopByErrorAction = StopByErrorAction;
            _port = Port;
            _connector = Connector;
        }

        public void StartListen(int delay, byte deviceAdress)
        {
            if(_port == null)
                throw new ArgumentNullException(nameof(Services));

            _port.Open();

            // Проверка на модуль (пока не нужно)

            //Запуск потока
            listenningThread = new Thread(() => Listen(_port, _connector, delay, deviceAdress));
            listenningThread.Start();
        }

        public void StopListen()
        {
            if (_port != null)
                _port.Close();
        }

        private void Listen(ObservablePort Port, DataConnector Connector, int delay, byte deviceAdress)
        {
            const int measureTime = 200; //Это время, за котрое программа считает все данные со всех каналов. Пока подбирается вручную
            if (delay < measureTime + 100)
                throw new ArgumentException("Delay is too short");

            Stopwatch timer = new Stopwatch();
            timer.Start();
            delay = delay - measureTime; //Таким образом мы учитываем время на считывания, и промежутки измерения будут примерно такими же, какими их задал пользователь

            while (Port.IsPortOpen)
            {
                try
                {
                    int time = Convert.ToInt32(timer.ElapsedMilliseconds);
                    byte[][] ChannelData = new byte[8][];

                    //ВАЖНО: каналы должны считываться строго по порядку от 0 до последнего
                    //Чарт обновляет точки тогда, когда обновился последний канал
                    for (int channel = 0; channel < 8; channel++)
                    {
                        byte[] sendCommand = ModBusCommandsList.GetReadChannelCommand(channel, deviceAdress); //Получаем команду, чтобы считать данные с конкретного канала
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
                    if (_port != null)
                        _port.Close();
                    stopByErrorAction?.Invoke();
                    break;
                }
            }
        }

    }
}
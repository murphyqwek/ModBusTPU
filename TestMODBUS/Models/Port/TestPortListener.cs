using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestMODBUS.Models.Data;
using TestMODBUS.Models.INotifyPropertyBased;
using TestMODBUS.Models.Modbus;
using TestMODBUS.Models.Port.Interfaces;

namespace TestMODBUS.Models.Services
{
    //Тестовый класс для имитации работы настоящего прибора
    public class TestPortListener : INotifyBase, IPortListener
    {
        private Thread _listenningThread;
        private bool _isListenning = false;
        private DataConnector _connector;

        public TestPortListener(DataConnector Connector)
        {
            _connector = Connector;
        }

        public void StartListen(int delay)
        {
            _isListenning = true;
            _listenningThread = new Thread(() => Listen(_connector, delay));
            _listenningThread.Start();
        }

        public void StopListen()
        {
            _isListenning = false;
        }

        private void Listen(DataConnector Connector, int delay)
        {
            const int measureTime = 200; //Это время, за котрое программа считает все данные со всех каналов. Пока подбирается вручную
            if (delay < measureTime + 100)
                throw new ArgumentException("Delay is too short");

            Stopwatch timer = new Stopwatch();
            timer.Start();
            delay = delay - measureTime; //Таким образом мы учитываем время на считывания, и промежутки измерения будут примерно такими же, какими их задал пользователь

            while (_isListenning)
            {
                int time = Convert.ToInt32(timer.ElapsedMilliseconds);
                Connector.SaveDataTest(time); //Сохраняем данные в хранилище через промежуточный класс-конектор
                Thread.Sleep(delay);
            }
        }
    }
}

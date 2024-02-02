using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestMODBUS.Exceptions;
using TestMODBUS.Models.INotifyPropertyBased;
using TestMODBUS.Models.MessageBoxes;

namespace TestMODBUS.Models.Channels
{

    //Обёртка стандартного класса SerialPort
    public class ObservablePort : INotifyBase
    {
        #region Public Attributes
        public string PortName
        {
            get => _portName;
            set
            {
                _portName = value;
                _port.PortName = _portName == null ? "COM1" : _portName;
                OnPropertyChanged();
            }
        }

        public int PortSpeed
        {
            get => _portSpeed;
            private set
            {
                _portSpeed = value;
                _port.BaudRate = _portSpeed;
                OnPropertyChanged();
            }
        }

        public bool IsPortOpen
        {
            get => _isPortOpen;
            private set
            {
                _isPortOpen = value;
                OnPropertyChanged();
            }
        }
        #endregion

        private SerialPort _port = new SerialPort();
        private string _portName;
        private int _portSpeed;
        private bool _isPortOpen = false;


        public ObservablePort()
        {
            PortSpeed = ListAvailableSpeeds.GetStandartSpeed();
            PortName = ListAvailablePorts.GetFirstAvailablePort();
        }

        public void SetPortSpeed(int portSpeed)
        {
            PortSpeed = portSpeed;
        }

        public void SetPortName(string portName)
        {
            if (portName == ListAvailablePorts.NoAvaiblePortsString)
                PortName = null;
            else
                PortName = portName;
        }

        public bool Open()
        {
            if (!ListAvailablePorts.IsAnyPortAvailable)
                throw new NoPortAvailableException();

            try
            {
                _port.Open();
            }
            catch (System.IO.IOException)
            {
                SetPortName(null);
                throw new ChosenPortUnavailableException(_portName);
            }

            IsPortOpen = true;
            return true;
        }

        public void Close()
        {
            _port.Close();
            IsPortOpen = false;
        }

        public void Send(byte[] data)
        {
            if (!_port.IsOpen)
                return;

            _port.Write(data, 0, data.Length);
        }

        public void Send(string data)
        {
            if (!_port.IsOpen) 
                return;

            _port.Write(data);
        }

        public byte[] ReadCommand()
        {
            int whileBreakerCount = 0; //Специальный счётчик, чтобы избежать бесконечного цикла
            int bytes;
            do
            {
                try
                {
                    Thread.Sleep(5);
                    bytes = _port.BytesToRead;
                    whileBreakerCount++;
                }
                catch
                {
                    return null;
                }
            }
            while (bytes != 7 && _isPortOpen && whileBreakerCount < 100);

            //Количество байтов задано вручную. На будущее: для универсального считывания нужно считывать байты, пока последний байт не будет равен контрольной сумме всей команды

            byte[] buffer = new byte[bytes];
            _port.Read(buffer, 0, bytes);

            _port.DiscardInBuffer(); //Очищаем буффер
            return buffer;
        }
    }
}

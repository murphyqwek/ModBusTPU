using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMODBUS.Models.INotifyPropertyBased;
using TestMODBUS.Models.MessageBoxes;

namespace TestMODBUS.Models.Port
{
    public class ObservablePort : INotifyBase
    {
        private SerialPort _port = new SerialPort();

        private string _portName;
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

        private int _portSpeed;
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

        private bool _isPortOpen = false;
        public bool IsPortOpen
        {
            get => _isPortOpen;
            private set
            {
                _isPortOpen = value;
                OnPropertyChanged();
            }
        }

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
            {
                ErrorMessageBox.Show("Порт не выбран");
                return false;
            }

            try
            {
                _port.Open();
            }
            catch (System.IO.IOException)
            {
                ErrorMessageBox.Show(string.Format("Порт {0} не существтует. Выберите другой", _port.PortName));
                ListAvailablePorts.UpdateAvailablePortList();
                SetPortName(null);
                return false;
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

        public string ReadAll()
        {
            if (!_port.IsOpen)
                return null;

            var recieved = _port.ReadExisting();

            _port.DiscardInBuffer();
            return recieved;
        }
    }
}

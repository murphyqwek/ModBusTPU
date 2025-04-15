using ModBusTPU.Services.Settings.SettingsContainer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ModBusTPU.Services.Settings.MotorSettings
{
    public class MotorSettingsContainer : BaseSettingsContainer, INotifyPropertyChanged
    {
        private string _port = "COM9";
        private int _baudrate = 9600;
        private byte _address = 1;

        private double _currentThreshold = 125; // Уставка по току
        private double _voltageThreshold = 40; // Уставка по напряжению

        private double _highCurrentThresholdBound = 15;
        private double _lowCurrentThresholdBound = 5;

        private double _highVoltageThresholdBound = 5;
        private double _lowVoltageThresholdBound = 5;

        private int _kzdelay = 5000;

        private int _writedelay = 20;
        private int _iterationalDelay = 30;
        private int _reverseDelay = 3000;
        private ushort _reverseSpeed = 500;
        private ushort _speed = 100;



        public string PORT { get => _port; set { _port = value; OnPropertyChanged(nameof(PORT)); } }

        public int BAUDRATE { get => _baudrate; set { _baudrate = value; OnPropertyChanged(nameof(BAUDRATE)); } }
        public byte ADDRESS { get => _address; set { _address = value; OnPropertyChanged(nameof(ADDRESS)); } }

        public double currentThreshold { get => _currentThreshold; set { _currentThreshold = value; OnPropertyChanged(nameof(currentThreshold)); } } // Уставка по току
        public double voltageThreshold { get => voltageThreshold; set { _voltageThreshold = value; OnPropertyChanged(nameof(_voltageThreshold)); } } // Уставка по напряжению

        public double highCurrentThresholdBound { get => _highCurrentThresholdBound; set { _highCurrentThresholdBound = value; OnPropertyChanged(nameof(highCurrentThresholdBound)); } }
        public double lowCurrentThresholdBound { get => _lowCurrentThresholdBound; set { _lowCurrentThresholdBound = value; OnPropertyChanged(nameof(lowCurrentThresholdBound)); } }

        public double highVoltageThresholdBound { get => _highVoltageThresholdBound; set { _highVoltageThresholdBound = value; OnPropertyChanged(nameof(highVoltageThresholdBound)); } }
        public double lowVoltageThresholdBound { get => _lowVoltageThresholdBound; set { _lowVoltageThresholdBound = value; OnPropertyChanged(nameof(lowVoltageThresholdBound)); } }

        public int KZDELAY { get => _kzdelay; set { _kzdelay = value; OnPropertyChanged(nameof(KZDELAY)); } }

        public int WRITEDELAY { get => _writedelay; set { _writedelay = value; OnPropertyChanged(nameof(KZDELAY)); } }
        public int ITERATIONDELAY { get => _iterationalDelay; set { _iterationalDelay = value; OnPropertyChanged(nameof(ITERATIONDELAY)); } }
        public int REVERSDELAY { get => _reverseDelay; set { _reverseDelay = value; OnPropertyChanged(nameof(REVERSDELAY)); } }
        public ushort REVERSESPEED { get => _reverseSpeed; set { _reverseSpeed = value; OnPropertyChanged(nameof(REVERSESPEED)); } }
        public ushort SPEED { get => _speed; set { _speed = value; OnPropertyChanged(nameof(SPEED)); } }

        public event PropertyChangedEventHandler PropertyChanged;

        public override void EnsureDefualt()
        {
            
        }

        public override bool isValid()
        {
            return true;
        }

        public override void SetDefaultSettingsContainer()
        {
            
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

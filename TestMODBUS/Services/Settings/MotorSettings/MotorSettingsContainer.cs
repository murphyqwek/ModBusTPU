using ModBusTPU.Services.Settings.SettingsContainer;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBusTPU.Services.Settings.MotorSettings
{
    internal class MotorSettingsContainer : BaseSettingsContainer
    {
        public string PORT { get; set; } = "COM9";
        public int BAUDRATE { get; set; } = 9600;
        public byte ADDRESS { get; set; } = 1;

        public double currentThreshold { get; set; } = 125; // Уставка по току
        public double voltageThreshold { get; set; } = 40; // Уставка по напряжению

        public double highCurrentThresholdBound { get; set; } = 15;
        public double lowCurrentThresholdBound { get; set; } = 5;

        public double highVoltageThresholdBound { get; set; } = 5;
        public double lowVoltageThresholdBound { get; set; } = 5;

        public int KZDELAY { get; set; } = 5000;

        public int WRITEDELAY { get; set; } = 20;
        public int ITERATIONDELAY { get; set; } = 30;
        public int REVERSDELAY { get; set; } = 3000;
        public ushort REVERSESPEED { get; set; } = 500;
        public ushort SPEED { get; set; } = 100;

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
    }
}

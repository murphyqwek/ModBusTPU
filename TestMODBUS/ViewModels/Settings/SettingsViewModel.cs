using ModBusTPU.Models.Data;
using ModBusTPU.Models.MessageBoxes;
using ModBusTPU.Models.Services.Settings.Data;
using ModBusTPU.Models.Services;
using ModBusTPU.Services.Settings.MotorSettings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;

namespace ModBusTPU.ViewModels.Settings
{
    public class SettingsViewModel
    {
        public ChannelsTypeChoosingViewModel ChannelsTypeChoosingViewModel { get; }
        public CoefficientProfileSettingsViewModel CoefficientProfileSettingsViewModel { get; }
        public MotorSettingsViewModel MotorSettingsViewModel { get; }

        public SettingsViewModel(MotorSettingsContainer motorSettings) 
        { 
            ChannelsTypeChoosingViewModel = new ChannelsTypeChoosingViewModel();
            CoefficientProfileSettingsViewModel = new CoefficientProfileSettingsViewModel();
            MotorSettingsViewModel = new MotorSettingsViewModel(motorSettings);
        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            ChannelsTypeChoosingViewModel.OnWindowClosing(sender, e);
            if (e.Cancel == true)
                return;
        }
    }
}

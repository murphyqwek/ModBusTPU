using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBusTPU.ViewModels.Settings
{
    public class SettingsViewModel
    {
        public ChannelsTypeChoosingViewModel ChannelsTypeChoosingViewModel { get; }
        public CoefficientProfileSettingsViewModel CoefficientProfileSettingsViewModel { get; }
        public SettingsViewModel() 
        { 
            ChannelsTypeChoosingViewModel = new ChannelsTypeChoosingViewModel();
            CoefficientProfileSettingsViewModel = new CoefficientProfileSettingsViewModel();
        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            ChannelsTypeChoosingViewModel.OnWindowClosing(sender, e);
            if (e.Cancel == true)
                return;
        }
    }
}

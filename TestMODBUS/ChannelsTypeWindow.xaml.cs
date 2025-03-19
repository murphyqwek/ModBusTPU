using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ModBusTPU.Models.Services;
using ModBusTPU.Properties;
using ModBusTPU.Services.Settings.MotorSettings;
using ModBusTPU.ViewModels;
using ModBusTPU.ViewModels.Settings;

namespace ModBusTPU.Views
{
    /// <summary>
    /// Логика взаимодействия для ChannelsTypeWindow.xaml
    /// </summary>
    public partial class ChannelsTypeWindow : Window
    {
        public ChannelsTypeWindow(MotorSettingsContainer motorSettingsContainer)
        {
            InitializeComponent();
            var DataContext = new SettingsViewModel(motorSettingsContainer);
            this.DataContext = DataContext;
            this.Closing += DataContext.OnWindowClosing;
        }

        private void PortComboBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListAvailablePorts.UpdateAvailablePortList();
        }
    }
}

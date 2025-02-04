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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ModBusTPU.Models.Services;
using ModBusTPU.Models.Services.Excel;
using ModBusTPU.Services.Settings.Channels;
using ModBusTPU.ViewModels;

namespace ModBusTPU
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            ChannelsSettingFileManager.UploadDefaultSettings();
            ExcelExport.SetUp();
            ListAvailablePorts.UpdateAvailablePortList();
            InitializeComponent();
            MainViewModel viewModel = new MainViewModel();
            this.DataContext = viewModel;
            this.Closing += viewModel.OnWindowClosing;
        }

        private void PortComboBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListAvailablePorts.UpdateAvailablePortList();
        }
    }
}

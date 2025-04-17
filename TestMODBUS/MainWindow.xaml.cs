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
        private MainViewModel mainViewModel = new MainViewModel();

        public MainWindow()
        {
            ChannelsSettingFileManager.UploadDefaultSettings();
            ExcelExport.SetUp();
            InitializeComponent();
            MainViewModel viewModel = new MainViewModel();
            mainViewModel = viewModel;
            mainViewModel.ListPorts.UpdateAvailablePortList();
            this.DataContext = viewModel;
            this.Closing += viewModel.OnWindowClosing;
        }

        private void PortComboBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mainViewModel.ListPorts.UpdateAvailablePortList();
        }
    }
}

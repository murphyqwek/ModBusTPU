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
using ModBusTPU.Models.Data;
using ModBusTPU.ViewModels;
using ModBusTPU.ViewModels.ExportViewModels;

namespace ModBusTPU
{
    /// <summary>
    /// Логика взаимодействия для ExportWindow.xaml
    /// </summary>
    public partial class ExportWindow : Window
    {
        public ExportWindow(DataStorage Data)
        {
            InitializeComponent();
            ExportViewModel viewModel = new ExportViewModel(Data);

            this.DataContext = viewModel;
        }
    }
}

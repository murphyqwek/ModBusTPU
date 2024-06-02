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
using TestMODBUS.Models.Data;
using TestMODBUS.ViewModels;
using TestMODBUS.ViewModels.ExportViewModels;

namespace TestMODBUS
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

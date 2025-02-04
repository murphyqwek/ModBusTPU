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
        ExportViewModel _model;

        public ExportWindow(ExportViewModel Model)
        {
            InitializeComponent();

            this.DataContext = Model;
            _model = Model;

            Closing += ExportWindow_Closing;
            Loaded += ExportWindow_Loaded;
        }

        private void ExportWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _model.WhenOpen();
        }

        private void ExportWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = _model.BeforeClosing();
        }
    }
}

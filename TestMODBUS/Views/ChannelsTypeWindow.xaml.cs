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
using TestMODBUS.ViewModels;

namespace TestMODBUS.Views
{
    /// <summary>
    /// Логика взаимодействия для ChannelsTypeWindow.xaml
    /// </summary>
    public partial class ChannelsTypeWindow : Window
    {
        public ChannelsTypeWindow()
        {
            InitializeComponent();
            var DataContext = new ChannelsTypeChoosingViewModel();
            this.DataContext = DataContext;
            this.Closing += DataContext.OnWindowClosing;
        }
    }
}

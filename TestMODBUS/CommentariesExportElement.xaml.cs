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
using TestMODBUS.Services.Settings;

namespace ModBusTPU.UserControls
{
    /// <summary>
    /// Логика взаимодействия для CommentariesExportElement.xaml
    /// </summary>
    public partial class CommentariesExportElement : UserControl
    {
        public CommentariesExportElement()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = ForbiddenSymbols.CheckLabelForForbiddenSymbols(e.Text);

            if (!e.Handled)
                return;

            var tooltip = new ToolTip { Content = $"Символы: {string.Join(", ", ForbiddenSymbols.LabelsForbiddenSymbols)} - запрещены" };

            this.Label.ToolTip = tooltip;

            tooltip.Opened += async delegate (object o, RoutedEventArgs args)
            {
                var s = o as ToolTip;
                await Task.Delay(1000);
                s.IsOpen = false;
            };
            tooltip.IsOpen = true;
        }
    }
}

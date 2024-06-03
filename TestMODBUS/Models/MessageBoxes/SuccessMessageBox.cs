using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ModBusTPU.Models.MessageBoxes
{
    public static class SuccessMessageBox
    {
        public static void Show(string message, string Title = "Успешно")
        {
            MessageBox.Show(message, Title, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
        }
    }
}

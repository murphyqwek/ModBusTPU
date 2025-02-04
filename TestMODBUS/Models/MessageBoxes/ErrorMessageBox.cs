using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ModBusTPU.Models.MessageBoxes
{
    public static class ErrorMessageBox
    {
        public static void Show(string ErrorMessage)
        {
            MessageBox.Show(ErrorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}

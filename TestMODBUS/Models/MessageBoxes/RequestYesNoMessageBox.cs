using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ModBusTPU.Models.MessageBoxes
{
    static public class RequestYesNoMessageBox
    {
        static public MessageBoxResult Show(string Message, string Title = "Внимание", MessageBoxImage Icon = MessageBoxImage.Exclamation) 
        {
            return MessageBox.Show(Message, Title, MessageBoxButton.YesNoCancel, Icon);
        }
    }
}

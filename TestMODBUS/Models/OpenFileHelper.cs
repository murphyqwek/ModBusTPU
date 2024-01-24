using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMODBUS.Models
{
    public static class OpenFileHelper
    {
        public static string GetSaveFile(string FileName = "Отчёт")
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Title = "Сохранить файл как...";
            sf.FileName = FileName;
            sf.Filter = "*.xlsx|*.xlsx;";
            sf.DefaultExt = ".xlsx";

            if (sf.ShowDialog() == true)
            {
                return sf.FileName;
            }
            else
            {
                return null;
            }
            
        }
    }
}

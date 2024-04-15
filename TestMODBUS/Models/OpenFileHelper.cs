using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

        public static string GetFilePath(string Filter, string DefalutExtension)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Title = "Открыть...";
            of.Filter = Filter;
            of.DefaultExt = DefalutExtension;
            of.Multiselect = false;

            if (of.ShowDialog() == true)
            {
                return of.FileName;
            }
            else
            {
                return null;
            }
        }

        public static bool isFileOpen(string FilePath)
        {
            if (!File.Exists(FilePath))
                return false;

            StreamReader reader;
            try
            {
                reader = new StreamReader(FilePath);
                reader.Close();
                return false;
            }
            catch (IOException)
            {
                return true;
            }
        }
    }
}

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMODBUS.Models.Services
{
    public static class FileHelper
    {
        public static string GetSaveFilePath(string Filter, string DefaultExt, string FileName = "")
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Title = "Сохранить файл как...";
            sf.FileName = FileName;
            sf.Filter = Filter;
            sf.DefaultExt = DefaultExt;

            if (sf.ShowDialog() == true)
            {
                return sf.FileName;
            }
            else
            {
                return null;
            }
            
        }

        public static string GetOpenFilePath(string Filter, string DefalutExtension)
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

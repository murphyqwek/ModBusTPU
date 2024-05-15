using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.TextFormatting;
using TestMODBUS.Models.Data;
using TestMODBUS.Models.MessageBoxes;

namespace TestMODBUS.Models.Services.Settings.Data
{
    static class DataFileManager
    {
        private static readonly string DataLogPATH = Path.Combine(Environment.GetFolderPath(
                                                                  Environment.SpecialFolder.ApplicationData), "ModBus", "Logs");

        private static readonly string DataLogEXTENSION = ".svmb";

        private static void CreateFolder()
        {
            if(!Directory.Exists(DataLogPATH))
                Directory.CreateDirectory(DataLogPATH);
        }

        public static void SaveLogs(string DataLog)
        {
            CreateFolder();

            string FileName = DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss") + DataLogEXTENSION;

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(DataLogPATH, FileName), false))
            {
                outputFile.WriteLine(DataLog);
            }
        }

        public static void SaveLogs(DataStorage DataStorage)
        {
            var DataLogs = DataLog.GetLog(DataStorage);

            SaveLogs(DataLogs);
        }

        public static DataStorage ReadLog()
        {
            var Path = OpenFileHelper.GetOpenFilePath($"*{DataLogEXTENSION}|*{DataLogEXTENSION};", DataLogEXTENSION);
            if (string.IsNullOrEmpty(Path))
                return null;

            return ReadLog(Path);
        }

        public static DataStorage ReadLog(string LogPath)
        {
            if (!File.Exists(LogPath))
            {
                ErrorMessageBox.Show("Файл не найден");
                return null;
            }

            try
            {
                string text = File.ReadAllText(LogPath);
            }
            catch
            {
                ErrorMessageBox.Show("Невозможно прочитать файл");
                return null;
            }

            try
            {
                string LogText = File.ReadAllText(LogPath);
                var DataStorage = DataLog.ReadLog(LogText);
                return DataStorage;
            }
            catch(Exception ex)
            {
                ErrorMessageBox.Show(ex.Message);
                return null;
            }
        }
    }
}

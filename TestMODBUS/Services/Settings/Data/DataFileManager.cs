using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.TextFormatting;
using ModBusTPU.Models.Data;
using ModBusTPU.Models.MessageBoxes;

namespace ModBusTPU.Models.Services.Settings.Data
{
    static class DataFileManager
    {
        private static readonly string DataLogPATH = Path.Combine(Environment.GetFolderPath(
                                                                  Environment.SpecialFolder.ApplicationData), "ModBus", "Logs");

        public static readonly string DataLogEXTENSION = ".svmb";

        private static void CreateFolder()
        {
            if(!Directory.Exists(DataLogPATH))
                Directory.CreateDirectory(DataLogPATH);
        }

        public static void SaveLogs(string DataLog)
        {
            CreateFolder();

            string FilePath = DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss") + DataLogEXTENSION;

            SaveLogs(DataLog, FilePath);
        }

        private static void SaveLogs(string DataLog, string FilePath)
        {
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(DataLogPATH, FilePath), false))
            {
                outputFile.WriteLine(DataLog);
            }
        }

        public static void SaveLogs(DataStorage DataStorage)
        {
            var DataLogs = DataLog.GetLog(DataStorage);

            SaveLogs(DataLogs);
        }

        public static void SaveLogs(DataStorage DataStorage, string FilePath)
        {
            var DataLogs = DataLog.GetLog(DataStorage);

            SaveLogs(DataLogs, FilePath);
        }

        public static DataStorage ReadLog()
        {
            var Path = FileHelper.GetOpenFilePath($"*{DataLogEXTENSION}|*{DataLogEXTENSION};", DataLogEXTENSION);
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

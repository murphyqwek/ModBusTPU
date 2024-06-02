using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMODBUS.Models.Services;

namespace TestMODBUS.Services.Settings.Export
{
    public static class ExportSettingsManager
    {
        const string RegistryFolder = "Export";
        const string ExportFieldLabel = "Export Path";

        public static bool SaveSettings(ExportSettings ExportSettings)
        {
            string FilePath = FileHelper.GetSaveFilePath("*.txt|*.txt;", ".txt");

            if(FilePath == null)
                return false;

            string data = ExportSaving.GetData(ExportSettings);

            File.WriteAllText(FilePath, data);

            SetField(FilePath);
            return true;
        }

        public static ExportSettings UploadExportSettings()
        {
            string FilePath = FileHelper.GetOpenFilePath("*.txt|*.txt;", ".txt");
            if (FilePath == null)
                return null;

            var ExportSettings = UploadExportSettings(FilePath);

            SetField(FilePath);

            return ExportSettings;
        }

        public static ExportSettings UploadExportSettings(string FilePath)
        {
            string data = File.ReadAllText(FilePath);

            return ExportReading.ReadFile(data);
        }

        public static ExportSettings GetStandartExportSettings()
        {
            string Path = GetField();

            if (string.IsNullOrEmpty(Path))
                return null;

            try
            {
                var Settings = UploadExportSettings(Path);
                return Settings;
            }
            catch(Exception ex)
            {
                SetField("");
                throw ex;
            }               
        }

        private static void SetField(string FilePath) => RegisrtyService.SetField(RegistryFolder, ExportFieldLabel, FilePath);
        private static string GetField() => (string)RegisrtyService.GetField(RegistryFolder, ExportFieldLabel, true);
    }
}

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModBusTPU.Models.Data;
using ModBusTPU.Models.Services;
using ModBusTPU.ViewModels;

namespace ModBusTPU.Services.Settings.Export
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

        public static ExportSettings UploadStandartSettingsOrGetStandart()
        {
            ExportSettings _exportSettings;
            try
            {
                _exportSettings = GetStandartExportSettings();
            }
            catch
            {
                _exportSettings = GetStandartSettings();
            }

            if (_exportSettings == null)
                _exportSettings = GetStandartSettings();

            return _exportSettings;
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

        public static ExportSettings GetStandartSettings()
        {
            List<ChannelData> channels = new List<ChannelData>();

            for(int i = 0; i < DataStorage.MaxChannelCount; i++)
            {
                channels.Add(new ChannelData() { Channel = i, IsChosen = false, Label = $"CH_{i}" });
            }

            return new ExportSettings(channels, new List<ExtraData>(), new List<ExtraData>(), new List<Commentary>());
        }

        private static void SetField(string FilePath) => RegisrtyService.SetField(RegistryFolder, ExportFieldLabel, FilePath);
        private static string GetField() => (string)RegisrtyService.GetField(RegistryFolder, ExportFieldLabel, true);
    }
}

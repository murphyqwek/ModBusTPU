using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMODBUS.Models;
using TestMODBUS.Models.MessageBoxes;
using TestMODBUS.Models.Services;

namespace TestMODBUS.Services.Settings.Channels
{
    public static class ChannelsSettingFileManager
    {
        private const string FileEXTENSION = ".chnls";
        private static readonly string ChannelsSettignsPATH = Path.Combine(Environment.GetFolderPath(
                                                                  Environment.SpecialFolder.ApplicationData), "ModBus", "Channels Settings");
        private const string RegistryFOLDER = "channels";
        private const string DefaultFileFIELDNAME = "Default File";

        public static void SetDefaultSettings()
        {
            string FilePath = GetDefaultFileSettingsRegistry();

            if (string.IsNullOrEmpty(FilePath))
                ChannelsTypeSettings.SetStandartChannelsType();
            else
                SetUserSettings(FilePath, true);
        }

        public static void UploadUserSettings()
        {
            string FilePath = OpenFileHelper.GetFilePath($"*.{FileEXTENSION}|*.{FileEXTENSION};", $".{FileEXTENSION}");

            SetUserSettings(FilePath , true);
        }

        public static void UploadDefaultSettings()
        {
            string FilePath = GetDefaultFileSettingsRegistry();

            SetUserSettings(FilePath, false);
        }

        private static void SetUserSettings(string FilePath, bool SetItAsDefault)
        {
            string Settings = File.ReadAllText(FilePath);

            if (!ChannelsTypeSettings.SetUserChannelsType(Settings))
            {
                ErrorMessageBox.Show("Не удалось загрузить настройки каналов");
                ChannelsTypeSettings.SetStandartChannelsType();
                SetDefalutFileSettingsRegisrty("");
            }
            else
            {
                if (SetItAsDefault)
                    SetDefalutFileSettingsRegisrty(FilePath);
                SuccessMessageBox.Show("Настройки каналов успешно загружены");
            }
        }

        public static void SaveSettings()
        {
            string Filename = OpenFileHelper.GetSaveFile($"*.{FileEXTENSION}|*.{FileEXTENSION};", $".{FileEXTENSION}");
            if (Filename == null)
                return;

            SaveSettings(Filename);
        }

        private static void SaveSettings(string Filepath)
        {
            string Settings = ChannelsTypeSettings.GetChannelsTypeSettings();

            using (StreamWriter outputFile = new StreamWriter(Filepath, false))
            {
                outputFile.WriteLine(Settings);
            }

            SetDefalutFileSettingsRegisrty(Filepath);
        }

        private static void SetDefalutFileSettingsRegisrty(string FilePath)
        {
            RegisrtyService.SetField(RegistryFOLDER, DefaultFileFIELDNAME, FilePath);
        }

        private static string GetDefaultFileSettingsRegistry()
        {
            return (string)RegisrtyService.GetField(RegistryFOLDER, DefaultFileFIELDNAME, true);
        }
    }
}

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

        public static void UploadDefaultSettings()
        {
            string FilePath = GetDefaultFileSettingsRegistry();

            if (string.IsNullOrEmpty(FilePath))
            {
                ChannelsTypeSettings.SetStandartChannelsType();
                return;
            }

            try
            {
                SetUserSettings(FilePath, true);
            }
            catch
            {
                SetDefalutFileSettingsRegisrty("");
                ChannelsTypeSettings.SetStandartChannelsType();
            }
        }

        public static bool UploadUserSettings()
        {
            string FilePath = OpenFileHelper.GetFilePath($"*{FileEXTENSION}|*{FileEXTENSION};", $"{FileEXTENSION}");

            if (string.IsNullOrEmpty(FilePath))
                return false;

            return SetUserSettings(FilePath , true);
        }

        private static bool SetUserSettings(string FilePath, bool SetItAsDefault)
        {
            string Settings = File.ReadAllText(FilePath);

            ChannelsTypeSettings.SetUserChannelsType(Settings);
            if (SetItAsDefault)
                SetDefalutFileSettingsRegisrty(FilePath);

            return true;
        }

        public static bool SaveSettings(List<ChannelType> ChannelsType)
        {
            string Filename = OpenFileHelper.GetSaveFile($"*{FileEXTENSION}|*{FileEXTENSION};", $"{FileEXTENSION}");
            if (Filename == null)
                return false;

            return SaveSettings(Filename, ChannelsType);
        }

        private static bool SaveSettings(string Filepath, List<ChannelType> ChannelsType)
        {
            string Settings = ChannelsTypeSettings.GetChannelsTypeSettings(ChannelsType);

            using (StreamWriter outputFile = new StreamWriter(Filepath, false))
            {
                outputFile.WriteLine(Settings);
            }

            SetDefalutFileSettingsRegisrty(Filepath);

            return true;
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

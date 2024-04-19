using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMODBUS.Models;
using TestMODBUS.Models.Data.Helpers;

namespace TestMODBUS.Services.Settings.Channels
{
    public static class ChannelsSettingFileManager
    {
        private const string FileEXTENSION = ".chnls";
        private static readonly string ChannelsSettignsPATH = Path.Combine(Environment.GetFolderPath(
                                                                  Environment.SpecialFolder.ApplicationData), "ModBus", "Channels Settings");

        public static void SaveSettings()
        {
            string Filename = OpenFileHelper.GetSaveFile($"*.{FileEXTENSION}|*.{FileEXTENSION};", $".{FileEXTENSION}");
            if (Filename == null)
                return;

            SaveSettings(Filename);
        }

        public static void SaveSettings(string Filename)
        {
            string Settings = ChannelsTypeSettings.GetChannelsTypeSettings();

            using (StreamWriter outputFile = new StreamWriter(Filename, false))
            {
                outputFile.WriteLine(Settings);
            }
        }
    }
}

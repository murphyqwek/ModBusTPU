using ModBusTPU.Models.MessageBoxes;
using ModBusTPU.Services.Settings.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBusTPU.Services.Settings.MotorSettings
{
    internal static class MotorSettingsUploader
    {
        public static (MotorSettingsContainer motorSettings, string exceptionText)  UploadDefulat(bool turnDownTheException)
        {
            object pathObj = RegisrtyService.GetField("Motor", "SettingsPath", true);

            if(pathObj == null)
            {
                return (new MotorSettingsContainer(), string.Empty);
            }


            return Upload(pathObj.ToString());
        }

        public static (MotorSettingsContainer motorSettings, string exceptionText) Upload(string path)
        {
            var result = new SettingsManager<MotorSettingsContainer>(path, new JsonSerializator<MotorSettingsContainer>()).Upload();

            if(result.exceptionTest == string.Empty)
            {
                RegisrtyService.SetField("Motor", "SettingsPath", path);
            }

            return result;
        }


        public static void Save(MotorSettingsContainer motorSettings, string filePath)
        {
            new SettingsManager<MotorSettingsContainer>(filePath, new JsonSerializator<MotorSettingsContainer>()).Save(motorSettings);
        }
    }
}

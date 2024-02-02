using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMODBUS.Services
{
    public static class Settings
    {
        public static string GetChannelLabel(int ChannelNumber)
        {
            string ChannelField = $"CH_{ChannelNumber}";
            string ChannelName = RegisrtyService.GetField(RegisrtyService.ChannelFolder, ChannelField, true).ToString();

            if(ChannelName == null)
            {
                ChannelName = ChannelField;
                RegisrtyService.SetField(RegisrtyService.ChannelFolder, ChannelField, ChannelName);
            }

            return ChannelName;
        }

        public static bool GetChannelIsChosen(int ChannelNumber)
        {
            string ChannelField = $"CH_{ChannelNumber} IsChosen";
            bool IsChannelChosen = false;
            object IsChannelChosenValue = RegisrtyService.GetField(RegisrtyService.ChannelFolder, ChannelField, true);

            if(IsChannelChosenValue == null)
            {
                RegisrtyService.SetField(RegisrtyService.ChannelFolder, ChannelField, IsChannelChosen);
                return IsChannelChosen;
            }

            IsChannelChosen = Convert.ToBoolean(IsChannelChosenValue);

            return IsChannelChosen;
        }
    }
}

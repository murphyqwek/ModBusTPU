using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using TestMODBUS.Models.Data;

namespace TestMODBUS.Services.Settings
{
    public static class ExportChannelsSettings
    {
        private static string GetChannelLabelFieldName(int ChannelNumber) => $"CH_{ChannelNumber}";
        private static string GetChannelIsChosenFieldName(int ChannelNumber) => $"CH_{ChannelNumber} IsChosen";

        public static bool SaveChannels(IList<ChannelModel> channels)
        {
            foreach (var channel in channels)
            {
                SaveChannel(channel.ChannelNumber, channel.Label, channel.IsChosen);
            }

            return true;
        }

        public static void SaveChannel(int ChannelNumber, string Label, bool IsChosen)
        {
            RegisrtyService.SetField(RegisrtyService.ChannelFolder, GetChannelLabelFieldName(ChannelNumber), Label);
            RegisrtyService.SetField(RegisrtyService.ChannelFolder, GetChannelIsChosenFieldName(ChannelNumber), IsChosen);
        }

        public static string GetChannelLabel(int ChannelNumber)
        {
            string ChannelField = GetChannelLabelFieldName(ChannelNumber);
            string ChannelName = RegisrtyService.GetField(RegisrtyService.ChannelFolder, ChannelField, true)?.ToString();

            if(ChannelName == null)
            {
                ChannelName = ChannelField;
                RegisrtyService.SetField(RegisrtyService.ChannelFolder, ChannelField, ChannelName);
            }

            return ChannelName;
        }

        public static bool GetChannelIsChosen(int ChannelNumber)
        {
            string ChannelField = GetChannelIsChosenFieldName(ChannelNumber);
            bool IsChannelChosen = false;
            object IsChannelChosenValue = RegisrtyService.GetField(RegisrtyService.ChannelFolder, ChannelField, true);

            if(IsChannelChosenValue == null || Boolean.TryParse(IsChannelChosenValue.ToString(), out bool r))
            {
                RegisrtyService.SetField(RegisrtyService.ChannelFolder, ChannelField, IsChannelChosen);
                return IsChannelChosen;
            }

            IsChannelChosen = Convert.ToBoolean(IsChannelChosenValue);

            return IsChannelChosen;
        }

        public static void UploadChannelSettings(IList<ChannelModel> Channels)
        {
            foreach (var channel in Channels)
            {
                int channelNumber = channel.ChannelNumber;
                channel.Label = GetChannelLabel(channelNumber);
                channel.IsChosen = GetChannelIsChosen(channelNumber);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace ModBusTPU.Services.Settings.Export
{
    public struct ChannelData
    {
        public int Channel;
        public string Label;
        public bool IsChosen;
    }

    public struct ExtraData
    {
        public string Label;
        public List<int> UsingChannels;
    }

    public static class ExportSaving
    {
        public const string CHANNELHEADER = "CHANNELS";
        public const string POWERHEADER = "POWER";
        public const string ENERGYHEADER = "ENERGY";
        public const string COMMENTARYHEADER = "COMMENTARY";

        public const char SPECSYMBOLFORREPLACINGBACKSPACE = '&';

        public const string SEPARATOR = " ";

        public static string GetData(ExportSettings Settings)
        {
            return GetData(Settings.ChannelsData, Settings.PowerData, Settings.EnergyData, Settings.CommentaryLabels);
        }

        public static string GetData(List<ChannelData> ChannelsData, List<ExtraData> PowerData, List<ExtraData> EnergyData, List<string> CommenatryLabels)
        {
            string Data = CHANNELHEADER + "\n";
            foreach(ChannelData Channel in ChannelsData)
                Data += Channel.Label.Replace(' ', SPECSYMBOLFORREPLACINGBACKSPACE) + " " + Channel.Channel.ToString() + " " + Convert.ToInt32(Channel.IsChosen).ToString() + "\n";

            Data += POWERHEADER + "\n";
            foreach (var ExtraData in PowerData)
                Data += ExtraData.Label.Replace(' ', SPECSYMBOLFORREPLACINGBACKSPACE) + " " + GetUsingChannelsString(ExtraData.UsingChannels) + "\n";

            Data += ENERGYHEADER + "\n";
            foreach (var ExtraData in EnergyData)
                Data += ExtraData.Label.Replace(' ', SPECSYMBOLFORREPLACINGBACKSPACE) + " " + GetUsingChannelsString(ExtraData.UsingChannels) + "\n";

            Data += COMMENTARYHEADER + "\n";
            foreach (var Label in CommenatryLabels)
                Data += Label.Replace(' ', SPECSYMBOLFORREPLACINGBACKSPACE) + "\n";

            return Data;
        }

        private static string GetUsingChannelsString(List<int> UsingChannels) => string.Join(SEPARATOR, UsingChannels);
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModBusTPU.Models.Data;
using ModBusTPU.ViewModels;
using ModBusTPU.ViewModels.ExportViewModels;

namespace ModBusTPU.Services.Settings.Export
{
    public class ExportSettings
    {
        public List<ChannelData> ChannelsData { get; }
        public List<ExtraData> PowerData { get; }
        public List<ExtraData> EnergyData { get; }
        public List<string> CommentaryLabels { get; }

        public ExportSettings(List<ChannelData> ChannelsData, List<ExtraData> PowerData, List<ExtraData> EnergyData, List<string> CommentaryLabels)
        {
            this.ChannelsData = ChannelsData;
            this.PowerData = PowerData;
            this.EnergyData = EnergyData;
            this.CommentaryLabels = CommentaryLabels; 
        }

        public ExportSettings(ObservableCollection<ChannelViewModel> ChannelsData, ObservableCollection<ExtraDataViewModel> PowerExtraData, ObservableCollection<ExtraDataViewModel> EnergyExtraData, ObservableCollection<CommentaryExportElementViewModel> Commentaries)
        {
            this.ChannelsData = ParseChannels(ChannelsData);
            this.PowerData = ParseExtraData(PowerExtraData);
            this.EnergyData = ParseExtraData(EnergyExtraData);
            this.CommentaryLabels = ParseCommentaries(Commentaries);
        }

        private List<string> ParseCommentaries(ObservableCollection<CommentaryExportElementViewModel> Commentaries)
        {
            List<string> CommentaryLabels = new List<string>();

            foreach(var Commentary in Commentaries)
            {
                CommentaryLabels.Add(Commentary.Label);
            }

            return CommentaryLabels;
        }

        private List<ChannelData> ParseChannels(Collection<ChannelViewModel> Channels)
        {
            List<ChannelData> ParsedData = new List<ChannelData>();

            foreach(var Channel in Channels)
            {
                var Data = new ChannelData();
                Data.Channel = Channel.ChannelNumber;
                Data.IsChosen = Channel.IsChosen;
                Data.Label = Channel.Label;

                ParsedData.Add(Data);
            }

            return ParsedData;
        }

        private List<ExtraData> ParseExtraData(Collection<ExtraDataViewModel> ExtraDataCollection)
        {
            var ParsedData = new List<ExtraData>();

            foreach(var ExtraData in ExtraDataCollection)
            {
                var Data = new ExtraData();
                Data.Label = ExtraData.Label;
                Data.UsingChannels = ExtraData.GetUsingChannels() as List<int>;
                ParsedData.Add(Data);
            }

            return ParsedData;
        }
    }
}

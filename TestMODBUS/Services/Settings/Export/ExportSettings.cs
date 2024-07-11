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
        public List<Commentary> Commentaries { get; }

        public ExportSettings(List<ChannelData> ChannelsData, List<ExtraData> PowerData, List<ExtraData> EnergyData, List<Commentary> Commentaries)
        {
            this.ChannelsData = ChannelsData;
            this.PowerData = PowerData;
            this.EnergyData = EnergyData;
            this.Commentaries = Commentaries; 
        }

        public ExportSettings(ObservableCollection<ChannelViewModel> ChannelsData, ObservableCollection<ExtraDataViewModel> PowerExtraData, ObservableCollection<ExtraDataViewModel> EnergyExtraData, ObservableCollection<CommentaryExportElementViewModel> Commentaries)
        {
            this.ChannelsData = ParseChannels(ChannelsData);
            this.PowerData = ParseExtraData(PowerExtraData);
            this.EnergyData = ParseExtraData(EnergyExtraData);
            this.Commentaries = ParseCommentaries(Commentaries);
        }

        private List<Commentary> ParseCommentaries(ObservableCollection<CommentaryExportElementViewModel> CommentariesCollection)
        {
            List<Commentary> Commentaries = new List<Commentary>();

            foreach(var Commentary in CommentariesCollection)
            {
                Commentary CommentaryStruct = new Commentary();
                CommentaryStruct.Label = Commentary.Label;
                CommentaryStruct.IsShownOnMainWindow = Commentary.IsShownOnMainWindow;

                Commentaries.Add(CommentaryStruct);
            }

            return Commentaries;
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
                if (!ExtraData.IsAllChosen)
                    throw new Exception("Не удалось сохранить Мощность или Энергию: не все каналы выбраны");
                var Data = new ExtraData();
                Data.Label = ExtraData.Label;
                Data.UsingChannels = ExtraData.GetUsingChannels() as List<int>;
                ParsedData.Add(Data);
            }

            return ParsedData;
        }
    }
}

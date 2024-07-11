using Microsoft.Windows.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using ModBusTPU.Models.Data;
using ModBusTPU.Models.ModbusSensor.ChartDataPrepatations;

namespace ModBusTPU.Services.Settings.Export
{
    public static class ExportReading
    {
        public static ExportSettings ReadFile(string Data)
        {
            string[] Lines = Data.Split('\n');

            if (Lines[0] != ExportSaving.CHANNELHEADER)
                throw new Exception("Файл настроек экспорта повреждён");

            int i = 1;
            List<ChannelData> ChannelsData = GetChannelsData(ref i, Lines);

            if(ChannelsData == null)
                throw new Exception("Файл настроек экспорта повреждён");

            if (Lines[i] != ExportSaving.POWERHEADER)
                throw new Exception("Файл настроек экспорта повреждён");

            i++;
            List<ExtraData> PowerData = GetExtraDataFromLine(ref i, Lines);

            i++;
            List<ExtraData> EnergyData = GetExtraDataFromLine(ref i, Lines);

            i++;
            List<Commentary> CommentaryLabels = GetCommentaryLabels(ref i, Lines);


            return new ExportSettings(ChannelsData, PowerData, EnergyData, CommentaryLabels);
        }

        private static List<Commentary> GetCommentaryLabels(ref int i, string[] Lines)
        {
            List<Commentary> Commentaries = new List<Commentary>();

            while (i < Lines.Length && !string.IsNullOrEmpty(Lines[i]))
            {
                try
                {
                    var Temp = Lines[i].Split(' ');
                    string Label = Temp[0];
                    Label = Label.Trim('\n');
                    Label = Label.Replace(ExportSaving.SPECSYMBOLFORREPLACINGBACKSPACE, ' ');

                    bool IsShownOnMainWindow = false;
                    if (Temp.Length > 1)
                        IsShownOnMainWindow = Convert.ToBoolean(Convert.ToInt32(Temp[1]));

                    Commentaries.Add(new Commentary { Label = Label, IsShownOnMainWindow = IsShownOnMainWindow });
                    i++;
                }
                catch
                {
                    return null;
                }
            }

            return Commentaries;
        }

        private static List<ChannelData> GetChannelsData(ref int i, string[] Lines)
        {
            List<ChannelData> ChannelsData = new List<ChannelData>();

            for (int j = 0; j < DataStorage.MaxChannelCount; j++)
            {
                try
                {
                    string Label, Channel, IsChosen;
                    Label = Lines[i].Split(' ')[0];
                    Channel = Lines[i].Split(' ')[1];
                    IsChosen = Lines[i].Split(' ')[2];

                    int ChannelNumber = Convert.ToInt32(Channel);
                    bool IsChosenBool = IsChosen == "1";

                    ChannelData ChannelData = new ChannelData();
                    ChannelData.Channel = ChannelNumber;
                    ChannelData.IsChosen = IsChosenBool;
                    ChannelData.Label = Label.Replace(ExportSaving.SPECSYMBOLFORREPLACINGBACKSPACE, ' ');

                    ChannelsData.Add(ChannelData);
                    i++;
                }
                catch
                {
                    return null;
                }
            }

            return ChannelsData;      
        }

        private static List<ExtraData> GetExtraDataFromLine(ref int i, string[] Lines)
        {
            List<ExtraData> ChannelsData = new List<ExtraData>();
            while(i < Lines.Length && Lines[i] != ExportSaving.ENERGYHEADER && Lines[i] != ExportSaving.COMMENTARYHEADER && !string.IsNullOrEmpty(Lines[i]) )
            {
                try
                {
                    List<int> UsingChannels = new List<int>();
                    string[] Line = Lines[i].Split(' ');
                    string Label = Line[0];
                    for(int j = 1; j < Line.Length; j++)
                        UsingChannels.Add(Convert.ToInt32(Line[j]));

                    ExtraData Data = new ExtraData();
                    Data.Label = Label.Replace(ExportSaving.SPECSYMBOLFORREPLACINGBACKSPACE, ' ');
                    Data.UsingChannels = UsingChannels;

                    ChannelsData.Add(Data);
                    i++;
                }
                catch
                {
                    return null;
                }
            }

            return ChannelsData;
        }
    }
}

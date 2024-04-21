using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMODBUS.Models.Data;
using TestMODBUS.Models.Services;

namespace TestMODBUS.Services.Settings.Channels
{
    public static class ChannelsTypeSettings
    {
        private const string TokChannelType = "T";
        private const string VoltChannelType = "V";
        private const string RegularChannelType = "C";


        public static void SetStandartChannelsType()
        {
            //5, 6, 7,
            List<ChannelType> StandartChannelType = new List<ChannelType>()
            {
                ChannelType.Tok,     // 0
                ChannelType.Tok,     // 1
                ChannelType.Tok,     // 2
                ChannelType.Tok,     // 3
                ChannelType.Regular, // 4
                ChannelType.Volt,    // 5
                ChannelType.Volt,    // 6
                ChannelType.Volt,    // 7
            };

            ChannelTypeList.SetChannelsType(StandartChannelType);
        }

        public static void SetUserChannelsType(string UserSettings)
        {
            UserSettings = UserSettings.TrimEnd('\r', '\n');
            string[] ChannelsType = UserSettings.Split(' ');

            if (ChannelsType.Length != DataStorage.MaxChannelCount)
            {
                throw new Exception("Файл настроек типов каналов был повреждён");
            }

            List<ChannelType> UserChannelsType = new List<ChannelType>();
            foreach(string ChannelType in ChannelsType) 
            {
                switch (ChannelType)
                {
                    case RegularChannelType:
                        UserChannelsType.Add(Models.Services.ChannelType.Regular);
                        break;
                    case TokChannelType:
                        UserChannelsType.Add(Models.Services.ChannelType.Tok);
                        break;
                    case VoltChannelType:
                        UserChannelsType.Add(Models.Services.ChannelType.Volt);
                        break;
                    default:
                        throw new Exception("Файл настроек типов каналов был повреждён");
                }
            }

            ChannelTypeList.SetChannelsType(UserChannelsType);
        }

        public static void SetUserChannelsType(List<ChannelType> NewChannelsType)
        {
            ChannelTypeList.SetChannelsType(NewChannelsType);
        }

        public static string GetChannelsTypeSettings(List<ChannelType> ChannelsType) 
        {
            string output = "";

            foreach(var Type in ChannelsType)
            {
                switch (Type)
                {
                    case ChannelType.Regular: output += RegularChannelType; break;
                    case ChannelType.Tok:     output += TokChannelType; break;
                    case ChannelType.Volt:    output += VoltChannelType; break;
                }

                output += " ";
            }

            output = output.TrimEnd();

            return output;
        }

        public static string GetCurrentChannelsTypeSetting()
        {
            return GetChannelsTypeSettings(ChannelTypeList.GetChannelsType());
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMODBUS.Models.Data;

namespace TestMODBUS.Models.Services
{
    public enum ChannelType
    {
        Regular,
        Tok,
        Volt
    }


    //Временный класс, который хранит номера портов тока и напряжения
    public static class ChannelTypeList
    {
        private static List<ChannelType> ChannelsType = new List<ChannelType>();

        public static HashSet<int> TokChannels = new HashSet<int>()
        {
            0, 1, 2, 3,
        };

        public static HashSet<int> VoltChannels = new HashSet<int>()
        {
            5, 6, 7,
        };


        public static void SetChannelsType(List<ChannelType> NewChannelsType)
        {
            if (NewChannelsType.Count != DataStorage.MaxChannelCount)
                throw new Exception($"Count of {nameof(NewChannelsType)} must be equal {nameof(DataStorage.MaxChannelCount)}");
            ChannelsType = NewChannelsType;
        }

        public static ChannelType GetChannelType(int Channel) 
        {
            if (Channel < 0 || Channel >= ChannelsType.Count)
                throw new ArgumentOutOfRangeException(nameof(Channel));

            return ChannelsType[Channel];
        }
    }
}

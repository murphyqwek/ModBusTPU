using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using ModBusTPU.Models.Services;

namespace ModBusTPU.Models.ModbusSensor.ModBusInputs.ChannelsFilters
{
    public class OnlyOneVoltAndSeveralTokFilter : IFilter
    {
        public void AddChannel(IList<int> Channels, int NewChannel)
        {
            if (ChannelTypeList.GetChannelType(NewChannel) == ChannelType.Regular)
                return;

            int prevVoltChannel = GetVoltChannel(Channels);
            if (prevVoltChannel != -1 && ChannelTypeList.GetChannelType(NewChannel) == ChannelType.Volt)
                Channels.Remove(prevVoltChannel);

            Channels.Add(NewChannel);
        }

        private int GetVoltChannel(IList<int> Channels)
        {
            foreach (var channel in Channels)
            {
                if (ChannelTypeList.GetChannelType(channel) == ChannelType.Volt)
                    return channel;
            }
            return -1;
        }

        public bool IsAllChannelsChosen(IList<int> Channels)
        {
            int TokChannels = 0, VoltChannels = 0;
            foreach (var channel in Channels)
            {
                if (ChannelTypeList.GetChannelType(channel) == ChannelType.Volt)
                    VoltChannels++;
                else if (ChannelTypeList.GetChannelType(channel) == ChannelType.Tok)
                    TokChannels++;
                else
                    return false;
            }

            if (TokChannels > 0 && VoltChannels == 1)
                return true;
            else
                return false;
        }
    }
}

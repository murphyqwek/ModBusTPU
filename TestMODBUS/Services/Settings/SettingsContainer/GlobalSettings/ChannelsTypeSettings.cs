using ModBusTPU.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBusTPU.Services.Settings.SettingsContainer.GlobalSettings
{
    internal class ChannelsTypeSettings
    {
        public List<ChannelType> ChannelTypes { get; set; } = new List<ChannelType>()
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

        public ChannelsTypeSettings(int channelsCount = 8) 
        { 
            if(channelsCount == 8)
            {
                return;
            }

            ChannelTypes = new List<ChannelType>();

            for(int i = 0; i < channelsCount; i++)
            {
                ChannelTypes.Add(ChannelType.Regular);
            }
        }


        public ChannelType GetChannelType(int channel)
        {
            if(channel < 0 || channel >= ChannelTypes.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return ChannelTypes[channel];
        }
    }
}

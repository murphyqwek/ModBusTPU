using ModBusTPU.Services.Settings.SettingsContainer;
using ModBusTPU.Services.Settings.SettingsContainer.GlobalSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBusTPU.Services.Settings.SetingsContainer.GlobalSettings
{
    internal class GlobalSettingsContainer : BaseSettingsContainer
    {
        public int ChannelsCount { get; set; } = 8;

        public ChannelsTypeSettings ChannelsTypeSettings { get; set; } = new ChannelsTypeSettings();

        public override void EnsureDefualt()
        {
            ChannelsTypeSettings = ChannelsTypeSettings != null ? ChannelsTypeSettings : new ChannelsTypeSettings(ChannelsCount);
        }

        public override void SetDefaultSettingsContainer()
        {
            ChannelsCount = 8;
            ChannelsTypeSettings = new ChannelsTypeSettings(ChannelsCount);
        }

        public override bool isValid()
        {
            if(ChannelsCount <= 0)
            {
                return false;
            }

            if(ChannelsTypeSettings.ChannelTypes.Count < ChannelsCount)
            {
                for(int i = ChannelsTypeSettings.ChannelTypes.Count; i <= ChannelsCount; i++)
                {
                    ChannelsTypeSettings.ChannelTypes.Add(Models.Services.ChannelType.Regular);
                }
            }
            else
            {
                return false;
            }


            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMODBUS.Models.Services;
using TestMODBUS.Services.Settings.Channels;
using TestMODBUS.ViewModels.Base;

namespace TestMODBUS.ViewModels
{
    public class ChannelsTypeChoosingViewModel : BaseViewModel
    {
        public List<ChannelTypeViewModel> Channels { get; }

        public ChannelsTypeChoosingViewModel()
        {
            ChannelsSettingFileManager.UploadDefaultSettings();

            Channels = new List<ChannelTypeViewModel>();
            for(int i = 0; i < ChannelTypeList.ChannelCounts;  i++)
            {
                Channels.Add(new ChannelTypeViewModel(i, ChannelTypeList.GetChannelType(i)));
            }
        }
    }
}

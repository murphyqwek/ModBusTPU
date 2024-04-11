using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using TestMODBUS.Models.Services;

namespace TestMODBUS.Models.ModbusSensor.ModBusInputs
{
    internal class ModBusInputPower : ModBusInputBase
    {
        public ModBusInputPower(ModbusSensorController Controller) : base(Controller)
        {
        }

        public ModBusInputPower(ModbusSensorController Controller, IEnumerable<int> Channels) : base(Controller, Channels)
        { }

        public override void AddNewChannel(int Channel)
        {
            if (!ChannelTypeList.VoltChannels.Contains(Channel) && !ChannelTypeList.TokChannels.Contains(Channel))
                return;

            int prevVoltChannel = CheckForVoltChannel(Channel);
            if (prevVoltChannel != -1)
                _controller.SetUsingChannel(prevVoltChannel, false);

            int previousLastChannel = _controller.GetLastChannel();
            _controller.SetUsingChannel(Channel, true);
            if (_controller.GetUsingChannels().Count == 1 && prevVoltChannel == -1)
                _controller.AddNewLineSerie("Мощность", ChannelColors.Colors[Channel]);
            ResignDataStorageLastUpdateChannel(previousLastChannel);

            _controller.UpdateChartAfterNewChannelAdded();
        }

        private int CheckForVoltChannel(int NewChannel)
        {
            if(!ChannelTypeList.VoltChannels.Contains(NewChannel))
                return -1;

            var channels = _controller.GetUsingChannels();
            foreach(var channel in channels)
            {
                if(ChannelTypeList.VoltChannels.Contains(channel))
                    return channel;
            }
            return -1;
        }

        public override void RemoveChannel(int Channel)
        {
            int lastChannel = _controller.GetLastChannel();
            _controller.SetUsingChannel(Channel, false);
            if (_controller.GetUsingChannels().Count == 0)
                _controller.RemoveSerie($"Мощность");
            ResignDataStorageLastUpdateChannel(lastChannel);
        }

        public override bool CheckAllChannelsChosen()
        {
            int TokChannels = 0, VoltChannels = 0;
            var channels = _controller.GetUsingChannels();
            foreach (var channel in channels)
            {
                if (ChannelTypeList.VoltChannels.Contains(channel))
                    VoltChannels++;
                else if (ChannelTypeList.TokChannels.Contains(channel))
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

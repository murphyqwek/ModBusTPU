using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMODBUS.Models.ModbusSensor.ModBusInputs.ChannelsFilters;
using TestMODBUS.Models.Services;

namespace TestMODBUS.Models.ModbusSensor.ModBusInputs
{
    internal class ModBusInputEnergy : ModBusInputBase
    {
        private IFilter _filter = new OnlyOneVoltAndSeveralTokFilter();

        public ModBusInputEnergy(ModbusSensorController Controller) : base(Controller)
        {
        }

        public ModBusInputEnergy(ModbusSensorController Controller, IEnumerable<int> Channels) : base(Controller, Channels)
        { }

        public override void AddNewChannel(int Channel)
        {
            if (ChannelTypeList.GetChannelType(Channel) == ChannelType.Regular)
                return;

            int prevVoltChannel = GetVoltChannel();
            if (prevVoltChannel != -1 && ChannelTypeList.GetChannelType(Channel) == ChannelType.Volt)
                _controller.SetUsingChannel(prevVoltChannel, false);

            int previousLastChannel = _controller.GetLastChannel();
            _controller.SetUsingChannel(Channel, true);

            if (_controller.GetUsingChannels().Count == 1 && prevVoltChannel == -1)
                _controller.AddNewLineSerie("Энергия", ChannelColors.Colors[Channel]);
            ResignDataStorageLastUpdateChannel(previousLastChannel);

            _controller.UpdateChartAfterNewChannelAdded();
        }

        private int GetVoltChannel()
        {
            var channels = _controller.GetUsingChannels();
            foreach (var channel in channels)
            {
                if (ChannelTypeList.GetChannelType(channel) == ChannelType.Volt)
                    return channel;
            }
            return -1;
        }

        public override void RemoveChannel(int Channel)
        {
            int lastChannel = _controller.GetLastChannel();
            _controller.SetUsingChannel(Channel, false);
            if (_controller.GetUsingChannels().Count == 0)
                _controller.RemoveSerie("Энергия");
            ResignDataStorageLastUpdateChannel(lastChannel);
        }

        public override bool CheckAllChannelsChosen()
        {
            var channels = _controller.GetUsingChannels();
            return _filter.IsAllChannelsChosen(channels);
        }

        private void DeleteAll()
        {
            _controller.DeleteExtraData();
            _controller.RemoveSerie("Энергия");
            foreach (int Channel in _controller.GetUsingChannels())
            {
                _controller.UnsignToChannelUpdation(Channel, DataStorageCollectionChangedHandler);
                _controller.SetUsingChannel(Channel, false);
            }
        }

        public override void CheckNewChannelsTypes()
        {
            DeleteAll();
            _controller.UpdateChartAfterNewChannelAdded();
        }
    }
}

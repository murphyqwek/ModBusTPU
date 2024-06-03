using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModBusTPU.Models.Data;
using ModBusTPU.Models.ModbusSensor.ChartDataPrepatations;
using ModBusTPU.Models.ModbusSensor.ModBusInputs;
using ModBusTPU.Models.ModbusSensor.ModBusInputs.ChannelsFilters;
using ModBusTPU.Models.Services;

namespace ModBusTPU.Models.ModbusSensor.ModBusInputs
{
    public class ModBusInputStandart : ModBusInputBase
    {
        private IFilter _filter = new StandratFilter();

        public ModBusInputStandart(ModbusSensorController Controller) : base(Controller)
        {
        }

        public ModBusInputStandart(ModbusSensorController Controller, IEnumerable<int> Channels) : base(Controller, Channels)
        { }

        public override void AddNewChannel(int Channel)
        {
            int previousLastChannel = _controller.GetLastChannel();
            _controller.SetUsingChannel(Channel, true);
            _controller.AddNewLineSerie($"CH_{Channel}", ChannelColors.Colors[Channel]);
            ResignDataStorageLastUpdateChannel(previousLastChannel);

            _controller.UpdateChartAfterNewChannelAdded();
        }

        public override bool CheckAllChannelsChosen()
        {
            var channels = _controller.GetUsingChannels();
            return _filter.IsAllChannelsChosen(channels);
        }

        public override void CheckNewChannelsTypes()
        {
            _controller.UpdateChartAfterNewChannelAdded();
        }

        public override void RemoveChannel(int Channel)
        {
            int lastChannel = _controller.GetLastChannel();
            _controller.SetUsingChannel(Channel, false);
            _controller.RemoveSerie($"CH_{Channel}");
            ResignDataStorageLastUpdateChannel(lastChannel);
        }
    }
}

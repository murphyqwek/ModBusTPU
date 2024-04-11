using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMODBUS.Models.Data;
using TestMODBUS.Models.ModbusSensor.ChartDataPrepatations;
using TestMODBUS.Models.ModbusSensor.ModBusInputs;
using TestMODBUS.Models.Services;

namespace TestMODBUS.Models.ModbusSensor.ModBusInputs
{
    public class ModBusInputStandart : ModBusInputBase
    {
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
            if (_controller.GetUsingChannels().Count > 0)
                return true;
            else
                return false;
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

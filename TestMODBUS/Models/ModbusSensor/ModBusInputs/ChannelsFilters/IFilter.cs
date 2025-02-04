using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBusTPU.Models.ModbusSensor.ModBusInputs.ChannelsFilters
{
    public interface IFilter
    {
        bool IsAllChannelsChosen(IList<int> Channels);

        void AddChannel(IList<int> Channels, int NewChannel);
    }
}

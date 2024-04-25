using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMODBUS.Models.ModbusSensor.ModBusInputs.ChannelsFilters
{
    public class StandratFilter : IFilter
    {
        public void AddChannel(IList<int> Channels, int NewChannel)
        {
            Channels.Add(NewChannel);
        }

        public bool IsAllChannelsChosen(IList<int> Channels) => Channels.Count > 0;
    }
}

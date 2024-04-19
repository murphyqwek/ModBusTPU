using LiveCharts.Defaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMODBUS.Models.Data;
using TestMODBUS.Models.Modbus;
using TestMODBUS.Models.Services;

namespace TestMODBUS.Models.ModbusSensor.ChartDataPrepatations
{
    public class ChartDataPreparationStandart : ChartDataPreparationBase
    {
        public override IList<string> GetCurrentValues(IList<int> Channels, DataStorage DataStorage)
        {
            IList<string> values = new List<string>();
            foreach(var channel in Channels)
            {
                double Value = GetLastConvertedValue(DataStorage, channel);
                string CurrentValue = $"CH_{channel}: ";
                string ValueType = "";

                if (ChannelTypeList.GetChannelType(channel) == ChannelType.Volt)
                    ValueType += " А";
                else if (ChannelTypeList.GetChannelType(channel) == ChannelType.Volt)
                    ValueType += " В";

                CurrentValue += Value.ToString() + ValueType;

                values.Add(CurrentValue);
            }

            return values;
        }

        protected override IList<SerieData> GetPoints(IList<int> ChannelsToUpdate, DataStorage DataStorage, int left, int right)
        {
            List<SerieData> SeriesToUpdate = new List<SerieData>();

            foreach (int Channel in ChannelsToUpdate)
            {
                var Points = WindowingDataHelper.GetWindowData(left, right, DataStorage.GetChannelData(Channel));

                Points = Convert(Points, Channel);

                SerieData serieData = new SerieData();
                serieData.SerieTitle = $"CH_{Channel}";
                serieData.Points = Points;

                SeriesToUpdate.Add(serieData);
            }

            return SeriesToUpdate;
        }
    }
}

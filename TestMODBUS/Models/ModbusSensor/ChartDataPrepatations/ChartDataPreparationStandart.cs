using LiveCharts.Defaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModBusTPU.Models.Data;
using ModBusTPU.Models.Modbus;
using ModBusTPU.Models.Services;

namespace ModBusTPU.Models.ModbusSensor.ChartDataPrepatations
{
    public class ChartDataPreparationStandart : ChartDataPreparationBase
    {
        public override IList<string> GetCurrentValues(IList<int> Channels, DataStorage DataStorage)
        {
            IList<string> values = new List<string>();
            foreach(var channel in Channels)
            {
                double Value = GetLastConvertedValue(DataStorage, channel);
                string CurrentValue = "";
                string ValueType = "";

                if (ChannelTypeList.GetChannelType(channel) == ChannelType.Tok)
                {
                    CurrentValue = "Ток: ";
                    ValueType += " А";
                }
                else if (ChannelTypeList.GetChannelType(channel) == ChannelType.Volt)
                {
                    CurrentValue = "Напр.: ";
                    ValueType += " В";
                }

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
                Points = ConvertMillisecondsToSeconds(Points);

                SerieData serieData = new SerieData();

                var ChannelType = ChannelTypeList.GetChannelType(Channel);

                string label = "";
                switch(ChannelType)
                {
                    case ChannelType.Tok:
                        label = "Ток";
                        break;
                    case ChannelType.Volt:
                        label = "Напряжение";
                        break;
                }

                serieData.SerieTitle = label;
                serieData.Points = Points;

                SeriesToUpdate.Add(serieData);
            }

            return SeriesToUpdate;
        }
    }
}

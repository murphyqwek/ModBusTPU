using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMODBUS.Models.Chart;
using TestMODBUS.Models.Data;
using TestMODBUS.Models.Services;

namespace TestMODBUS.Models.ModbusSensor.ChartDataPrepatations
{
    public class ChartDataPreparationStandart : ChartDataPrepatationBase
    {
        public override IList<string> GetCurrentValues(IList<int> Channels, DataStorage DataStorage)
        {
            IList<string> values = new List<string>();
            foreach(var channel in Channels)
            {
                string value = $"CH_{channel}: {DataStorage.GetChannelData(channel).Last().Y}";
                if (ChannelTypeList.TokChannels.Contains(channel))
                    value += " А";
                else if (ChannelTypeList.VoltChannels.Contains(channel))
                    value += " В";

                values.Add(value);
            }

            return values;
        }

        /*
        public override IList<SerieData> GetPointsByCurrentX(IList<int> ChannelsToUpdate, DataStorage DataStorage, double CurrentX)
        {
            int LastChannel = ChannelsToUpdate.Last();
            (int, int) WindowDataEdges;
            WindowDataEdges = WindowingDataHelper.GetDataWindowIndex(CurrentX, CurrentX + Chart.MaxWindowWidth, DataStorage.GetChannelData(LastChannel));
            int leftEdge = WindowDataEdges.Item1, rightEdge = WindowDataEdges.Item2;

            return GetPoints(ChannelsToUpdate, DataStorage, leftEdge, rightEdge);
        }
        */

        protected override IList<SerieData> GetPoints(IList<int> ChannelsToUpdate, DataStorage DataStorage, int left, int right)
        {
            List<SerieData> SeriesToUpdate = new List<SerieData>();

            foreach (int Channel in ChannelsToUpdate)
            {
                var Points = WindowingDataHelper.GetWindowData(left, right, DataStorage.GetChannelData(Channel));

                SerieData serieData = new SerieData();
                serieData.SerieTitle = $"CH_{Channel}";
                serieData.Points = Points;

                SeriesToUpdate.Add(serieData);
            }

            return SeriesToUpdate;
        }
    }
}

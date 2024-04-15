using LiveCharts.Defaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using TestMODBUS.Models.Chart;
using TestMODBUS.Models.Data;
using TestMODBUS.Models.MathModules;
using TestMODBUS.Models.Services;

namespace TestMODBUS.Models.ModbusSensor.ChartDataPrepatations
{
    internal class ChartDataPreparationPower : ChartDataPrepatationBase
    {
        public override IList<string> GetCurrentValues(IList<int> Channels, DataStorage DataStorage)
        {
            IList<string> values = new List<string>();
            double Tok = 0, Volt = 0;

            foreach (var Channel in Channels)
            {
                if (ChannelTypeList.TokChannels.Contains(Channel))
                    Tok += DataStorage.GetChannelData(Channel).Last().Y;
                if (ChannelTypeList.VoltChannels.Contains(Channel))
                    Volt += DataStorage.GetChannelData(Channel).Last().Y;
            }

            values.Add($"Мощность: {PowerMathModule.CountKV(Tok, Volt)} Вт");
            return values;
        }

        protected override IList<SerieData> GetPoints(IList<int> ChannelsToUpdate, DataStorage DataStorage, int left, int right)
        {
            List<SerieData> SeriesToUpdate = new List<SerieData>();

            List<ObservablePoint> TokPoints = new List<ObservablePoint>();
            List<ObservablePoint> VoltPoints = new List<ObservablePoint>();


            foreach (var Channel in ChannelsToUpdate)
            {
                var Points = WindowingDataHelper.GetWindowData(left, right, DataStorage.GetChannelData(Channel));

                if (ChannelTypeList.TokChannels.Contains(Channel))
                    AddPoints(TokPoints, Points);
                if (ChannelTypeList.VoltChannels.Contains(Channel))
                    AddPoints(VoltPoints, Points);
            }

            if (TokPoints.Count == 0 || VoltPoints.Count == 0)
                return SeriesToUpdate;

            var PowerPoints = PowerMathModule.Apply(TokPoints, VoltPoints);

            SerieData serieData = new SerieData();
            serieData.SerieTitle = $"Мощность";
            serieData.Points = PowerPoints.ToArray();

            SeriesToUpdate.Add(serieData);

            return SeriesToUpdate;
        }

        private void AddPoints(List<ObservablePoint> Points, ObservablePoint[] NewPoints)
        {
            if(Points.Count == 0)
            {
                for(int i = 0; i < NewPoints.Length; i++)
                {
                    Points.Add(new ObservablePoint(NewPoints[i].X, NewPoints[i].Y));
                }
                return;
            }

            for(int i = 0; i < NewPoints.Length; i++)
            {
                Points[i].Y += NewPoints[i].Y;
            }
        }
    }
}

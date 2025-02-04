using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMODBUS.ViewModels;

namespace TestMODBUS.Models.Chart.ChartInputModelus.Factories
{
    public class ChartInputSimpleFactory : AbstractChartInputFactory
    {
        public override ChartInputModuleBase GetModule(ChartInputType ChartType, ChartModel Chart)
        {
            switch(ChartType)
            {
                case ChartInputType.Standart:
                    return new ChartInputStandart(Chart);
                case ChartInputType.Energy: 
                    return new ChartInputEnergy(Chart);
                case ChartInputType.Power: 
                    return new ChartInputPower(Chart);
                default:
                    return null;
            }
        }
    }
}

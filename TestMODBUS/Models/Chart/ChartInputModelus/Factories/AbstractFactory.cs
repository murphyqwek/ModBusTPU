using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMODBUS.ViewModels;

namespace TestMODBUS.Models.Chart.ChartInputModelus.Factories
{
    public abstract class AbstractChartInputFactory
    {
        public abstract ChartInputModuleBase GetModule(ChartInputType ChartType, ChartModel Chart);
    }
}

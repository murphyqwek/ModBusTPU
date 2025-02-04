using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMODBUS.ViewModels.ChartViewModels
{
    public class ChartInputTypeViewModel
    {
        public string Name { get; }
        public ChartInputType ChartInputType { get; }

        public ChartInputTypeViewModel(ChartInputType ChartInputType)
        {
            this.ChartInputType = ChartInputType;
            Name = ChartInputTypeName.GetChartInputTypeName(ChartInputType);
        }
    }
}

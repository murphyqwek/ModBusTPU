using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMODBUS.Models.Chart.ChartInputModelus.Factories
{
    public enum ChartInputType
    {
        Standart,
        Energy,
        Power
    }

    public static class ChartInputTypeName
    {
        static public string GetChartInputTypeName(ChartInputType type)
        {
            switch (type)
            {
                case ChartInputType.Standart: return "Стандарт";
                case ChartInputType.Energy: return "Энергия";
                case ChartInputType.Power: return "Мощность";
                default: return "";
            }
        }

        static public IEnumerable<ChartInputType> GetValues()
        {
            return Enum.GetValues(typeof(ChartInputType)).Cast<ChartInputType>();
        }
    }
}

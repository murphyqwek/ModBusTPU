using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMODBUS.Models.Chart.ChartInputModelus.Factories;
using TestMODBUS.Models.ModbusSensor.Factories;

namespace TestMODBUS.Models.ModbusSensor
{
    public static class SensorTypeNameGetter
    {
        static public string GetSensorTypeName(SensorType Type)
        {
            switch (Type)
            {
                case SensorType.Standart: return "Стандарт";
                case SensorType.Energy: return "Энергия";
                case SensorType.Power: return "Мощность";
                default: return "";
            }
        }
    }
}

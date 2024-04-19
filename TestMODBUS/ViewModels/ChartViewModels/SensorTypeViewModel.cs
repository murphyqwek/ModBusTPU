using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMODBUS.Models.ModbusSensor;
using TestMODBUS.Models.ModbusSensor.Factories;

namespace TestMODBUS.ViewModels.ChartViewModels
{
    public class SensorTypeViewModel
    {
        public string Name { get; }
        public SensorType SensorType { get; }

        public SensorTypeViewModel(SensorType SensorType)
        {
            this.SensorType = SensorType;
            Name = SensorTypeNameGetter.GetSensorTypeName(SensorType);
        }
    }
}

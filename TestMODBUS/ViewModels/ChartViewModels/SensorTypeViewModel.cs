using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModBusTPU.Models.ModbusSensor;
using ModBusTPU.Models.ModbusSensor.Factories;

namespace ModBusTPU.ViewModels.ChartViewModels
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

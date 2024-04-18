using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMODBUS.Models.ModbusSensor.ChartDataPrepatations;
using TestMODBUS.Models.ModbusSensor.ModBusInputs;

namespace TestMODBUS.Models.ModbusSensor.Factories
{
    public enum SensorType
    {
        Standart,
        Power,
        Energy
    }

    public abstract class AbstractModbusSensorFactory
    {
        public abstract ChartDataPreparationBase GetChartDataPrepatation(SensorType SensorType);

        public abstract ModBusInputBase GetInputModule(SensorType SensorType, ModbusSensorController Controller, IEnumerable<int> Channels);

        public abstract ModBusInputBase GetInputModule(SensorType SensorType, ModbusSensorController Controller);
    }
}

using System;
using System.Collections.Generic;
using ModBusTPU.Models.ModbusSensor.ChartDataPrepatations;
using ModBusTPU.Models.ModbusSensor.ModBusInputs;

namespace ModBusTPU.Models.ModbusSensor.Factories
{
    public class ModbusSensorSimpleFactory : AbstractModbusSensorFactory
    {
        public override ChartDataPreparationBase GetChartDataPrepatation(SensorType SensorType)
        {
            switch(SensorType)
            {
                case SensorType.Standart:
                    return new ChartDataPreparationStandart();
                case SensorType.Power: 
                    return new ChartDataPreparationPower();
                case SensorType.Energy:
                    return new ChartDataPreparationEnergy();
                default:
                    throw new NotImplementedException();
            }
        }

        public override ModBusInputBase GetInputModule(SensorType SensorType, ModbusSensorController Controller, IEnumerable<int> Channels)
        {
            switch(SensorType)
            {
                case SensorType.Standart:
                    return new ModBusInputStandart(Controller, Channels);
                case SensorType.Power:
                    return new ModBusInputPower(Controller, Channels);
                case SensorType.Energy:
                    return new ModBusInputEnergy(Controller, Channels);
                default:
                    throw new NotImplementedException();
            }
        }

        public override ModBusInputBase GetInputModule(SensorType SensorType, ModbusSensorController Controller)
        {
            return GetInputModule(SensorType, Controller, null);
        }
    }
}

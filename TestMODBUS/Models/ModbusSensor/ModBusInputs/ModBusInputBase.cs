using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMODBUS.Models.ModbusSensor.ModBusInputs
{
    internal abstract class ModBusInputBase
    {
        protected ModbusSensorData _sensorData;

        public ModBusInputBase(ModbusSensorData SensorData) 
        { 
            _sensorData = SensorData;
        }

        public abstract IEnumerable<string> GetValuesNames(); 
    }
}

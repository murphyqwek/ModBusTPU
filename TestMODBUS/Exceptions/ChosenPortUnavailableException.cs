using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBusTPU.Exceptions
{
    public class ChosenPortUnavailableException : Exception
    {
        public ChosenPortUnavailableException(string PortName) : base(string.Format("Порт {0} не доступен. Выберите другой", PortName)) { }
    }
}

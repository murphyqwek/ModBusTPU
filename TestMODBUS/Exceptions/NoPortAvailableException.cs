using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMODBUS.Exceptions
{
    public class NoPortAvailableException: Exception
    {
        public NoPortAvailableException() : base("Ни один порт не доступен"){ }
    }
}

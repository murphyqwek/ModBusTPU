using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBusTPU.Exceptions
{
    public class ProfileIsDamaged : Exception
    {
        public ProfileIsDamaged() : base("Профиль поврежден") { }
    }
}

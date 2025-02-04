using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBusTPU.Exceptions
{
    public class ProfileDoesNotExist : Exception
    {
        public ProfileDoesNotExist() : base("Профиль не существует") { }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMODBUS.Exceptions
{
    public class NotAllChannelsChosen : Exception
    {
        public NotAllChannelsChosen() : base("Не все нужные порты были выбраны") { }
    }
}

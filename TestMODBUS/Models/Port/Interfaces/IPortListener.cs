using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMODBUS.Models.Port.Interfaces
{
    public interface IPortListener
    {
        void StartListen(int delay);
        void StopListen();
    }
}

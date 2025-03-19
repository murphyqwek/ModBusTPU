using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBusTPU.Services.Settings.Serialization
{
    internal interface ISerializator<T>
    {
        string Serialize(T serialazable);

        T Deserialize(string serialazable);
    }
}

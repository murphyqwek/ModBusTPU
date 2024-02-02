using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMODBUS.Models.Channels
{
    //Временный класс, который хранит номера портов тока и напряжения
    public class ChannelTypeList
    {
        static public HashSet<int> TokChannels = new HashSet<int>()
        {
            0, 1, 2, 3,
        };

        static public HashSet<int> VoltChannels = new HashSet<int>()
        {
            5, 6, 7,
        };
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMODBUS.Models.Services
{
    //Временный класс, который хранит номера портов тока и напряжения
    public static class ChannelTypeList
    {
        public static HashSet<int> TokChannels = new HashSet<int>()
        {
            0, 1, 2, 3,
        };

        public static HashSet<int> VoltChannels = new HashSet<int>()
        {
            5, 6, 7,
        };
    }
}

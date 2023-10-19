﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMODBUS.Models.Port
{
    class ListAvailableSpeeds
    {
        public static List<int> ListPortSpeeds = new List<int>()
        {
            300,
            600,
            1200,
            2400,
            4800,
            9600,
            14400,
            19200,
            28800,
            31250,
            38400,
            57600,
            115200,
        };

        public static int GetStandartSpeed()
        {
            return 9600;
        }
    }
}

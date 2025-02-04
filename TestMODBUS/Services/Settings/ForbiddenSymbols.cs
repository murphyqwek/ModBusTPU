using ModBusTPU.Services.Settings.Channels;
using ModBusTPU.Services.Settings.Export;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMODBUS.Services.Settings
{
    public static class ForbiddenSymbols
    {
        public static readonly char[] LabelsForbiddenSymbols = new[] { ExportSaving.SPECSYMBOLFORREPLACINGBACKSPACE };


        public static bool CheckLabelForForbiddenSymbols(string Label)
        {
            foreach(var symbol in LabelsForbiddenSymbols)
            {
                if(Label.Contains(symbol)) return true;
            }

            return false;
        }
    }
}

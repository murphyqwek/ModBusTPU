using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TestMODBUS.Models.Data;

namespace TestMODBUS.Models.Services.Settings.Data
{
    public static class DataLog
    {
        public static string GetLog(DataStorage DataStorage)
        {
            return DataSaver.GetDataLog(DataStorage);
        }
        
        public static DataStorage ReadLog(string LogText)
        { 
            DataStorage dataStorage = DataReader.ReadLog(LogText);
            return dataStorage;
        }

    }
}

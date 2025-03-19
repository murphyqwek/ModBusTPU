using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ModBusTPU.Services.Settings
{
    internal class SettingsManager
    {
        private string _configPath;

        private static JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true, //Красивый формат
            PropertyNameCaseInsensitive = true, // Позволяет игнорировать регистр полей
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull // Игнорирует null-поля
        };

        public string ConfigPath { get { return _configPath; } }

        public SettingsManager(string configPath)
        {
            _configPath = configPath;
        }

        
    }
}

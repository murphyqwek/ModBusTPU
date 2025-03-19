using ModBusTPU.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ModBusTPU.Services.Settings.Serialization
{
    internal class JsonSerializator<T> : ISerializator<T>
    {

        private static JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true, //Красивый формат
            PropertyNameCaseInsensitive = true, // Позволяет игнорировать регистр полей
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull // Игнорирует null-поля
        };

        public T Deserialize(string serialazable)
        {
            var settings = JsonSerializer.Deserialize<T>(serialazable, JsonSerializerOptions);
            return settings;
        }

        public string Serialize(T serialazable)
        {
            string json = JsonSerializer.Serialize(serialazable, JsonSerializerOptions);
            return json;
        }
    }
}

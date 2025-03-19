using ModBusTPU.Services.Settings.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ModBusTPU.Services.Settings
{
    internal class SettingsManager
    {
        private string _configPath;

        private ISerializator<SettingsContainer> _serializator;

        public string ConfigPath { get { return _configPath; } }

        public SettingsManager(string configPath, ISerializator<SettingsContainer> serializator)
        {
            _configPath = configPath;
            _serializator = serializator;
        }

        /// <summary>
        /// Метод для загрузки файла настроек
        /// </summary>
        /// <returns>Возвращает SettingsContainer и текст ошибки, если она произошла. Если файл загружен успено, текст ошибки будет равен string.Empty. Если не удалось загрузить файл, то SettingsContainer будет иметь стандартные настройки</returns>
        public (SettingsContainer settings, string exceptionTest) Upload()
        {
            if(!File.Exists(ConfigPath))
            {
                try
                {
                    var Settings = new SettingsContainer();
                    Save(Settings);
                }
                catch (IOException)
                {
                    return (GetDefaultSettings(), "Не удалось открыть файл");
                }
                catch (Exception ex)
                {
                    return (GetDefaultSettings(), "Произошла непредвиденная ошибка: " + ex.Message);
                }
            }

            try
            {
                string json = File.ReadAllText(ConfigPath);
                var settings = _serializator.Deserialize(json);

                if (settings == null)
                {
                    return (GetDefaultSettings(), "Не удалось десериализовать настройки. Возвращены дефолтные значения.");
                }

                return (settings, string.Empty);
            }
            catch (IOException)
            {
                return (GetDefaultSettings(), "Не удалось открыть файл");
            }
            catch (Exception ex)
            {
                return (GetDefaultSettings(), "Произошла непредвиденная ошибка: " + ex.Message);
            }
        }

        /// <summary>
        /// Метод для сохранения настроек
        /// </summary>
        /// <param name="settings"> Контейнер с настройками </param>
        public void Save(SettingsContainer settings)
        {
            string json = _serializator.Serialize(settings);
            File.WriteAllText(json, ConfigPath);
        }

        /// <summary>
        /// Метод для получения стандартных настроек
        /// </summary>
        private SettingsContainer GetDefaultSettings()
        {
            return new SettingsContainer();
        }
    }
}

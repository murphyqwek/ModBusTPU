using ModBusTPU.Services.Settings.Serialization;
using ModBusTPU.Services.Settings.SetingsContainer;
using ModBusTPU.Services.Settings.SettingsContainer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ModBusTPU.Services.Settings
{
    /// <summary>
    /// Класс для загрузки и сохранения настроек
    /// </summary>
    internal class SettingsManager<T> where T : BaseSettingsContainer, new()
    {
        private string _configPath;

        private ISerializator<T> _serializator;

        public string ConfigPath { get { return _configPath; } }

        public SettingsManager(string configPath, ISerializator<T> serializator)
        {
            _configPath = configPath;
            _serializator = serializator;
        }

        /// <summary>
        /// Метод для загрузки файла настроек
        /// </summary>
        /// <returns>Возвращает контейнер с настройками и текст ошибки, если она произошла. Если файл загружен успено, текст ошибки будет равен string.Empty. Если не удалось загрузить файл, то SettingsContainer будет иметь стандартные настройки</returns>
        public (T settings, string exceptionTest) Upload()
        {
            if(!File.Exists(ConfigPath))
            {
                try
                {
                    var Settings = GetDefaultSettings();
                    Save(Settings);
                    return (Settings, string.Empty);
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

                settings.EnsureDefualt();

                if(!settings.isValid()) {
                    return (GetDefaultSettings(), "Файл содержит невалидные значения.");
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
        public void Save(T settings)
        {
            string json = _serializator.Serialize(settings);
            File.WriteAllText(json, ConfigPath);
        }

        /// <summary>
        /// Метод для получения стандартных настроек
        /// </summary>
        private T GetDefaultSettings()
        {
            return new T();
        }
    }
}

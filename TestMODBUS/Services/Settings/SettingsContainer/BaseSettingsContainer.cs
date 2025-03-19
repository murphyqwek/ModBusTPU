using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBusTPU.Services.Settings.SettingsContainer
{
    /// <summary>
    /// Базовый класс для контейнеров, которые содержат настройки
    /// </summary>
    internal abstract class BaseSettingsContainer
    {
        public BaseSettingsContainer() 
        {
            SetDefaultSettingsContainer();    
        }

        /// <summary>
        /// Метод для установки стандартных настроек
        /// </summary>
        public abstract void SetDefaultSettingsContainer();

        /// <summary>
        /// Метод, который устанавливает неустановленные настройки в стандартные значения
        /// </summary>
        public abstract void EnsureDefualt();

        /// <summary>
        /// Метод для проверки валидности данных
        /// </summary>
        /// <returns>true, если данные валидны, false, если данные невалидны</returns>
        public abstract bool isValid();
    }
}

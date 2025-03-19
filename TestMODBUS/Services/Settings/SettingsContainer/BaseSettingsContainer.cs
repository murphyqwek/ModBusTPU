using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBusTPU.Services.Settings.SettingsContainer
{
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
    }
}

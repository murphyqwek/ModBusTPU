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

        public abstract void SetDefaultSettingsContainer();
    }
}

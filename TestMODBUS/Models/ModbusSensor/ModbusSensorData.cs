using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TestMODBUS.Models.Services;

namespace TestMODBUS.Models.ModbusSensor
{
    internal class ModbusSensorData
    {
        public ObservableCollection<string> CurrentValues { get; }
        public ObservableCollection<bool> UsingPorts { get; }

        private Dictionary<string, int> CurrentValuesIndexDictionary = new Dictionary<string, int>();

        public ModbusSensorData() 
        {
            CurrentValues = new ObservableCollection<string>();
            UsingPorts = new ObservableCollection<bool>();

            SetUpUsingPorts();
        }

        private void SetUpUsingPorts()
        {
            for(int i = 0; i < ChannelColors.Colors.Count; i++)
            {
                UsingPorts.Add(false);
            }
        }

        public void SetUpCurrentValues(IEnumerable<string> NameofValues)
        {
            CurrentValues.Clear();
            CurrentValuesIndexDictionary.Clear();

            int i = 0;
            foreach(var valueName in NameofValues)
            {
                CurrentValues.Add("");
                CurrentValuesIndexDictionary.Add(valueName, i);
                i++;
            }
        }

        public void SetCurrentValue(string ValueName, string Value)
        {
            if (!CurrentValuesIndexDictionary.ContainsKey(ValueName))
                throw new ArgumentException($"Doesn't contain value named {ValueName}");

            int index = CurrentValuesIndexDictionary[ValueName];
            CurrentValues[index] = Value;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using TestMODBUS.Models.Services;

namespace TestMODBUS.Models.ModbusSensor
{
    public class ModbusSensorData
    {
        public ObservableCollection<string> CurrentValues { get; }
        public ObservableCollection<bool> UsingChannels { get; }

        public ModbusSensorData() 
        {
            CurrentValues = new ObservableCollection<string>();
            UsingChannels = new ObservableCollection<bool>();

            SetUpUsingChannels();
        }

        private void SetUpUsingChannels()
        {
            for(int i = 0; i < ChannelColors.Colors.Count; i++)
            {
                UsingChannels.Add(false);
            }
        }

        public void UnusedAllChannels()
        {
            for (int i = 0; i < UsingChannels.Count; i++)
                SetUsingChannel(i, false);
        }

        public void ClearCurrentValues()
        {
            Application.Current.Dispatcher.Invoke(() => CurrentValues.Clear());
        }

        public void SetCurrentValues(IEnumerable<string> values)
        {
            ClearCurrentValues();
            //TODO maybe not work
            foreach(var value in values)
                Application.Current.Dispatcher.Invoke(() => CurrentValues.Add(value));
        }

        public void SetUsingChannel(int Channel, bool State)
        {
            if(Channel < 0 || Channel > 7)
                throw new ArgumentOutOfRangeException(nameof(Channel));

            UsingChannels[Channel] = State;
        }

        public List<int> GetUsingChannels()
        {
            List<int> UsingChannelsInt = new List<int>();

            for(int i = 0; i < UsingChannels.Count; i++)
            {
                if (UsingChannels[i])
                    UsingChannelsInt.Add(i);
            }

            return UsingChannelsInt;
        }

        public bool GetChannelUsingState(int Channel)
        {
            return UsingChannels[Channel];
        }
    }
}

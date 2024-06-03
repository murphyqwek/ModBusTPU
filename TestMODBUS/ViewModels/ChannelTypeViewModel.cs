using System;
using System.Collections.Generic;
using System.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModBusTPU.Models.Services;
using ModBusTPU.ViewModels.Base;
using System.Windows.Media;
using TestMODBUS.Services.Channels;

namespace ModBusTPU.ViewModels
{
    public class ChannelTypeViewModel : BaseViewModel
    {
        #region Public Attributies
        
        public List<string> ChannelsType { get; }
        public Brush BackgroundColor
        {
            get => _backgroundColor;
            private set
            {
                _backgroundColor = value;
                OnPropertyChanged();
            }
        }
        public Brush ForegorundColor
        {
            get => _foregroundColor;
            set
            {
                _foregroundColor = value;
                OnPropertyChanged();
            }
        }


        public int Channel => _channelNumber;

        public string CurrentChannelType
        {
            get => _currentChannelType;
            set
            {
                _currentChannelType = value;
                _channelType = ChannelTypesDictionary[_currentChannelType];
                BackgroundColor = ChannelTypeColors.GetBackgroundColor(ChannelType, true);
                ForegorundColor = ChannelTypeColors.GetForegroundColor(ChannelType, true);
            }
        }
        public ChannelType ChannelType 
        {
            get => _channelType;
            private set
            {
                if (_channelType == value)
                    return;
                _channelType = value;

                BackgroundColor = ChannelTypeColors.GetBackgroundColor(ChannelType, true);
                ForegorundColor = ChannelTypeColors.GetForegroundColor(ChannelType, true);
                OnPropertyChanged();
            }
        }

        #endregion

        #region Private Fields

        private Brush _backgroundColor;
        private Brush _foregroundColor;
        private int _channelNumber;
        private string _currentChannelType;
        private ChannelType _channelType;

        #endregion


        private Dictionary<string, ChannelType> ChannelTypesDictionary = new Dictionary<string, ChannelType>()
        {
            {"Обычный", ChannelType.Regular},
            {"Ток", ChannelType.Tok },
            {"Напряжение", ChannelType.Volt }
        };

        public ChannelTypeViewModel(int Channel, ChannelType ChannelType)
        {
            _channelNumber = Channel;
            CurrentChannelType = ChannelTypesDictionary.FirstOrDefault(x => x.Value == ChannelType).Key;
            ChannelsType = ChannelTypesDictionary.Keys.ToList();
        }
    }
}

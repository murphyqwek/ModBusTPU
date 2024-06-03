using System;
using System.Collections.Generic;
using System.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModBusTPU.Models.Services;
using ModBusTPU.ViewModels.Base;
using System.Windows.Media;

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
        public int Channel => _channelNumber;
        public string CurrentChannelType
        {
            get => _currentChannelType;
            set
            {
                _currentChannelType = value;
                _channelType = ChannelTypesDictionary[_currentChannelType];
                BackgroundColor = ChannelTypeBackgroundDictionary[_channelType];
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
                OnPropertyChanged();
            }
        }

        #endregion

        #region Private Fields

        private Brush _backgroundColor;
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

        private Dictionary<ChannelType, Brush> ChannelTypeBackgroundDictionary = new Dictionary<ChannelType, Brush>()
        {
            {ChannelType.Regular, new SolidColorBrush(Color.FromRgb(247, 245, 245))},
            {ChannelType.Tok, new SolidColorBrush(Color.FromRgb(108, 139, 186))},
            {ChannelType.Volt, new SolidColorBrush(Color.FromRgb(240, 12, 42))}
        };


        public ChannelTypeViewModel(int Channel, ChannelType ChannelType)
        {
            CurrentChannelType = ChannelTypesDictionary.FirstOrDefault(x => x.Value == ChannelType).Key;
            _channelNumber = Channel;
            ChannelsType = ChannelTypesDictionary.Keys.ToList();
        }
    }
}

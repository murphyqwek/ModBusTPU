using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModBusTPU.ViewModels.Base;

namespace ModBusTPU.Models.Data
{
    public class ChannelModel : BaseViewModel
    {
        public int ChannelNumber { get; }
        public string Label 
        { 
            get => _label; 
            set
            {
                if (_label == value)
                    return;
                _label = value;
                OnPropertyChanged();
            }
        }
        public bool IsChosen
        {
            get => _isChosen;
            set
            {
                if (_isChosen == value) return;
                _isChosen = value;
                OnPropertyChanged();
            }
        }

        private bool _isChosen = false;
        private string _label;

        public ChannelModel(int ChannelNumber)
        {
            this.ChannelNumber = ChannelNumber;
            Label = $"CH_{ChannelNumber}";
        }

        public ChannelModel(int ChannelNumber, bool IsChosen)
        {
            this.ChannelNumber = ChannelNumber;
            this.IsChosen = IsChosen;
            this.Label = $"CH_{ChannelNumber}";
        }

        public ChannelModel(int ChannelNumber, bool IsChosen, string Label)
        {
            this.ChannelNumber = ChannelNumber;
            this.IsChosen = IsChosen;
            this.Label = Label;
        }
    }
}

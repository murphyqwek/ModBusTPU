using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModBusTPU.Models.Data;
using ModBusTPU.ViewModels.Base;

namespace ModBusTPU.ViewModels
{
    public class ChannelViewModel : BaseViewModel
    {
        public bool IsChosen 
        {
            get => _model.IsChosen;
            set
            {
                _model.IsChosen = value;
                OnPropertyChanged();
            }
        }
        public string Label
        {
            get => _model.Label;
            set
            {
                _model.Label = value;
                OnPropertyChanged();
            }
        }

        public int ChannelNumber { get => _model.ChannelNumber; }

        private ChannelModel _model;

        public ChannelViewModel(ChannelModel Channel) 
        {
            _model = Channel;
            _model.PropertyChanged += (s, e) => OnPropertyChanged(e.PropertyName);
        }
    }
}

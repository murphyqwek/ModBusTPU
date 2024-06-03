using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ModBusTPU.Models.Data;
using ModBusTPU.ViewModels.Base;
using TestMODBUS.Services.Channels;

namespace ModBusTPU.ViewModels
{
    public class ChannelViewModel : BaseViewModel
    {
        public Brush Background => _background;
        public Brush Foreground => _foreground;

        public bool IsChosen 
        {
            get => _model.IsChosen;
            set
            {
                _model.IsChosen = value;

                UpdateColor();
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
        private Brush _background;
        private Brush _foreground;

        public ChannelViewModel(ChannelModel Channel) 
        {
            _model = Channel;
            _model.PropertyChanged += (s, e) => OnPropertyChanged(e.PropertyName);
            UpdateColor();
        }

        public void UpdateColor()
        {
            _background = ChannelTypeColors.GetBackgroundColor(ChannelNumber, IsChosen);
            _foreground = ChannelTypeColors.GetForegroundColor(ChannelNumber, IsChosen);

            OnPropertyChanged(nameof(Background));
            OnPropertyChanged(nameof(Foreground));
        }
    }
}

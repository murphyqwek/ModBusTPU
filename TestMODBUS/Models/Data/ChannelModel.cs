using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMODBUS.ViewModels.Base;

namespace TestMODBUS.Models.Data
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
        public Collection<Point> Data;
        private string _label;

        public ChannelModel(int ChannelNumber, List<Point> ChannelData)
        {
            Data = new Collection<Point>(ChannelData);
            this.ChannelNumber = ChannelNumber;
            Label = $"CH_{ChannelNumber}";
        }
    }
}

using ModBusTPU.Models.Coefficients;
using ModBusTPU.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBusTPU.ViewModels
{
    public class CoefficientProfileViewModel : BaseViewModel
    {
        #region Attributes
        public double HolostMove 
        {
            get => _profile.HolostMove;
            set
            {
                _profile.HolostMove = value;
                OnPropertyChanged();
                _isSaved = false;
            }
        }

        public double AmperKoeff
        {
            get => _profile.AmperKoeff;
            set
            {
                _profile.AmperKoeff = value;
                OnPropertyChanged();
                _isSaved = false;
            }
        }

        public double VoltKoeff
        {
            get => _profile.VoltKoeff;
            set
            {
                _profile.VoltKoeff = value;
                OnPropertyChanged();
                _isSaved = false;
            }
        }

        public double KoeffValueChannel
        {
            get => _profile.KoeffValueChannel;
            set
            {
                _profile.KoeffValueChannel = value;
                OnPropertyChanged();
                _isSaved = false;
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
                _isSaved = false;
                _isRenamed = true;
            }
        }

        public bool IsSaved
        {
            get => _isSaved;
            set
            {
                if (_isSaved == value)
                    return;

                _isSaved = value;
                OnPropertyChanged();
            }
        }

        public bool IsRenamed
        {
            get => _isRenamed;
            set
            {
                if (_isRenamed == value)
                    return;

                _isRenamed = value;
                OnPropertyChanged();

                if (_isRenamed == true)
                    PreviousName = Name;
            }
        }

        public string PreviousName
        {
            get => _previousName;
            set
            {
                if (_previousName == value)
                    return;

                _previousName = value;
                OnPropertyChanged();
            }
        }
        #endregion

        private string _name;
        private bool _isSaved;
        private CoefficientProfile _profile;
        private bool _isRenamed;
        private string _previousName;

        public CoefficientProfile GetProfile() => _profile;

        public CoefficientProfileViewModel(string Name, CoefficientProfile Profile)
        {
            _name = Name;
            PreviousName = Name;

            _profile = Profile;

            _isSaved = true;
            _isRenamed = false;
        }
    }
}

using ModBusTPU.Commands;
using ModBusTPU.Models.Coefficients;
using ModBusTPU.Models.MessageBoxes;
using ModBusTPU.Services;
using ModBusTPU.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ModBusTPU.ViewModels.Settings
{
    public class CoefficientProfileSettingsViewModel : BaseViewModel
    {
        #region Attributes
        public ObservableCollection<CoefficientProfileViewModel> CoefficientProfiles { get; }

        public CoefficientProfileViewModel CurrentCoefficientProfile 
        { 
            get => _currentCoefficientProfile; 
            set
            {
                if (_currentCoefficientProfile == value)
                    return;

                _currentCoefficientProfile = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ProfileUnchosen));
            } 
        }

        public Visibility ProfileUnchosen => CurrentCoefficientProfile != null ? Visibility.Visible : Visibility.Hidden;
        #endregion

        #region Private Fields
        private CoefficientProfileViewModel _currentCoefficientProfile;
        #endregion

        #region Commands

        #region Upload Command
        
        public ICommand UploadCommand { get; }

        private void UploadCommandHandler()
        {
            Coefficients.SetCoefficients(CurrentCoefficientProfile.GetProfile());

            SuccessMessageBox.Show("Профиль успешно загружен");
        }

        #endregion

        #endregion

        public CoefficientProfileSettingsViewModel() 
        {
            CoefficientProfiles = new ObservableCollection<CoefficientProfileViewModel>();

            UploadCommand = new RemoteCommand(UploadCommandHandler);

            UploadProfiles();
        }

        private void UploadProfiles()
        {
            CoefficientProfiles.Clear();
            foreach(var ProfileName in CoefficientProfileFileManager.GetAllProfilesNames())
            {
                try
                {
                    var Profile = CoefficientProfileFileManager.GetProfileData(ProfileName);

                    CoefficientProfiles.Add(new CoefficientProfileViewModel(ProfileName, Profile));
                }
                catch { continue; }
            }
        }
    }
}

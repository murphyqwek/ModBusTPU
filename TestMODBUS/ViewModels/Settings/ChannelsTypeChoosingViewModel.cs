﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using ModBusTPU.Commands;
using ModBusTPU.Models.MessageBoxes;
using ModBusTPU.Models.Services;
using ModBusTPU.Services.Settings.Channels;
using ModBusTPU.ViewModels.Base;
using System.Windows;

namespace ModBusTPU.ViewModels
{
    public class ChannelsTypeChoosingViewModel : BaseViewModel
    {
        #region Public Attribuites
        public ObservableCollection<ChannelTypeViewModel> Channels { get; }
        public bool IsSaved
        {
            get => _isSaved;
            private set
            {
                _isSaved = value;
                OnPropertyChanged();
            }
        }
        public bool IsApplied
        {
            get => _isApplied;
            private set
            {
                _isApplied = value;
                OnPropertyChanged();
            }
        }

        public bool IsChanged = false;
        #endregion

        #region Private Fields
        private bool _isApplied = true;
        private bool _isSaved = true;
        #endregion

        #region Commands


        #region Apply Command
        public ICommand ApplyCommand { get; }

        private void ApplyCommandHandler()
        {
            ChannelsTypeSettings.SetUserChannelsType(GetChannelsType());

            SuccessMessageBox.Show("Настройки успешно загружены");
            IsApplied = true;
        }
        #endregion

        #region Save Command

        public ICommand SaveCommand { get; }

        private void SaveCommandHandler()
        {
            try
            {
                if (!ChannelsSettingFileManager.SaveSettings(GetChannelsType()))
                    return;
                SuccessMessageBox.Show("Настройки сохранены");
                IsSaved = true;
            }
            catch (Exception ex)
            {
                ErrorMessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region Upload Command

        public ICommand UploadCommand { get; }

        private void UploadCommandHandler()
        {
            try
            {
                if (!IsSaved)
                {
                    if (RequestYesNoMessageBox.Show("Текущие настройки не сохранены. Вы уверены, что хотите продолжить (текущие настройки не сохранянтся)?") != System.Windows.MessageBoxResult.Yes)
                        return;
                }

                if (!ChannelsSettingFileManager.UploadUserSettings())
                    return;

                IsSaved = true;
                IsApplied = true;
                UploadChannels();
                SuccessMessageBox.Show("Настрйоки загружены и применены");
            }
            catch(Exception ex)
            {
                ErrorMessageBox.Show(ex.Message);
            }
        }

        #endregion

        #endregion

        #region Events

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            if (!IsSaved)
            {
                var answer = RequestYesNoMessageBox.Show("Текущие настройки не сохранены, вы хотите их сохранить?");
                if (answer == MessageBoxResult.Yes)
                    SaveCommandHandler();
                else if (answer == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }

            if(!IsApplied)
            {
                var answer = RequestYesNoMessageBox.Show("Текущие настройки не применены, вы хотите их применить?");
                if (answer == MessageBoxResult.Yes)
                    ApplyCommandHandler();
                else if (answer == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }

        #endregion

        public ChannelsTypeChoosingViewModel()
        {
            Channels = new ObservableCollection<ChannelTypeViewModel>();
            UploadChannels();

            //Intialaize commands
            ApplyCommand = new RemoteCommand(ApplyCommandHandler);
            SaveCommand = new RemoteCommand(SaveCommandHandler);
            UploadCommand = new RemoteCommand(UploadCommandHandler);
        }

        private void UploadChannels()
        {
            Channels.Clear();

            for (int i = 0; i < ChannelTypeList.ChannelCounts; i++)
            {
                var Channel = new ChannelTypeViewModel(i, ChannelTypeList.GetChannelType(i));
                Channel.PropertyChanged += OnChannelTypeChanged;
                Channels.Add(Channel);
            }
        }

        private void OnChannelTypeChanged(object sender, PropertyChangedEventArgs e)
        {
            IsApplied = false;
            IsSaved = false;
        }

        private List<ChannelType> GetChannelsType()
        {
            List<ChannelType> Types = new List<ChannelType>();
            foreach(var ChannelType in Channels)
            {
                Types.Add(ChannelType.ChannelType);
            }

            return Types;
        }
    }
}

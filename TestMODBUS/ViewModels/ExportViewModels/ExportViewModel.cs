using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TestMODBUS.Commands;
using TestMODBUS.Models;
using TestMODBUS.Models.Data;
using TestMODBUS.Models.MessageBoxes;
using TestMODBUS.Services.Settings;
using TestMODBUS.ViewModels.Base;
using TestMODBUS.Models.Services;
using TestMODBUS.Models.Services.Excel;
using TestMODBUS.Services.Settings.Channels;
using TestMODBUS.Models.ModbusSensor.ChartDataPrepatations;
using TestMODBUS.Models.ModbusSensor.ModBusInputs.ChannelsFilters;

namespace TestMODBUS.ViewModels.ExportViewModels
{
    public class ExportViewModel : BaseViewModel
    {
        #region Public Attributes

        public ObservableCollection<ExtraDataViewModel> PowerExtraData { get; }
        public ObservableCollection<ExtraDataViewModel> EnergyExtraData { get; }

        public ObservableCollection<ChannelViewModel> ExportChannels { get; }
        public string FileName { get; }

        #endregion

        private List<ChannelModel> _channelsExportSettings = new List<ChannelModel>();

        #region Commands
        #region Export Data
        public ICommand ExportDataCommand { get; }

        private void ExportDataHandle()
        {
            if(!_channelsExportSettings.Any(channel => channel.IsChosen))
            {
                ErrorMessageBox.Show("Нужно выбрать как минимум один канал");
                return;
            }

            string path = OpenFileHelper.GetSaveFile("*.xlsx|*.xlsx;", ".xlsx", FileName);
            if (path == null)
                return;

            try
            {
                ExportExcel.SaveData(_channelsExportSettings, path);
                if(RequestYesNoMessageBox.Show("Отчёт сохранён. Открыть папку с отчётом?", "Успешно", System.Windows.MessageBoxImage.Information) == System.Windows.MessageBoxResult.Yes)
                {
                    string folder = Path.GetDirectoryName(path);
                    Process.Start("explorer", folder);
                }

            }
            catch(Exception e)
            {
                ErrorMessageBox.Show(e.Message);
            }
        }
        #endregion

        #region Save Settings

        public ICommand SaveSettingsCommand { get; }
        public void SaveSettingsCommandHandle()
        {
            if (ExportChannelsSettings.SaveChannels(_channelsExportSettings))
                SuccessMessageBox.Show("Настройки экспорта каналов сохранены");
            else
                ErrorMessageBox.Show("Не удалось сохранить настройки экспорта каналов");
        }

        #endregion

        #region Upload Settings

        public ICommand UploadSettingsCommand { get; }

        public void UploadSettingsHandle()
        {
            ExportChannelsSettings.UploadChannelSettings(_channelsExportSettings);
        }

        #endregion

        #region Clear Channels Export Settings

        public ICommand ClearChannelsExportSettingsCommand { get; }

        public void ClearChannelsExportSettingsHandle()
        {
            if (RequestYesNoMessageBox.Show("Вы уверены, что хотите очистить каналы?") != MessageBoxResult.Yes)
                return;

            ClearChannelsExportSettings();
        }

        #endregion

        #region Add New Power ExtraData

        public ICommand AddNewPowerExtraDataCommand { get; }

        public void AddNewPowerExtraDataCommandHander()
        {
            var newExtraData = new ExtraDataViewModel(new ChartDataPreparationPower(), new OnlyOneVoltAndSeveralTokFilter(),
                                                      "Power", DeleteExtraData);
            PowerExtraData.Add(newExtraData);
        }

        #endregion

        #region Add New Energy ExtraData

        public ICommand AddNewEnergyExtraDataCommand { get; }

        public void AddNewEnergyExtraDataCommandHandler()
        {
            var newExtraData = new ExtraDataViewModel(new ChartDataPreparationEnergy(), new OnlyOneVoltAndSeveralTokFilter(),
                                                      "Energy", DeleteExtraData);
            EnergyExtraData.Add(newExtraData);
        }

        #endregion

        #endregion

        public ExportViewModel(DataStorage Data, string FileName) 
        {
            this.FileName = FileName;
            ExportChannels = new ObservableCollection<ChannelViewModel>();
            UploadChannelsData(Data);

            PowerExtraData = new ObservableCollection<ExtraDataViewModel>();
            EnergyExtraData = new ObservableCollection<ExtraDataViewModel>();

            ExportDataCommand = new RemoteCommand(ExportDataHandle);
            SaveSettingsCommand = new RemoteCommand(SaveSettingsCommandHandle);
            UploadSettingsCommand = new RemoteCommand(UploadSettingsHandle);
            ClearChannelsExportSettingsCommand = new RemoteCommand(ClearChannelsExportSettingsHandle);
            AddNewPowerExtraDataCommand = new RemoteCommand(AddNewPowerExtraDataCommandHander);
            AddNewEnergyExtraDataCommand = new RemoteCommand(AddNewEnergyExtraDataCommandHandler);
        }

        private void UploadChannelsData(DataStorage Data)
        {
            for(int i = 0; i < Data.GetMaxChannelsCount(); i++) 
            {
                ChannelModel channelModel = new ChannelModel(i, Data.GetChannelData(i).ToList());
                _channelsExportSettings.Add(channelModel);
                ExportChannels.Add(new ChannelViewModel(channelModel));
            }

            ExportChannelsSettings.UploadChannelSettings(_channelsExportSettings);
        }

        private void ClearChannelsExportSettings()
        {
            foreach (var channel in _channelsExportSettings)
            {
                channel.Label = "";
                channel.IsChosen = false;
            }
        }

        private void DeleteExtraData(ExtraDataViewModel ExtraData)
        {
            if(ExtraData.Type == "Power")
                PowerExtraData.Remove(ExtraData);
            if (ExtraData.Type == "Energy")
                EnergyExtraData.Remove(ExtraData);
        }
    }
}

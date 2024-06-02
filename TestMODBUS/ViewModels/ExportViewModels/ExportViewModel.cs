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
using TestMODBUS.Services.Excel;
using TestMODBUS.Services.Settings.Export;

namespace TestMODBUS.ViewModels.ExportViewModels
{
    public class ExportViewModel : BaseViewModel
    {
        #region Public Attributes

        public ObservableCollection<ExtraDataViewModel> PowerExtraData { get; }
        public ObservableCollection<ExtraDataViewModel> EnergyExtraData { get; }
        public ObservableCollection<CommentaryExportElementViewModel> Commentaries { get; }
        public ObservableCollection<ChannelViewModel> ChannelsData { get; }

        public string BigComment 
        {
            get => _bigComment;
            set
            {
                if(_bigComment == value) return;
                _bigComment = value;
                OnPropertyChanged();
            }
        }
        #endregion

        private DataStorage _dataStorage;
        private string _bigComment;

        #region Commands
        #region Export Data
        public ICommand ExportDataCommand { get; }

        private void ExportDataHandle()
        {
            if(!ChannelsData.Any(channel => channel.IsChosen))
            {
                ErrorMessageBox.Show("Нужно выбрать как минимум один канал");
                return;
            }

            if (!IsAllChannelsChosen())
            {
                ErrorMessageBox.Show("Не все поля заполены корректно");
                return;
            }

            if (Commentaries.Any(commentary => string.IsNullOrEmpty(commentary.Commentary)))
            {
                var response = RequestYesNoMessageBox.Show("Не все внешние данные заполнены, продолжить?");

                if (response != MessageBoxResult.Yes)
                    return;
            }

            if (string.IsNullOrEmpty(BigComment))
            {
                var response = RequestYesNoMessageBox.Show("Комменатарии не заполнены, продолжить?");

                if (response != MessageBoxResult.Yes)
                    return;
            }

            string path = FileHelper.GetSaveFilePath("*.xlsx|*.xlsx;", ".xlsx", "Отчёт");
            if (path == null)
                return;

            try
            {
                Export(path);
                if (RequestYesNoMessageBox.Show("Отчёт сохранён. Открыть его?", "Успешно", MessageBoxImage.Information) == MessageBoxResult.Yes)
                    Process.Start(path);
            }
            catch(Exception e)
            {
                ErrorMessageBox.Show(e.Message);
            }
        }

        private void Export(string FilePath)
        {
            var RawDataPage = ExcelDataPreparation.ExtractRawData(_dataStorage);
            var ChannelsDataPage = ExcelDataPreparation.ExtractChannelsData(ChannelsData, _dataStorage);
            var ChannelsToChart = ExcelDataPreparation.GetChannelsToChartList(ChannelsData);

            var ExtractedCommentaries = ExcelDataPreparation.ExtractCommentaries(Commentaries);

            Dictionary<string, IEnumerable<ExtraDataViewModel>> ExtraData = new Dictionary<string, IEnumerable<ExtraDataViewModel>>()
            {
                { "Мощность", PowerExtraData },
                { "Энергия", EnergyExtraData }
            };

            var ExtraDataPages = ExcelDataPreparation.ExtractExtraData(ExtraData, _dataStorage);

            ExcelExport.SaveData(RawDataPage, ChannelsDataPage, ChannelsToChart, ExtraDataPages, ExtractedCommentaries, BigComment, FilePath);
        }

        private bool IsAllChannelsChosen()
        {
            if (!PowerExtraData.All(data => data.IsAllChosen) && PowerExtraData.Count > 0)
                return false;
            if (!EnergyExtraData.All(data => data.IsAllChosen) && EnergyExtraData.Count > 0)
                return false;
            if (!Commentaries.All(data => !string.IsNullOrEmpty(data.Label)) && Commentaries.Count > 0)
                return false;
            return true;
        }
        #endregion

        #region Save Settings

        public ICommand SaveSettingsCommand { get; }
        public void SaveSettingsCommandHandle()
        {
            ExportSettings ExportSettings = new ExportSettings(ChannelsData, PowerExtraData, EnergyExtraData, Commentaries);
            try
            {
                if (!ExportSettingsManager.SaveSettings(ExportSettings))
                    return;

                SuccessMessageBox.Show("Настройки успешно сохранены");
            }
            catch(Exception ex)
            {
                ErrorMessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region Upload Settings

        public ICommand UploadSettingsCommand { get; }

        public void UploadSettingsHandle()
        {
            try
            {
                var NewExportSettings = ExportSettingsManager.UploadExportSettings();
                if (NewExportSettings == null)
                    return;

                UploadSettings(NewExportSettings);
                SuccessMessageBox.Show("Настройки успешно применены");
            }
            catch (Exception ex)
            {
                ErrorMessageBox.Show(ex.Message);
            }
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

        #region Add New Commentary

        public ICommand AddNewCommentaryCommand { get; }

        public void AddNewCommentaryCommandHandler()
        {
            var NewCommentary = new CommentaryExportElementViewModel(DeleteCommentary);
            Commentaries.Add(NewCommentary);
        }

        #endregion

        #endregion

        public ExportViewModel(DataStorage DataStorage) 
        {
            _dataStorage = DataStorage;

            ChannelsData = new ObservableCollection<ChannelViewModel>();

            PowerExtraData = new ObservableCollection<ExtraDataViewModel>();
            EnergyExtraData = new ObservableCollection<ExtraDataViewModel>();
            Commentaries = new ObservableCollection<CommentaryExportElementViewModel>();

            UploadStandartSettings();

            ExportDataCommand = new RemoteCommand(ExportDataHandle);
            SaveSettingsCommand = new RemoteCommand(SaveSettingsCommandHandle);
            UploadSettingsCommand = new RemoteCommand(UploadSettingsHandle);
            ClearChannelsExportSettingsCommand = new RemoteCommand(ClearChannelsExportSettingsHandle);
            AddNewPowerExtraDataCommand = new RemoteCommand(AddNewPowerExtraDataCommandHander);
            AddNewEnergyExtraDataCommand = new RemoteCommand(AddNewEnergyExtraDataCommandHandler);
            AddNewCommentaryCommand = new RemoteCommand(AddNewCommentaryCommandHandler);
        }

        private void UploadStandartSettings()
        {
            try
            {
                var Settings = ExportSettingsManager.GetStandartExportSettings();
                UploadSettings(Settings);
            }
            catch
            {
                DefalutSettings();
            }
        }

        private void DefalutSettings()
        {
            ChannelsData.Clear();
            for (int i = 0; i < DataStorage.MaxChannelCount; i++) 
            {
                ChannelModel channelModel = new ChannelModel(i);
                ChannelsData.Add(new ChannelViewModel(channelModel));
            }
        }

        private void ClearChannelsExportSettings()
        {
            foreach (var channel in ChannelsData)
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

        private void DeleteCommentary(CommentaryExportElementViewModel CommentData)
        {
            Commentaries.Remove(CommentData);
        }

        private void UploadSettings(ExportSettings Settings)
        {
            if (Settings == null)
            {
                DefalutSettings();
                return;
            }

            ChannelsData.Clear();
            foreach(var Channel in Settings.ChannelsData)
            {
                ChannelsData.Add(new ChannelViewModel(new ChannelModel(Channel.Channel, Channel.IsChosen, Channel.Label)));
            }

            PowerExtraData.Clear();
            foreach (var ExtraData in Settings.PowerData)
                PowerExtraData.Add(new ExtraDataViewModel(new ChartDataPreparationPower(), new OnlyOneVoltAndSeveralTokFilter(), "Power", DeleteExtraData, ExtraData));

            EnergyExtraData.Clear();
            foreach (var ExtraData in Settings.EnergyData)
                EnergyExtraData.Add(new ExtraDataViewModel(new ChartDataPreparationEnergy(), new OnlyOneVoltAndSeveralTokFilter(), "Energy", DeleteExtraData, ExtraData));

            Commentaries.Clear();
            foreach (var Commentary in Settings.CommentaryLabels)
                Commentaries.Add(new CommentaryExportElementViewModel(Commentary, DeleteCommentary));
        }
    }
}

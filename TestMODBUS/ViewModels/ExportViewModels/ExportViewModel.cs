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
using ModBusTPU.Commands;
using ModBusTPU.Models;
using ModBusTPU.Models.Data;
using ModBusTPU.Models.MessageBoxes;
using ModBusTPU.Services.Settings;
using ModBusTPU.ViewModels.Base;
using ModBusTPU.Models.Services;
using ModBusTPU.Models.Services.Excel;
using ModBusTPU.Services.Settings.Channels;
using ModBusTPU.Models.ModbusSensor.ChartDataPrepatations;
using ModBusTPU.Models.ModbusSensor.ModBusInputs.ChannelsFilters;
using ModBusTPU.Services.Excel;
using ModBusTPU.Services.Settings.Export;
using System.Security.RightsManagement;

namespace ModBusTPU.ViewModels.ExportViewModels
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

        public bool IsSaved 
        { 
            get => _isSaved;
            set
            {
                if(_isSaved == value) return;

                _isSaved = value;
                OnPropertyChanged();
            } 
        }
        #endregion

        private ExportSettings _exportSettings;

        private DataStorage _dataStorage;
        private string _bigComment;

        private bool _isSaved = true;
        private bool _isUploading = false;

        //Если комментарии были изменены и не сохранены, то при закрытии окна мы заменяем комменатрии на исходные
        private List<CommentaryExportElementViewModel> tempCommentary = new List<CommentaryExportElementViewModel>();


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
            try
            {
                ExportSettings ExportSettings = new ExportSettings(ChannelsData, PowerExtraData, EnergyExtraData, Commentaries);
                if (!ExportSettingsManager.SaveSettings(ExportSettings))
                    return;

                _exportSettings = ExportSettings;

                tempCommentary = Commentaries.ToList();

                IsSaved = true;
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
                                                      "Power", DeleteExtraData, FieldChanged);
            PowerExtraData.Add(newExtraData);
        }

        #endregion

        #region Add New Energy ExtraData

        public ICommand AddNewEnergyExtraDataCommand { get; }

        public void AddNewEnergyExtraDataCommandHandler()
        {
            var newExtraData = new ExtraDataViewModel(new ChartDataPreparationEnergy(), new OnlyOneVoltAndSeveralTokFilter(),
                                                      "Energy", DeleteExtraData, FieldChanged);
            EnergyExtraData.Add(newExtraData);
        }

        #endregion

        #region Add New Commentary

        public ICommand AddNewCommentaryCommand { get; }

        public void AddNewCommentaryCommandHandler()
        {
            var NewCommentary = new CommentaryExportElementViewModel(DeleteCommentary, FieldChanged);
            Commentaries.Add(NewCommentary);
        }

        #endregion

        #region Before Closing

        public void BeforeClosing()
        {
            if(tempCommentary == null || tempCommentary.Count == 0)
                tempCommentary = Commentaries.ToList();

            if(!IsSaved)
                UploadSettings(_exportSettings);

            UploadCommentaries();
            _isUploading = true;
        }

        private void UploadCommentaries()
        {
            _isUploading = true;
            Commentaries.Clear();

            foreach(var commentary in tempCommentary)
                Commentaries.Add(commentary);

            tempCommentary.Clear();
            _isUploading = false;
        }

        #endregion

        #region When Open

        public void WhenOpen() => _isUploading = false;

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

            PowerExtraData.CollectionChanged += (s, e) => { if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add) FieldChanged(null); };
            EnergyExtraData.CollectionChanged += (s, e) => { if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add) FieldChanged(null); };
            Commentaries.CollectionChanged += (s, e) => { if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add) FieldChanged(Commentaries[0]); };
        }

        private void UploadStandartSettings()
        {
            _isUploading = true;
            try
            {
                _exportSettings = ExportSettingsManager.GetStandartExportSettings();
            }
            catch
            {
                _exportSettings = ExportSettingsManager.GetStandartSettings();
            }

            UploadSettings(_exportSettings);

            _isUploading = false;
            IsSaved = true;
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
                throw new ArgumentNullException();

            ChannelsData.Clear();
            foreach(var Channel in Settings.ChannelsData)
            {
                ChannelsData.Add(new ChannelViewModel(new ChannelModel(Channel.Channel, Channel.IsChosen, Channel.Label)));
            }

            PowerExtraData.Clear();
            foreach (var ExtraData in Settings.PowerData)
                PowerExtraData.Add(new ExtraDataViewModel(new ChartDataPreparationPower(), new OnlyOneVoltAndSeveralTokFilter(), "Power", DeleteExtraData, ExtraData, FieldChanged));


            EnergyExtraData.Clear();
            foreach (var ExtraData in Settings.EnergyData)
                EnergyExtraData.Add(new ExtraDataViewModel(new ChartDataPreparationEnergy(), new OnlyOneVoltAndSeveralTokFilter(), "Energy", DeleteExtraData, ExtraData, FieldChanged));
           

            Commentaries.Clear();
            foreach (var Commentary in Settings.Commentaries)
                Commentaries.Add(new CommentaryExportElementViewModel(Commentary, DeleteCommentary, FieldChanged));
        }

        public void SetNewDataStorage(DataStorage Data)
        {
            if (Data != null)
                this._dataStorage = Data;
        }

        public void FieldChanged(object Field)
        {
            if (_isUploading)
                return;

            IsSaved = false;
            if (Field == null)
                return;
            if (Field.GetType() == typeof(CommentaryExportElementViewModel))
                tempCommentary = Commentaries.ToList();

        }
    }
}

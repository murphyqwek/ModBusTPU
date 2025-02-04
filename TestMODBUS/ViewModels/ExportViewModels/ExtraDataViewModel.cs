using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ModBusTPU.Commands;
using ModBusTPU.Models.Data;
using ModBusTPU.Models.ModbusSensor.ChartDataPrepatations;
using ModBusTPU.Models.ModbusSensor.ModBusInputs.ChannelsFilters;
using ModBusTPU.Services.Settings.Export;
using ModBusTPU.ViewModels.Base;

namespace ModBusTPU.ViewModels.ExportViewModels
{
    public enum ExtraDataFieldStatus
    {
        NotAllChannelsChosen,
        EmptyName,
        Filled
    }

    public class ExtraDataViewModel : BaseViewModel
    {
        #region Public Attributes

        public string Label 
        { 
            get => _name; 
            set
            {
                if (_name == value)
                    return;

                _name = value;
                UpdateStaus();

                this.FieldChangedEvent?.Invoke(this, nameof(Label));
                OnPropertyChanged();
            }
        }
        public ObservableCollection<ChannelViewModel> Channels { get; }
        public ChartDataPreparationBase ChartDataPreparation { get => _chartDataPreparation; }
        public IFilter Filter { get => _filter; }
        public bool IsAllChosen => _status == ExtraDataFieldStatus.Filled; 
        public ExtraDataFieldStatus Status
        {
            get => _status;
            set
            {
                if(value == _status)
                    return;
                _status = value;
                OnPropertyChanged();
            }
        }
        public string Type { get; }

        #endregion

        #region Private Fields

        private string _name;
        private ChartDataPreparationBase _chartDataPreparation;
        private IFilter _filter;
        private ExtraDataFieldStatus _status;
        private Action<ExtraDataViewModel> _deleteFunction;

        private Action<object, string> FieldChangedEvent;

        #endregion

        #region Commands

        #region Change Channel List
        public ICommand ChangeChannelListCommand { get; }

        private void ChangeChannelListHandler(object Channel)
        {
            if (!Int32.TryParse(Channel.ToString(), out int channel))
                throw new Exception("Channel must be Interger");

            if (Channels[channel].IsChosen)
                Channels[channel].IsChosen = false;
            else
                AddNewChannel(channel);

            UpdateStaus();
        }

        private void AddNewChannel(int NewChannel)
        {
            var Channels = GetUsingChannels();
            _filter.AddChannel(Channels, NewChannel);
            SetChannels(Channels);
        }

        #endregion

        #region Delete

        public ICommand DeleteCommand { get; }

        private void DeleteCommandHandler()
        {
            FieldChangedEvent?.Invoke(this, "Deleted");
            _deleteFunction?.Invoke(this);
        }

        #endregion

        #endregion

        public ExtraDataViewModel(ChartDataPreparationBase DataPreparation, IFilter Filter, string Type, Action<ExtraDataViewModel> DeleteFunction, ExtraData Data, Action<object, string> FieldChangedEvent = null)
        {
            _chartDataPreparation = DataPreparation;
            _filter = Filter;
            this.Type = Type;
            _deleteFunction = DeleteFunction;

            Channels = new ObservableCollection<ChannelViewModel>();
            for (int i = 0; i < DataStorage.MaxChannelCount; i++)
                Channels.Add(new ChannelViewModel(new ChannelModel(i, false, $"CH_{i}"), FieldChangedEvent));

            foreach (var Channel in Data.UsingChannels)
                AddNewChannel(Channel);

            ChangeChannelListCommand = new RemoteCommandWithParameter(ChangeChannelListHandler);
            DeleteCommand = new RemoteCommand(DeleteCommandHandler);

            Label = Data.Label;
            this.FieldChangedEvent = FieldChangedEvent;
        }

        public ExtraDataViewModel(ChartDataPreparationBase DataPreparation, IFilter Filter, string Type, Action<ExtraDataViewModel> DeleteFunction, Action<object, string> FieldChangedEvent = null)
        {
            _chartDataPreparation = DataPreparation;
            _filter = Filter;
            this.Type = Type;
            _deleteFunction = DeleteFunction;

            Channels = new ObservableCollection<ChannelViewModel>();
            for (int i = 0; i < DataStorage.MaxChannelCount; i++)
                Channels.Add(new ChannelViewModel(new ChannelModel(i, false, $"CH_{i}"), FieldChangedEvent));

            ChangeChannelListCommand = new RemoteCommandWithParameter(ChangeChannelListHandler);
            DeleteCommand = new RemoteCommand(DeleteCommandHandler);

            this.FieldChangedEvent = FieldChangedEvent;
        }

        public IList<int> GetUsingChannels()
        {
            var UsingChannels = new List<int>();
            for (int i = 0; i < Channels.Count; i++)
            {
                if (Channels[i].IsChosen)
                    UsingChannels.Add(i);
            }
            return UsingChannels;
        }

        private void SetChannels(IList<int> Channels)
        {
            for (int i = 0; i < this.Channels.Count; i++)
                this.Channels[i].IsChosen = false;

            foreach (int Channel in Channels)
                this.Channels[Channel].IsChosen = true;
        }

        private void UpdateStaus()
        {
            if (!Filter.IsAllChannelsChosen(GetUsingChannels()))
                Status = ExtraDataFieldStatus.NotAllChannelsChosen;
            else if (string.IsNullOrEmpty(_name))
                Status = ExtraDataFieldStatus.EmptyName;
            else
                Status = ExtraDataFieldStatus.Filled;
        }
    }
}

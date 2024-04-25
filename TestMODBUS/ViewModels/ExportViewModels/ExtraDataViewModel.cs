using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TestMODBUS.Commands;
using TestMODBUS.Models.Data;
using TestMODBUS.Models.ModbusSensor.ChartDataPrepatations;
using TestMODBUS.Models.ModbusSensor.ModBusInputs.ChannelsFilters;
using TestMODBUS.ViewModels.Base;

namespace TestMODBUS.ViewModels.ExportViewModels
{
    public class ExtraDataViewModel : BaseViewModel
    {
        #region Public Attributes

        public string Name 
        { 
            get => _name; 
            set
            {
                if (_name == value)
                    return;

                _name = value;
                IsAllChosen = Filter.IsAllChannelsChosen(GetUsingChannels()) && !string.IsNullOrEmpty(_name);
                OnPropertyChanged();
            }
        }
        public ObservableCollection<bool> Channels { get; }
        public ChartDataPreparationBase ChartDataPreparation { get => _chartDataPreparation; }
        public IFilter Filter { get => _filter; }
        public bool IsAllChosen 
        { 
            get => _isAllChosen; 
            private set
            {
                if (_isAllChosen == value)
                    return;

                _isAllChosen = value;
                OnPropertyChanged();
            }
        }

        public string Type { get; }

        #endregion

        #region Private Fields

        private string _name;
        private ChartDataPreparationBase _chartDataPreparation;
        private IFilter _filter;
        private bool _isAllChosen;
        private Action<ExtraDataViewModel> _deleteFunction;

        #endregion

        #region Commands

        #region Change Channel List
        public ICommand ChangeChannelListCommand { get; }

        private void ChangeChannelListHandler(object Channel)
        {
            if (!Int32.TryParse(Channel.ToString(), out int channel))
                throw new Exception("Channel must be Interger");

            if (Channels[channel])
                Channels[channel] = false;
            else
                AddNewChannel(channel);

            IsAllChosen = Filter.IsAllChannelsChosen(GetUsingChannels()) && !string.IsNullOrEmpty(_name);
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

        private void DeleteCommandHandler() => _deleteFunction(this);

        #endregion

        #endregion

        public ExtraDataViewModel(ChartDataPreparationBase DataPreparation, IFilter Filter, string Type, Action<ExtraDataViewModel> DeleteFunction)
        {
            _chartDataPreparation = DataPreparation;
            _filter = Filter;
            this.Type = Type;
            _deleteFunction = DeleteFunction;

            Channels = new ObservableCollection<bool>();
            for(int i = 0; i < DataStorage.MaxChannelCount; i++)
            {
                Channels.Add(false);
            }

            ChangeChannelListCommand = new RemoteCommandWithParameter(ChangeChannelListHandler);
            DeleteCommand = new RemoteCommand(DeleteCommandHandler);
        }

        public IList<int> GetUsingChannels()
        {
            var UsingChannels = new List<int>();
            for (int i = 0; i < Channels.Count; i++)
            {
                if (Channels[i])
                    UsingChannels.Add(i);
            }
            return UsingChannels;
        }

        private void SetChannels(IList<int> Channels)
        {
            for (int i = 0; i < this.Channels.Count; i++)
                this.Channels[i] = false;

            foreach (int Channel in Channels)
                this.Channels[Channel] = true;
        }
    }
}

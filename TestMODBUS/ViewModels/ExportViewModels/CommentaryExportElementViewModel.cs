using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    public class CommentaryExportElementViewModel : BaseViewModel
    {
        #region Public Attributes

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
        public string Commentary
        {
            get => _comment;

            set
            {
                if (_comment == value)
                    return;

                _comment = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Private Fields

        private string _label;
        private string _comment;
        private Action<CommentaryExportElementViewModel> _deleteFunction;

        #endregion

        #region Commands

        #region Delete

        public ICommand DeleteCommand { get; }

        private void DeleteCommandHandler() => _deleteFunction(this);

        #endregion

        #endregion

        public CommentaryExportElementViewModel(Action<CommentaryExportElementViewModel> DeleteFunction)
        {
            this.Label = "";
            _deleteFunction = DeleteFunction;
            DeleteCommand = new RemoteCommand(DeleteCommandHandler);
        }

        public CommentaryExportElementViewModel(string Label, Action<CommentaryExportElementViewModel> DeleteFunction)
        {
            _deleteFunction = DeleteFunction;
            this.Label = Label;
            DeleteCommand = new RemoteCommand(DeleteCommandHandler);
        }

    }
}
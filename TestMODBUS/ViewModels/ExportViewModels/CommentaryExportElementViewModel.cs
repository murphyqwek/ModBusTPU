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

                //Мы делаем копию комменатриев, потом меняем поле
                FieldChanged?.Invoke(this, nameof(Label));
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

                //Мы делаем копию комменатриев, потом меняем поле
                FieldChanged?.Invoke(this, nameof(Commentary));
                _comment = value;
                OnPropertyChanged();
            }
        }
        public bool IsShownOnMainWindow
        {
            get => _isShownOnMainWindow;

            set
            {
                if (_isShownOnMainWindow == value)
                    return;

                //Мы делаем копию комменатриев, потом меняем поле
                FieldChanged?.Invoke(this, nameof(IsShownOnMainWindow));

                _isShownOnMainWindow = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Private Fields

        private string _label;
        private string _comment;
        private Action<CommentaryExportElementViewModel> _deleteFunction;
        private bool _isShownOnMainWindow;

        private Action<object, string> FieldChanged;

        #endregion

        #region Commands

        #region Delete

        public ICommand DeleteCommand { get; }

        private void DeleteCommandHandler()
        {
            FieldChanged?.Invoke(this, "Deleted");
            _deleteFunction(this);
        }

        #endregion

        #endregion

        public CommentaryExportElementViewModel(Action<CommentaryExportElementViewModel> DeleteFunction, Action<object, string> fieldChanged = null)
        {
            Label = "";
            IsShownOnMainWindow = false;
            _deleteFunction = DeleteFunction;
            DeleteCommand = new RemoteCommand(DeleteCommandHandler);
            FieldChanged = fieldChanged;
        }

        public CommentaryExportElementViewModel(string Label, bool IsShownOnMainWindow, Action<CommentaryExportElementViewModel> DeleteFunction, Action<object, string> fieldChanged = null)
        {
            this.Label = Label;
            this.IsShownOnMainWindow = IsShownOnMainWindow;
            _deleteFunction = DeleteFunction;
            DeleteCommand = new RemoteCommand(DeleteCommandHandler);

            FieldChanged = fieldChanged;
        }

        public CommentaryExportElementViewModel(Commentary Commentary, Action<CommentaryExportElementViewModel> DeleteFunction, Action<object, string> fieldChanged = null)
        {
            this.Label = Commentary.Label;
            this.IsShownOnMainWindow = Commentary.IsShownOnMainWindow;
            _deleteFunction = DeleteFunction;
            DeleteCommand = new RemoteCommand(DeleteCommandHandler);
            FieldChanged = fieldChanged;
        }

    }
}
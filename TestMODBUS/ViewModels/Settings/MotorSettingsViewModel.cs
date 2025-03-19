using ModBusTPU.Services.Settings.MotorSettings;
using ModBusTPU.ViewModels.Base;
using System;
using ModBusTPU.ViewModels.Field;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml.ConditionalFormatting.Contracts;
using ModBusTPU.Models.Services;
using ModBusTPU.Models.MessageBoxes;
using System.Windows.Input;
using System.Windows;
using ModBusTPU.Commands;
using System.Reflection;
using System.Globalization;

namespace ModBusTPU.ViewModels.Settings
{
    public class MotorSettingsViewModel : BaseViewModel
    {

        #region Public Fields
        public ObservableCollection<Field.Field> Fields { get; } = new ObservableCollection<Field.Field>();

        public string PORT { get; set; }

        public ObservableCollection<string> Ports => ListAvailablePorts.AvailablePorts;
        #endregion

        #region Private fields
        private MotorSettingsContainer motorSettings;
        #endregion

        #region Commands
        #region Upload Settings

        public ICommand UploadCommand { get; }

        private void UploadCommandCommandHandler()
        {
            var path = FileHelper.GetOpenFilePath($"*.json|*.json;", ".json");

            if (path == null)
            {
                return;
            }

            if (RequestYesNoMessageBox.Show("Вы уверены, что загрузить новые настройки?") != MessageBoxResult.Yes)
                return;

            var input = MotorSettingsUploader.Upload(path);

            if (input.exceptionText != string.Empty)
            {
                ErrorMessageBox.Show(input.exceptionText);
                return;
            }

            CopyFields(motorSettings, input.motorSettings);

            OnPropertyChanged(nameof(Fields));

            SuccessMessageBox.Show("Файл загружен");
        }

        #endregion

        #region Save Settings

        public ICommand SaveCommand { get; }

        private void SaveCommandCommandHandler()
        {
            var path = FileHelper.GetSaveFilePath($"*.json|*.json;", ".json");

            if (path == null)
            {
                return;
            }

            try
            {
                MotorSettingsUploader.Save(motorSettings, path);
            }
            catch (Exception ex)
            {
                ErrorMessageBox.Show("Что-то пошло не так: " + ex.Message);
            }

            OnPropertyChanged(nameof(Fields));

            SuccessMessageBox.Show("Файл сохранен");
        }

        #endregion
        #endregion

        public MotorSettingsViewModel(MotorSettingsContainer motorSettings)
        {
            this.motorSettings = motorSettings;
            this.PORT = motorSettings.PORT;

            Fields = new ObservableCollection<Field.Field>()
            {
                new Field.Field("BaudRate", () => motorSettings.BAUDRATE, value => motorSettings.BAUDRATE = Convert.ToInt32(value)),
                new Field.Field("Адрес", () => motorSettings.ADDRESS, value => motorSettings.ADDRESS = Convert.ToByte(value)),
                new Field.Field("Порог тока",() => motorSettings.currentThreshold, value => motorSettings.currentThreshold = Convert.ToDouble(value.ToString().Replace(',', '.').ToString().Replace(',', '.'), CultureInfo.InvariantCulture)),
                new Field.Field("Порог напр.",() => motorSettings.voltageThreshold, value => motorSettings.voltageThreshold = Convert.ToDouble(value.ToString().Replace(',', '.'), CultureInfo.InvariantCulture)),
                new Field.Field("Верхнее порог. знач. тока",() => motorSettings.highCurrentThresholdBound, value => motorSettings.highCurrentThresholdBound =Convert.ToDouble(value.ToString().Replace(',', '.'), CultureInfo.InvariantCulture)),
                new Field.Field("Нижнее порог. знач. тока",() => motorSettings.lowCurrentThresholdBound, value => motorSettings.lowCurrentThresholdBound =Convert.ToDouble(value.ToString().Replace(',', '.'), CultureInfo.InvariantCulture)),
                new Field.Field("Верхнее порог. знач. напр.",() => motorSettings.highVoltageThresholdBound, value => motorSettings.highVoltageThresholdBound =Convert.ToDouble(value.ToString().Replace(',', '.'), CultureInfo.InvariantCulture)),
                new Field.Field("Нижнее порог. знач. напр.",() => motorSettings.lowVoltageThresholdBound, value => motorSettings.lowVoltageThresholdBound =Convert.ToDouble(value.ToString().Replace(',', '.'), CultureInfo.InvariantCulture)),
                new Field.Field("КЗ задержка", () => motorSettings.KZDELAY, value => motorSettings.KZDELAY = Convert.ToInt32(value)),
                new Field.Field("Задержка записи", () => motorSettings.WRITEDELAY, value => motorSettings.WRITEDELAY = Convert.ToInt32(value)),
                new Field.Field("Задержка итерации",() => motorSettings.ITERATIONDELAY, value => motorSettings.ITERATIONDELAY =Convert.ToInt32(value)),
                new Field.Field("Задержка обратного хода",() => motorSettings.REVERSDELAY, value => motorSettings.REVERSDELAY =Convert.ToInt32(value)),
                new Field.Field("Скорость",() => motorSettings.SPEED, value => motorSettings.SPEED =Convert.ToUInt16(value)),
            };

            UploadCommand = new RemoteCommand(UploadCommandCommandHandler);
            SaveCommand = new RemoteCommand(SaveCommandCommandHandler);
        }


        public static void CopyFields(MotorSettingsContainer target, MotorSettingsContainer source)
        {
            // Получаем все поля объекта
            var fields = typeof(MotorSettingsContainer).GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (var field in fields)
            {
                // Копируем значения
                field.SetValue(target, field.GetValue(source));
            }
        }

    }
}

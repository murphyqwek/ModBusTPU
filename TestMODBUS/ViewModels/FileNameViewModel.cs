using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModBusTPU.ViewModels.Base;

namespace ModBusTPU.ViewModels
{
    public class FileNameViewModel : BaseViewModel
    {
        private string _serie;
        private string _experiment;
        private string _tok1;
        private string _tok2;
        private string _time;
        private string _sensorBefore;
        private string _sensorAfter;

        #region Public Properties
        public string Serie { get => _serie; set { _serie = value; OnPropertyChanged(); } }
        public string Experiment { get => _experiment; set { _experiment = value; OnPropertyChanged(); } }
        public string Tok1 { get => _tok1; set { _tok1 = value; OnPropertyChanged(); } }
        public string Tok2 { get => _tok2; set { _tok2 = value; OnPropertyChanged(); } }
        public string Time { get => _time; set { _time = value; OnPropertyChanged(); } }

        public string SensorBefore { get => _sensorBefore; set { _sensorBefore = value; OnPropertyChanged(); } }
        public string SensoreAfter { get => _sensorAfter; set { _sensorAfter = value; OnPropertyChanged(); } }
        #endregion

        public string GetFileName() 
        {
            return $"{_experiment}({_tok1} А, {_tok2} А, {_time} с)";
        }

        public bool isFileNameEmpty()
        {
            return "( А,  А,  с)" == GetFileName();
        }
    }
}

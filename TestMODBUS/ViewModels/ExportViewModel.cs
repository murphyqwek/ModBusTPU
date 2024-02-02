using LiveCharts.Defaults;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using TestMODBUS.Commands;
using TestMODBUS.Models;
using TestMODBUS.Models.Data;
using TestMODBUS.Models.Excel;
using TestMODBUS.Models.MessageBoxes;
using TestMODBUS.ViewModels.Base;

namespace TestMODBUS.ViewModels
{
    public class ExportViewModel : BaseViewModel
    {
        public ObservableCollection<ChannelViewModel> ExportChannels { get; }
        public string FileName { get; }


        private List<ChannelModel> _channelsOutputData = new List<ChannelModel>();


        public ICommand ExportDataCommand { get; }

        private void ExportDataHandle()
        {
            if(!_channelsOutputData.Any(channel => channel.IsChosen))
            {
                ErrorMessageBox.Show("Нужно выбрать как минимум один канал");
                return;
            }

            string path = OpenFileHelper.GetSaveFile(FileName);
            if (path == null)
                return;

            try
            {
                ExportExcel.SaveData(_channelsOutputData, path);
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

        public ExportViewModel(Data Data, string FileName) 
        {
            this.FileName = FileName;
            ExportChannels = new ObservableCollection<ChannelViewModel>();
            UploadChannelsData(Data);
            ExportDataCommand = new RemoteCommand(ExportDataHandle);
        }

        private void UploadChannelsData(Data Data)
        {
            for(int i = 0; i < Data.ChannelsData.Count; i++) 
            {
                ChannelModel channelModel = new ChannelModel(i, Data.GetChannelData(i).ToList());
                _channelsOutputData.Add(channelModel);
                ExportChannels.Add(new ChannelViewModel(channelModel));
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModBusTPU.Models.Data;
using ModBusTPU.Models.Modbus;
using ModBusTPU.Models.ModbusSensor;
using ModBusTPU.Models.ModbusSensor.ChartDataPrepatations;
using ModBusTPU.Models.Services;
using ModBusTPU.ViewModels;
using ModBusTPU.ViewModels.ExportViewModels;

namespace ModBusTPU.Services.Excel
{
    public struct ExcelChart
    {
        public string Title;
        public List<Point> Points;
        public string XTitle, YTitle;
        public string SerieTitle;
    }

    public struct ExcelPage
    {
        public string Title;
        public IList<ExcelChart> Charts;
    }

    public struct ChannelsDataExcelPage
    {
        public string Title;
        public List<int> ChannelsToChart;
        public List<ExcelChart> ChannelsData;
    }

    public struct Commentary
    {
        public string Label;
        public string Comment;
    }

    public static class ExcelDataPreparation
    {
        //Подготавливает дополнительные данные для Excel
        //Ключ - название листа, на котором будут графики
        //Значение - Данные графика: заголовок, ChartDataPreparation, используемые каналы

        public static List<Commentary> ExtractCommentaries(IEnumerable<CommentaryExportElementViewModel> Commentaries)
        {
            var ExtractedComments = new List<Commentary>();

            foreach(var Comment in Commentaries)
            {
                var ExtractedComment = new Commentary();
                ExtractedComment.Label = Comment.Label;
                ExtractedComment.Comment = Comment.Commentary;

                ExtractedComments.Add(ExtractedComment);
            }

            return ExtractedComments;
        }

        public static List<ExcelPage> ExtractExtraData(Dictionary<string, IEnumerable<ExtraDataViewModel>> ExtraData, DataStorage DataStorage)
        {
            List<ExcelPage> Pages = new List<ExcelPage>();
            foreach(var PageTitle in ExtraData.Keys)
            {
                Pages.Add(ExtractPage(PageTitle, ExtraData[PageTitle], DataStorage));
            }

            return Pages;
        }

        private static ExcelPage ExtractPage(string PageTitle, IEnumerable<ExtraDataViewModel> ChartsRawData, DataStorage DataStorage)
        {
            List<ExcelChart> Charts = new List<ExcelChart>();

            foreach(var ChartRawData in ChartsRawData) 
            {
                var chart = ExtractChart(ChartRawData, DataStorage);
                if(chart.Title != null)
                    Charts.Add(chart);
            }

            ExcelPage Page = new ExcelPage();
            Page.Title = PageTitle;
            Page.Charts = Charts;

            return Page;
        }

        private static ExcelChart ExtractChart(ExtraDataViewModel ChartRawData, DataStorage DataStorage) 
        {
            if (DataStorage.GetChannelLength() == 0)
                return new ExcelChart();

            var UsingChannels = ChartRawData.GetUsingChannels();
            var Filter = ChartRawData.Filter;
            var ChartPreparation = ChartRawData.ChartDataPreparation;
            var Title = ChartRawData.Label;

            if (!Filter.IsAllChannelsChosen(UsingChannels))
                throw new Exception("Не все нужные каналы были выбраны");

            var SeriePoints = ChartPreparation.GetAllPoints(UsingChannels, DataStorage);

            if (SeriePoints.Count > 1)
                throw new Exception("Фукнция GetAllPoints вернула больше одной серии точек, хотя ожидалась одна");

            if (SeriePoints.Count == 0)
                throw new Exception("Ошибка экспорта данных: неверно выбранные каналы");

            var Points = GetPoints(SeriePoints);

            ExcelChart Chart = new ExcelChart();
            Chart.Title = Title;
            Chart.Points = Points;
            Chart.XTitle = "Время, с";
            Chart.YTitle = GetYTittle(ChartPreparation);
            Chart.SerieTitle = SeriePoints[0].SerieTitle;

            return Chart;
        }

        private static List<Point> GetPoints(IList<SerieData> Series)
        {
            var Points = new List<Point>();
            foreach(var point in Series[0].Points)
            {
                Points.Add(new Point(point.X / 1000, point.Y));
            }

            return Points;
        }

        private static string GetYTittle(ChartDataPreparationBase DataPreparation)
        {
            switch(DataPreparation)
            {
                case ChartDataPreparationPower _:
                    return "Мощность, кВт";
                case ChartDataPreparationEnergy _:
                    return "Энергия, кВт*ч";
                default:
                    throw new Exception("Необработанный ChartDataPreparation в ExcelDataPreparation");
            }
        }

        public static List<int> GetChannelsToChartList(ObservableCollection<ChannelViewModel> Channels)
        {
            List<int> ChannelsToChart = new List<int>();
            foreach(var Channel in Channels)
            {
                if (Channel.IsChosen)
                    ChannelsToChart.Add(Channel.ChannelNumber);
            }

            return ChannelsToChart;
        }

        public static ExcelPage ExtractChannelsData(ObservableCollection<ChannelViewModel> Channels, DataStorage DataStorage)
        {
            ExcelPage Page = new ExcelPage();
            List<ExcelChart> Charts = new List<ExcelChart>();

            foreach(var Channel in new int[] { 0, 1 })
            {
                ExcelChart Chart = new ExcelChart();
                var Points = new List<Point>();

                foreach(var Point in DataStorage.GetChannelData(Channel))
                {
                    var channelType = ChannelTypeList.GetChannelType(Channel);
                    double Time = Point.X / 1000;
                    double Value = Point.Y;
                    switch (channelType)
                    {
                        case ChannelType.Tok:
                            Value = ModBusValueConverter.ConvertToAmperValue(Value);
                            Chart.YTitle = "Сила тока, А";
                            Chart.SerieTitle = "Сила тока";
                            Chart.Title = "Сила тока";
                            break;
                        case ChannelType.Volt:
                            Value = ModBusValueConverter.ConvertToVoltValue(Value);
                            Chart.YTitle = "Напряжение, В";
                            Chart.SerieTitle = "Напряжение";
                            Chart.Title = "Напряжение";
                            break;
                        case ChannelType.Regular:
                            Chart.YTitle = "";
                            Chart.SerieTitle = "";
                            break;
                        default:
                            throw new Exception("Необработанный тип каналов в ExcelDataPreparation");   
                    }
                    Points.Add(new Point(Time, Value));
                }
                Chart.XTitle = "Вермя, с";
                Chart.Points = Points;

                Charts.Add(Chart);

            }

            Page.Title = "Данные с каналов";
            Page.Charts = Charts;

            return Page;
        }

        public static ExcelPage ExtractRawData(DataStorage DataStorage)
        {
            ExcelPage Page = new ExcelPage();
            List<ExcelChart> Charts = new List<ExcelChart>();

            for(int i = 0; i < DataStorage.MaxChannelCount; i++)
            {
                ExcelChart Chart = new ExcelChart();
                var Points = new List<Point>();

                foreach (var Point in DataStorage.GetChannelData(i))
                {
                    Points.Add(new Point(Point.X / 1000, Point.Y));
                }
                Chart.Title = $"CH_{i}";
                Chart.YTitle = "Сырые данные";
                Chart.XTitle = "Время, с";
                Chart.Points = Points;
                Chart.SerieTitle = $"CH_{i}";

                Charts.Add(Chart);
            }

            Page.Title = "Сырые Данные с каналов";
            Page.Charts = Charts;

            return Page;
        }
    }
}

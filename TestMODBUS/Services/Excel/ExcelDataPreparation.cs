using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMODBUS.Models.Data;
using TestMODBUS.Models.Modbus;
using TestMODBUS.Models.ModbusSensor;
using TestMODBUS.Models.ModbusSensor.ChartDataPrepatations;
using TestMODBUS.Models.Services;
using TestMODBUS.ViewModels.ExportViewModels;

namespace TestMODBUS.Services.Excel
{
    public struct ExcelChart
    {
        public string Title;
        public List<Point> Points;
        public string XTitle, YTitle;
    }

    public struct ExcelPage
    {
        public string Title;
        public IList<ExcelChart> Charts;
    }

    public static class ExcelDataPreparation
    {
        //Подготавливает дополнительные данные для Excel
        //Ключ - название листа, на котором будут графики
        //Значение - Данные графика: заголовок, ChartDataPreparation, используемые каналы
        public static List<ExcelPage> ExtractExtraDataForExcel(Dictionary<string, List<ExtraDataViewModel>> ExtraData, DataStorage DataStorage)
        {
            List<ExcelPage> Pages = new List<ExcelPage>();
            foreach(var PageTitle in ExtraData.Keys)
            {
                Pages.Add(ExtractPage(PageTitle, ExtraData[PageTitle], DataStorage));
            }

            return Pages;
        }

        private static ExcelPage ExtractPage(string PageTitle, List<ExtraDataViewModel> ChartsRawData, DataStorage DataStorage)
        {
            List<ExcelChart> Charts = new List<ExcelChart>();

            foreach(var ChartRawData in ChartsRawData) 
            { 
                Charts.Add(ExtractChart(ChartRawData, DataStorage));
            }

            ExcelPage Page = new ExcelPage();
            Page.Title = PageTitle;
            Page.Charts = Charts;

            return Page;
        }

        private static ExcelChart ExtractChart(ExtraDataViewModel ChartRawData, DataStorage DataStorage) 
        {
            var UsingChannels = ChartRawData.GetUsingChannels();
            var Filter = ChartRawData.Filter;
            var ChartPreparation = ChartRawData.ChartDataPreparation;
            var Title = ChartRawData.Name;

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
            Chart.XTitle = "Секунды";
            Chart.YTitle = GetYTittle(ChartPreparation);

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
                case ChartDataPreparationPower p:
                    return "кВт";
                case ChartDataPreparationEnergy e:
                    return "кВт*ч";
                case ChartDataPreparationStandart s:
                    return "-";
                default:
                    throw new Exception("Необработанный ChartDataPreparation в ExcelDataPreparation");
            }
        }

        private static ExcelPage ExtractChannels(List<ChannelModel> Channels)
        {
            ExcelPage Page = new ExcelPage();
            List<ExcelChart> Charts = new List<ExcelChart>();

            foreach(var channel in Channels)
            {
                ExcelChart Chart = new ExcelChart();
                var Points = new List<Point>();

                foreach(var Point in Points)
                {
                    var channelType = ChannelTypeList.GetChannelType(channel.ChannelNumber);
                    switch (channelType)
                    {
                        case ChannelType.Tok:
                            Points.Add(new Point(Point.X / 1000, ModBusValueConverter.ConvertToAmperValue(Point.Y)));
                            Chart.YTitle = "А";
                            break;
                        case ChannelType.Volt:
                            Points.Add(new Point(Point.X / 1000, ModBusValueConverter.ConvertToAmperValue(Point.Y)));
                            Chart.YTitle = "В";
                            break;
                        case ChannelType.Regular:
                            Chart.YTitle = "";
                            Points.Add(new Point(Point.X / 1000, Point.Y));
                            break;
                        default:
                            throw new Exception("Необработанный тип каналов в ExcelDataPreparation");   
                    }
                }
                Chart.Title = channel.Label;
                Chart.XTitle = "Секунды";
                Chart.Points = Points;

                Charts.Add(Chart);
            }

            Page.Title = "Данные с каналов";
            Page.Charts = Charts;

            return Page;
        }
    }
}

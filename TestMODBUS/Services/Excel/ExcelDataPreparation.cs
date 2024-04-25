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
        public string SerieTitle;
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
                Points.Add(new Point(point.X, point.Y));
            }

            return Points;
        }

        private static string GetYTittle(ChartDataPreparationBase DataPreparation)
        {
            switch(DataPreparation)
            {
                case ChartDataPreparationPower p:
                    return "Мощность, кВт";
                case ChartDataPreparationEnergy e:
                    return "Энергия, кВт*ч";
                default:
                    throw new Exception("Необработанный ChartDataPreparation в ExcelDataPreparation");
            }
        }

        public static ExcelPage ExtractChannelsData(List<ChannelModel> Channels)
        {
            ExcelPage Page = new ExcelPage();
            List<ExcelChart> Charts = new List<ExcelChart>();

            foreach(var Channel in Channels)
            {
                if (!Channel.IsChosen)
                    continue;

                ExcelChart Chart = new ExcelChart();
                var Points = new List<Point>();

                foreach(var Point in Channel.Data)
                {
                    var channelType = ChannelTypeList.GetChannelType(Channel.ChannelNumber);
                    switch (channelType)
                    {
                        case ChannelType.Tok:
                            Points.Add(new Point(Point.X, ModBusValueConverter.ConvertToAmperValue(Point.Y)));
                            Chart.YTitle = "Сила тока, А";
                            Chart.SerieTitle = "Сила тока";
                            break;
                        case ChannelType.Volt:
                            Points.Add(new Point(Point.X, ModBusValueConverter.ConvertToVoltValue(Point.Y)));
                            Chart.YTitle = "Напряжение, В";
                            Chart.SerieTitle = "Напряжение";
                            break;
                        case ChannelType.Regular:
                            Points.Add(new Point(Point.X, Point.Y));
                            Chart.YTitle = "";
                            Chart.SerieTitle = "";
                            break;
                        default:
                            throw new Exception("Необработанный тип каналов в ExcelDataPreparation");   
                    }
                }
                Chart.Title = Channel.Label;
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

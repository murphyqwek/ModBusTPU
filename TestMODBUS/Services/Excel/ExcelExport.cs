using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Drawing.Chart.Style;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using TestMODBUS.Exceptions;
using TestMODBUS.Models.Data;
using TestMODBUS.Models.Services;
using TestMODBUS.Services.Excel;

namespace TestMODBUS.Models.Services.Excel
{
    public static class ExcelExport
    {
        //Размеры чартов в Excel
        private const int ChartHeight = 450;
        private const int ChartWidth = 900;
       

        //Библиотека EPPlus платная для коммерческого использования и бесплатная для некомерческого использования
        //Перед тем, как её использовать в программе нужно указать тип LicenseContext
        public static void SetUp()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public static void SaveData(ExcelPage RawDataPage, ExcelPage ChannelsPage, IList<ExcelPage> ExtraDataPages, string FilePath)
        {
            if (OpenFileHelper.isFileOpen(FilePath))
                throw new FileIsAlreadyOpenException(FilePath);

            using(ExcelPackage ExcelPackage = new ExcelPackage())
            {
                //Указываем метаданные Excel файла
                ExcelPackage.Workbook.Properties.Created = DateTime.Now;

                //Заполняем комментарии
                //TODO: Сделать

                //Заполняем дополнительные данные
                foreach (var Page in ExtraDataPages)
                {
                    if (Page.Charts.Count == 0)
                        continue;
                    var ExtraDataSheet = FillDataSheet(ExcelPackage, Page, Page.Title + " - Данные");
                    CreateCharts(ExcelPackage, ExtraDataSheet, Page, Page.Title + " - Графики");
                }

                //Заполняем обработанные данные
                if (ChannelsPage.Charts.Count != 0)
                {
                    var ChannelsDataSheet = FillDataSheet(ExcelPackage, ChannelsPage, ChannelsPage.Title + " - Данные");
                    CreateCharts(ExcelPackage, ChannelsDataSheet, ChannelsPage, ChannelsPage.Title + " - Графики");
                }
                //Заполняем сырые данные
                FillDataSheet(ExcelPackage, RawDataPage, RawDataPage.Title);


                //Сохраняем файл
                FileInfo fi = new FileInfo(FilePath);
                ExcelPackage.SaveAs(fi);
            }
        }

        private static ExcelWorksheet FillDataSheet(ExcelPackage ExcelPackage, ExcelPage Page, string Title)
        {
            ExcelWorksheet dataSheet = ExcelPackage.Workbook.Worksheets.Add(Title);
            for(int i = 0; i < Page.Charts.Count; i++)
            {
                var Data = Page.Charts[i];
                AddChannelDataColumn(dataSheet, Data.Points, i, Data.Title, Data.XTitle, Data.YTitle);
            }

            return dataSheet;
        }

        private static void FillCommentaries(ExcelPackage ExcelPackage, Dictionary<string, string> CommentariesData)
        {
            ExcelWorksheet Commentaries = ExcelPackage.Workbook.Worksheets.Add("Комментарии");

            Commentaries.Columns[0].AutoFit();
            Commentaries.Columns[1].AutoFit();

            int index = 1;
            foreach(var CommentName in CommentariesData.Keys)
            {
                Commentaries.Cells[index, 1].Value = CommentName;//GetChannelTitle(ChannelIndex);
                Commentaries.Cells[index, 2].Value = CommentariesData[CommentName];
                Commentaries.Cells[index, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                Commentaries.Cells[index, 2].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            }
        }

        //Создаёт графики данных с канала
        private static void CreateCharts(ExcelPackage ExcelPackage, ExcelWorksheet DataSheet, ExcelPage Page, string PageTitle)
        {
            ExcelWorksheet ChartSheet = ExcelPackage.Workbook.Worksheets.Add(PageTitle);
            var Charts = Page.Charts;

            for(int i = 0; i < Charts.Count; i++)
            {
                CreateChart(ChartSheet, DataSheet, i, Charts[i]);
            }
        }

        private static void CreateChart(ExcelWorksheet ChartSheet, ExcelWorksheet DataSheet, int ChartIndex, TestMODBUS.Services.Excel.ExcelChart ChartData)
        {
            int DataLength = ChartData.Points.Count;

            int DataColumn = ChartIndex * 3 + 1;
            int DataLastRow = DataLength + 3 - 1;
            int ChartColumn = ChartIndex * 3 + 1;

            var graphic = ChartSheet.Drawings.AddLineChart(ChartData.Title, eLineChartType.Line);
            graphic.SetSize(ChartWidth, ChartHeight);
            graphic.SetPosition(ChartColumn / 3 * ChartHeight, 0);
            graphic.StyleManager.SetChartStyle(ePresetChartStyle.LineChartStyle1, ePresetChartColors.ColorfulPalette1); //ePresetChartStyle.LineChartStyle1 - стиль графика
            graphic.XAxis.DisplayUnit = 1000;

            ExcelRange timeRange = DataSheet.Cells[3, DataColumn, DataLastRow, DataColumn];
            ExcelRange dataRange = DataSheet.Cells[3, DataColumn + 1, DataLastRow, DataColumn + 1];

            graphic.Series.Add(dataRange, timeRange);

            graphic.XAxis.AddGridlines();
            graphic.XAxis.Title.Text = ChartData.XTitle;

            graphic.YAxis.Title.Text = ChartData.YTitle;
            graphic.YAxis.Title.TextBody.VerticalText = OfficeOpenXml.Drawing.eTextVerticalType.Vertical270;

            graphic.Title.Text = ChartData.Title;
            graphic.Series[0].Header = ChartData.Title;
            graphic.Legend.Position = eLegendPosition.TopRight;

            graphic.YAxis.Crosses = 0;
        }


        //Создаёт колонку с данными с канала
        private static void AddChannelDataColumn(ExcelWorksheet dataSheet, IList<Point> points, int ChannelIndex, string ColumnTitle, string FirstColumn, string SecondColumn)
        {
            int column = ChannelIndex * 3 + 1;

            dataSheet.Cells[2, column].Value = FirstColumn;
            dataSheet.Cells[2, column + 1].Value = SecondColumn;

            for(int i = 0; i < points.Count; i++)
            {
                dataSheet.Cells[i + 3, column].Value = points[i].X / 1000;
                dataSheet.Cells[i + 3, column + 1].Value = points[i].Y;
            }

            dataSheet.Columns[column].AutoFit();
            dataSheet.Columns[column + 1].AutoFit();

            dataSheet.Cells[1, column, 1, column + 1].Merge = true;
            dataSheet.Cells[1, column].Value = ColumnTitle;
            dataSheet.Cells[1, column].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            dataSheet.Cells[1, column].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        }
    }
}

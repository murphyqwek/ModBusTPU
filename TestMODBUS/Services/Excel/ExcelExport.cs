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
using ModBusTPU.Exceptions;
using ModBusTPU.Models.Data;
using ModBusTPU.Models.Services;
using ModBusTPU.Services.Excel;
using OfficeOpenXml.Style;
using System.Xml.Linq;
using System.Windows.Controls;

namespace ModBusTPU.Models.Services.Excel
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

        public static void SaveData(ExcelPage RawDataPage, ExcelPage ChannelsPage, IList<int> ChannelsToChart, IList<ExcelPage> ExtraDataPages, IList<Commentary> Commentaries, string BigComment, string FilePath)
        {
            if (FileHelper.isFileOpen(FilePath))
                throw new FileIsAlreadyOpenException(FilePath);

            using(ExcelPackage ExcelPackage = new ExcelPackage())
            {
                //Указываем метаданные Excel файла
                ExcelPackage.Workbook.Properties.Created = DateTime.Now;

                //Заполняем комментарии
                FillCommentaries(ExcelPackage, Commentaries, BigComment);

                //Заполняем дополнительные данные
                foreach (var Page in ExtraDataPages)
                {
                    if (Page.Charts.Count == 0)
                        continue;
                    var ExtraDataSheet = FillDataSheet(ExcelPackage, Page, Page.Title + " - Данные");
                    CreateCharts(ExcelPackage, ExtraDataSheet, Page, Page.Title + " - Графики");
                }

                //Заполняем обработанные данные
                var ChannelsDataSheet = FillDataSheet(ExcelPackage, ChannelsPage, ChannelsPage.Title + " - Данные");
                if (ChannelsToChart.Count != 0)
                {
                    ExcelWorksheet ChannelsDataChartSheet = ExcelPackage.Workbook.Worksheets.Add(ChannelsPage.Title + " - Графики");
                    int ChartIndex = 0;
                    foreach(var Channel in ChannelsToChart)
                    {
                        CreateChart(ChannelsDataChartSheet, ChannelsDataSheet, Channel, ChartIndex, ChannelsPage.Charts[Channel]);
                        ChartIndex++;
                    }
                }

                //Заполняем сырые данные (не нужны)
                //FillDataSheet(ExcelPackage, RawDataPage, RawDataPage.Title);


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

        private static void FillCommentaries(ExcelPackage ExcelPackage, IList<Commentary> CommentariesData, string BigComment)
        {
            ExcelWorksheet Commentaries = ExcelPackage.Workbook.Worksheets.Add("Комментарии");

            var Title = Commentaries.Cells["A1:B1"];
            Title.Merge = true;
            Title.Value = "Внешние данные";
            Title.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            Title.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            Commentaries.Columns[1].AutoFit();
            Commentaries.Columns[2].AutoFit();

            int index = 2;
            foreach(var Comment in CommentariesData)
            {
                Commentaries.Cells[index, 1].Value = Comment.Label;
                Commentaries.Cells[index, 2].Value = Comment.Comment;
                Commentaries.Cells[index, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                Commentaries.Cells[index, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                index++;
            }

            var comment_title = Commentaries.Cells["D1:J2"];
            var comment_section = Commentaries.Cells["D3:J20"];

            comment_title.Merge = true;
            comment_title.Value = "Комментарии к эксперименту";
            comment_title.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            comment_title.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            comment_section.Merge = true;
            comment_section.Value = BigComment;
            comment_section.Style.WrapText = true;
            comment_section.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
        }

        //Создаёт графики данных с канала
        private static void CreateCharts(ExcelPackage ExcelPackage, ExcelWorksheet DataSheet, ExcelPage Page, string PageTitle)
        {
            ExcelWorksheet ChartSheet = ExcelPackage.Workbook.Worksheets.Add(PageTitle);
            var Charts = Page.Charts;

            for(int i = 0; i < Charts.Count; i++)
            {
                CreateChart(ChartSheet, DataSheet, i, i, Charts[i]);
            }
        }

        private static void CreateChart(ExcelWorksheet ChartSheet, ExcelWorksheet DataSheet, int DataIndex, int ChartIndex, ModBusTPU.Services.Excel.ExcelChart ChartData)
        {
            int DataLength = ChartData.Points.Count;

            int DataColumn = DataIndex * 3 + 1;
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
                dataSheet.Cells[i + 3, column].Value = points[i].X;
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

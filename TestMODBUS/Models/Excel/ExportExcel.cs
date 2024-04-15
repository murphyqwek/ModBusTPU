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

namespace TestMODBUS.Models.Excel
{
    public static class ExportExcel
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

        public static void SaveData(List<ChannelModel> Channels, string FilePath, Dictionary<string, string> CommentariesData = null)
        {
            if (OpenFileHelper.isFileOpen(FilePath))
                throw new FileIsAlreadyOpenException(FilePath);

            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                //Указываем метаданные Excel файла
                excelPackage.Workbook.Properties.Created = DateTime.Now;

                //Создаём лист с графиками и лист с данными с каналов
                ExcelWorksheet graphicSheet = excelPackage.Workbook.Worksheets.Add("Графики");
                ExcelWorksheet dataSheet = excelPackage.Workbook.Worksheets.Add("Данные с каналов");

                if(CommentariesData != null)
                {
                    FillCommentaries(excelPackage, CommentariesData);
                }

                int ChartIndex = 0;
                foreach(var ChannelModel in Channels)
                {
                    if (!ChannelModel.IsChosen)
                        continue;

                    AddChannelDataColumn(dataSheet, ChannelModel.Data, ChannelModel.ChannelNumber, ChartIndex, ChannelModel.Label);
                    CreateChart(graphicSheet, dataSheet, ChannelModel.ChannelNumber, ChannelModel.Data.Count - 1, ChannelModel.Label, ChartIndex);
                    ChartIndex++;
                }

                //Сохраняем файл
                FileInfo fi = new FileInfo(FilePath);
                excelPackage.SaveAs(fi);
            }
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

        public static void SaveData(Data.DataStorage DataStorage, string FilePath)
        {
            if (OpenFileHelper.isFileOpen(FilePath))
                throw new FileIsAlreadyOpenException(FilePath);
            if (DataStorage == null)
                throw new ArgumentNullException(nameof(DataStorage));
            if (FilePath == null)
                throw new ArgumentNullException(nameof(FilePath));

            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                //Указываем метаданные Excel файла
                excelPackage.Workbook.Properties.Created = DateTime.Now;

                //Создаём лист с графиками и лист с данными с каналов
                ExcelWorksheet graphicSheet = excelPackage.Workbook.Worksheets.Add("Графики");
                ExcelWorksheet dataSheet = excelPackage.Workbook.Worksheets.Add("Данные с каналов");
                
                for(int i = 0; i < DataStorage.ChannelsData.Count; i++)
                {
                    AddChannelDataColumn(dataSheet, DataStorage.GetChannelData(i), i, i, GetChannelTitle(i));
                    CreateChart(graphicSheet, dataSheet, i, DataStorage.GetChannelLastPointIndex(), GetChannelTitle(i), i);
                }

                //Сохраняем файл
                FileInfo fi = new FileInfo(FilePath);
                excelPackage.SaveAs(fi);
            }
        }

        private static string GetChannelTitle(int ChannelIndex)
        {
            return "CH_" + ChannelIndex.ToString();
        }

        private static string GetAxisYName(int ChannelIndex)
        {
            if (ChannelTypeList.TokChannels.Contains(ChannelIndex))
                return "Сила тока, А";
            if (ChannelTypeList.VoltChannels.Contains(ChannelIndex))
                return "Напряжение, В";

            return "";
        }

        //Создаёт график данных с канала
        private static void CreateChart(ExcelWorksheet graphicSheet, ExcelWorksheet dataSheet, int ChannelIndex, int DataLength, string Title, int ChartIndex)
        {
            int DataColumn = ChartIndex * 3 + 1;
            int DataLastRow = DataLength + 3;
            int ChartColumn = ChartIndex * 3 + 1;

            var graphic = graphicSheet.Drawings.AddLineChart(Title, eLineChartType.Line);
            graphic.SetSize(ChartWidth, ChartHeight);
            graphic.SetPosition(ChartColumn / 3 * ChartHeight, 0);
            graphic.StyleManager.SetChartStyle(ePresetChartStyle.LineChartStyle1, ePresetChartColors.ColorfulPalette1); //ePresetChartStyle.LineChartStyle1 - стиль графика
            graphic.XAxis.DisplayUnit = 1000;

            ExcelRange timeRange = dataSheet.Cells[3, DataColumn, DataLastRow, DataColumn];
            ExcelRange dataRange = dataSheet.Cells[3, DataColumn + 1, DataLastRow, DataColumn + 1];

            graphic.Series.Add(dataRange, timeRange);

            graphic.XAxis.AddGridlines();
            graphic.XAxis.Title.Text = "Время, с";

            graphic.YAxis.Title.Text = GetAxisYName(ChannelIndex);
            graphic.YAxis.Title.TextBody.VerticalText = OfficeOpenXml.Drawing.eTextVerticalType.Vertical270;

            graphic.Title.Text = Title;
            graphic.Series[0].Header = Title;
            graphic.Legend.Position = eLegendPosition.TopRight;

            graphic.YAxis.Crosses = 0;
        }

        //Создаёт колонку с данными с канала
        private static void AddChannelDataColumn(ExcelWorksheet dataSheet, Collection<Point> points, int ChannelIndex, int ChartIndex, string ColumnTitle)
        {
            int column = ChartIndex * 3 + 1;

            dataSheet.Cells[2, column].Value = "Время, с";
            dataSheet.Cells[2, column + 1].Value = GetAxisYName(ChannelIndex);

            for(int i = 0; i < points.Count; i++)
            {
                dataSheet.Cells[i + 3, column].Value = points[i].X / 1000;
                dataSheet.Cells[i + 3, column + 1].Value = points[i].Y;
            }

            dataSheet.Columns[column].AutoFit();
            dataSheet.Columns[column + 1].AutoFit();

            dataSheet.Cells[1, column, 1, column + 1].Merge = true;
            dataSheet.Cells[1, column].Value = ColumnTitle;//GetChannelTitle(ChannelIndex);
            dataSheet.Cells[1, column].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            dataSheet.Cells[1, column].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        }
    }
}

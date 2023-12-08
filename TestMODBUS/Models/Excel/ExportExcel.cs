using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Drawing.Chart.Style;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using TestMODBUS.Exceptions;
using TestMODBUS.Models.Data;

namespace TestMODBUS.Models.Excel
{
    public static class ExportExcel
    {
        //Размеры чартов в Excel
        private const int ChartHeight = 450;
        private const int ChartWidth = 900;
        
        //Проверка, открыт ли файл в другой программе
        private static bool isFileOpen(string FilePath)
        {
            if (!File.Exists(FilePath))
                return false;

            StreamReader reader;
            try
            {
                reader = new StreamReader(FilePath);
                reader.Close();
                return false;
            }
            catch (IOException)
            {
                return true;
            }
        }

        //Библиотека EPPlus платная для коммерческого использования и бесплатная для некомерческого использования
        //Перед тем, как её использовать в программе нужно указать тип LicenseContext
        public static void SetUp()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public static void SaveData(Data.Data DataStorage, string FilePath)
        {
            if (isFileOpen(FilePath))
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
                    AddChannelDataColumn(dataSheet, DataStorage.GetChannelData(i), i);
                    CreateChart(graphicSheet, dataSheet, i, DataStorage.GetChannelLastPointIndex());
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

        //Создаёт график данных с канала
        private static void CreateChart(ExcelWorksheet graphicSheet, ExcelWorksheet dataSheet, int ChannelIndex, int DataLength)
        {
            int DataColumn = ChannelIndex * 3 + 1;
            int DataLastRow = DataLength + 3;
            int ChartColumn = ChannelIndex * 3 + 1;
            string ChartTitle = GetChannelTitle(ChannelIndex);

            var graphic = graphicSheet.Drawings.AddLineChart(ChartTitle, eLineChartType.Line);
            graphic.SetSize(ChartWidth, ChartHeight);
            graphic.SetPosition(ChartColumn / 3 * ChartHeight, 0);
            graphic.StyleManager.SetChartStyle(ePresetChartStyle.LineChartStyle1, ePresetChartColors.ColorfulPalette1); //ePresetChartStyle.LineChartStyle1 - стиль графика
            graphic.XAxis.DisplayUnit = 1000;

            ExcelRange timeRange = dataSheet.Cells[3, DataColumn, DataLastRow, DataColumn];
            ExcelRange dataRange = dataSheet.Cells[3, DataColumn + 1, DataLastRow, DataColumn + 1];

            graphic.Series.Add(dataRange, timeRange);

            graphic.XAxis.AddGridlines();
            graphic.XAxis.Title.Text = "Время, мс";

            graphic.YAxis.Title.Text = "Напряжение, В";
            graphic.YAxis.Title.TextBody.VerticalText = OfficeOpenXml.Drawing.eTextVerticalType.Vertical270;

            graphic.Title.Text = ChartTitle;
            graphic.Series[0].Header = ChartTitle;
            graphic.Legend.Position = eLegendPosition.TopRight;

            graphic.YAxis.Crosses = 0;
        }

        //Создаёт колонку с данными с канала
        private static void AddChannelDataColumn(ExcelWorksheet dataSheet, ObservableCollection<Point> points, int ChannelIndex)
        {
            int column = ChannelIndex * 3 + 1;

            dataSheet.Cells[2, column].Value = "Время, мс";
            dataSheet.Cells[2, column + 1].Value = "Напряжение, В";

            for(int i = 0; i < points.Count; i++)
            {
                dataSheet.Cells[i + 3, column].Value = points[i].X;
                dataSheet.Cells[i + 3, column + 1].Value = points[i].Y;
            }

            dataSheet.Columns[column].AutoFit();
            dataSheet.Columns[column + 1].AutoFit();

            dataSheet.Cells[1, column, 1, column + 1].Merge = true;
            dataSheet.Cells[1, column].Value = GetChannelTitle(ChannelIndex);
            dataSheet.Cells[1, column].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            dataSheet.Cells[1, column].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        }
    }
}

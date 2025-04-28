using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBusTPU.Models.Mercucry
{
    public class MercuryDataChartPreparation
    {
        /*
         * Так как LiveCharts не оптимизирована для вывода больших данных, ответственность падает на нас
         * Этот класс берет из MercuryDataStorage только те данные, которые нужно отобразить на экране
         * Для примера см. Models.Modbus.ModbusSenser.ChartDataPrepatations
         * Для выборки данных используется класс WindowidDataHelper
         */
        //TODO: прописать класс для получения данных на отображения данных в чарт
    }
}

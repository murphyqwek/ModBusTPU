using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMODBUS.Models.Modbus
{
    //Дата-класс, хранящий команды, а также массив данных, использующийся при подсчёте контрольной суммы
    public static class ModBusCommandsList
    { 
        //Команда на получение имя модуля
        static private byte[] _getModuleNameCommand = new byte[]
        {
            0x02, 
            0x46, 
            0x00
        };

        //Команда на считывание данных с канала
        static private byte[] _readChannelCommandBase = new byte[]
        {
            0x02, //0 - адрес модуля
            0x04, //1
            0x00, //2
            0x00, //3 - номер канала
            0x00, //4
            0x01, //5
        };

        public static byte[] GetModuleNameCommand()
        {
            return _getModuleNameCommand;
        }

        public static byte[] GetReadChannelCommand(int ChannelNumber)
        {
            if (ChannelNumber < 0 || ChannelNumber > 7)
            {
                throw new ArgumentOutOfRangeException("ChannelNumber", "Неверный номер канала");
            }

            byte[] readChannelCommand = new byte[_readChannelCommandBase.Length];
            Array.Copy(_readChannelCommandBase, readChannelCommand, _readChannelCommandBase.Length);

            readChannelCommand[3] = Convert.ToByte(ChannelNumber);
            var crc = ModBusCRC.CalculateCRC(readChannelCommand); //Получаем контрольную сумму команды

            var readChannelCommandWithCrc = readChannelCommand.ToList();

            readChannelCommandWithCrc.Add(crc[0]);
            readChannelCommandWithCrc.Add(crc[1]);

            return readChannelCommandWithCrc.ToArray();
        }
    }
}

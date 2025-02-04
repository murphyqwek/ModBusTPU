using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using Modbus.Device;
using System.Threading;
using ModBusTPU.Models.Data;
using System.Windows.Media.TextFormatting;
using OfficeOpenXml.ConditionalFormatting;
using System.ComponentModel;

namespace ModBusTPU.Models.Modbus
{
    internal class Motor
    {
        private DataStorage dataStorage;
        private bool working = false;

        static string PORT = "COM9";
        static int BAUDRATE = 9600;
        static byte ADDRESS = 1;

        static SerialPort serialPort;
        static ModbusSerialMaster master;
        static double currentThreshold = 125; // Уставка по току
        static double voltageThreshold = 40; // Уставка по напряжению

        const double highCurrentThresholdBound = 15;
        const double lowCurrentThresholdBound = 5;

        const double highVoltageThresholdBound = 5;
        const double lowVoltageThresholdBound = 5;

        const int KZDELAY = 5000;

        const int WRITEDELAY = 20;
        const int ITERATIONDELAY = 30;
        const int REVERSDELAY = 3000;
        const int REVERSESPEED = 500;
        const int SPEED = 100;

        public Motor(DataStorage dataStorage)
        {
            serialPort = new SerialPort(PORT, BAUDRATE, Parity.None, 8, StopBits.One);
            serialPort.ReadTimeout = 500;
            serialPort.WriteTimeout = 500;

            this.dataStorage = dataStorage;
        }

        public void WriteRegister(ushort register, ushort value)
        {
            master.WriteSingleRegister(ADDRESS, register, value);
            Thread.Sleep(WRITEDELAY);
        }

        public ushort? ReadRegister(ushort register)
        {
            try
            {
                return master.ReadHoldingRegisters(ADDRESS, register, 1)[0];
            }
            catch
            {
                return null;
            }
        }

        private double GetCurrent3()
        {
            if(dataStorage.GetChannelLength() < 5)
            {
                return 125;
            }

            double current3 = 0;
            lock (this)
            {
                for(int i = 0; i < 5; i++)
                {
                    double raw = dataStorage.GetChannelData(0)[dataStorage.GetChannelLength() - 1 - i].Y;
                    current3 += ModBusValueConverter.ConvertToAmperValue(raw);
                }
            }

            current3 = current3 / 5;

            return current3;
        }

        public void ControlMotor()
        {
            WriteRegister(0x000F, 1);

            double current = 0;
            double voltage = 0;

            WriteRegister(0x0103, 50);
            Console.WriteLine("Вниз до кз");
            while (current < 145 && working)
            {
                WriteRegister(0x0105, SPEED);
                WriteRegister(0x0100, 1);

                lock (this)
                {
                    current = dataStorage.GetChannelData(0).Last().Y;
                    voltage = dataStorage.GetChannelData(5).Last().Y;
                }

                current = ModBusValueConverter.ConvertToAmperValue(current);
                voltage = ModBusValueConverter.ConvertToVoltValue(voltage);
                Console.WriteLine($"{current} {voltage}");
            }
            Console.WriteLine("КЗ!!!!");
            WriteRegister(0x0100, 3);
            Thread.Sleep(KZDELAY);
            Console.WriteLine("KZDELAY прошел");
            while (working)
            {
                current = GetCurrent3();
                voltage = ModBusValueConverter.ConvertToVoltValue(voltage);

                if (current < currentThreshold - lowCurrentThresholdBound)
                {
                    WriteRegister(0x0105, SPEED);
                    WriteRegister(0x0100, 1);
                }
                else if (current > currentThreshold + highCurrentThresholdBound)
                {
                    WriteRegister(0x0105, SPEED);
                    WriteRegister(0x0100, 0);
                }
                else
                {
                    WriteRegister(0x0105, SPEED);
                    WriteRegister(0x0100, 3);
                }

                /*if (voltage < voltageThreshold-2)
                {
                    WriteRegister(0x0105, SPEED);
                    WriteRegister(0x0100, 0);
                }
                else if (voltage > voltageThreshold+2)
                {
                    WriteRegister(0x0105, SPEED);
                    WriteRegister(0x0100, 1);
                }
                else
                {
                    WriteRegister(0x0105, SPEED);
                    WriteRegister(0x0100, 3);
                }*/
                Thread.Sleep(ITERATIONDELAY);
            }

            WriteRegister(0x0103, 10000);
            WriteRegister(0x0105, REVERSESPEED);
            WriteRegister(0x0100, 0);
            Thread.Sleep(REVERSDELAY);
            WriteRegister(0x0100, 3);

            WriteRegister(0x000F, 0);
            serialPort.Close();
        }

        public void RunMotor()
        {
            try
            {
                serialPort.Open();
                master = ModbusSerialMaster.CreateRtu(serialPort);
                //Console.WriteLine("Соединение установлено! Начинаем контроль параметров...");
                var listenningThread = new Thread(() => ControlMotor());
                working = true;
                //Console.WriteLine(ReadRegister(0x0101));
                listenningThread.Start();
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Ошибка подключения: {ex.Message}");
            }
            finally
            { 
                //Console.WriteLine("Соединение закрыто.");
            }
        }

        public void StopMotor()
        {
            working = false;
        }
    }
}

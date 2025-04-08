using ModBusTPU.Models.Data;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Threading;

namespace ModBusTPU.Models.Modbus
{
    internal class Motor
    {
        private DataStorage dataStorage;
        private bool working = false;
        private bool contact = false;
        private bool boundariesSet = false;  // Флаг, что границы рассчитаны


        static string PORT = "COM13"; // Замените на ваш порт
        static int BAUDRATE = 115200;
        static SerialPort serialPort;
        //300- 275, 250; 200- 194, 165; 100 - 113 78; 150- 153 121; 250- 235, 210;350- 322, 298; 400- 370 342
        //private double currentThreshold = 275; // Уставка по току
        private double currentThreshold = 1000; // Уставка по току
        private double voltageThreshold = 40; // Уставка по напряжению

        private double highCurrentThresholdBound = 1000;
        //const double lowCurrentThresholdBound = 250;
        private double lowCurrentThresholdBound = 10;
        private Queue<double> currentBuffer = new Queue<double>();  // Очередь для хранения последних 3 значений
        private double bufferIndex = 0;
        //TODO: добавить все параметры в отдельное окно настроек, добавить связь по напряжению, разобраться почему по каналу напряжения идет ерунда
        const double highVoltageThresholdBound = 5;
        const double lowVoltageThresholdBound = 5;
        double current = 0;
        double voltage = 0;

        private static readonly Dictionary<int, (double lowBound, double highBound, double currentThreshold)> boundaryCoefficients = new Dictionary<int, (double, double, double)>
        {
            { 100,  (0.675, 0.730, 0.99) },
            { 150,  (0.801, 0.819, 0.995) },
            { 200,  (0.857, 0.876, 0.997) },
            { 250,  (0.877, 0.896, 0.998) },
            { 300,  (0.908, 0.924, 0.999) },
            { 350,  (0.925, 0.939, 0.999) },
            { 400,  (0.951, 0.963, 0.999) },
            { 450,  (0.952, 0.965, 0.999) }
        };
        //TODO: подумать как эту калибровочную таблицу лучше сделать в настройках
        const int SYNTHTIME = 30000;
        const int KZDELAY = 3000;


        private (double lowBound, double highBound, double currentThreshold) GetCoefficients(double stableMax)
        {
            var keys = boundaryCoefficients.Keys.OrderBy(k => k).ToList();

            // Если значение точно есть в таблице, возвращаем его
            if (boundaryCoefficients.ContainsKey((int)stableMax))
            {
                return boundaryCoefficients[(int)stableMax];
            }

            // Поиск ближайших двух точек
            int lowerKey = keys.First();
            int upperKey = keys.Last();

            for (int i = 0; i < keys.Count - 1; i++)
            {
                if (stableMax >= keys[i] && stableMax <= keys[i + 1])
                {
                    lowerKey = keys[i];
                    upperKey = keys[i + 1];
                    break;
                }
            }

            // Линейная интерполяция между ближайшими точками
            double factor = (stableMax - lowerKey) / (upperKey - lowerKey);

            double interpolatedLow = boundaryCoefficients[lowerKey].lowBound +
                factor * (boundaryCoefficients[upperKey].lowBound - boundaryCoefficients[lowerKey].lowBound);

            double interpolatedHigh = boundaryCoefficients[lowerKey].highBound +
                factor * (boundaryCoefficients[upperKey].highBound - boundaryCoefficients[lowerKey].highBound);

            double interpolatedThreshold = boundaryCoefficients[lowerKey].currentThreshold +
                factor * (boundaryCoefficients[upperKey].currentThreshold - boundaryCoefficients[lowerKey].currentThreshold);

            return (interpolatedLow, interpolatedHigh, interpolatedThreshold);
        }



        public Motor(DataStorage dataStorage)
        {
            serialPort = new SerialPort(PORT, BAUDRATE, Parity.None, 8, StopBits.One);
            serialPort.ReadTimeout = 500;
            serialPort.WriteTimeout = 500;

            this.dataStorage = dataStorage;
        }

        public double FindStableMaximum(double newCurrent)
        {
            if (newCurrent <= 10)
            {
                return -1;  // Игнорируем слишком малые значения
            }

            if (currentBuffer.Count >= 10)
            {
                currentBuffer.Dequeue();  // Удаляем старое значение
            }
            currentBuffer.Enqueue(newCurrent);  // Добавляем новое значение

            if (currentBuffer.Count < 10)
            {
                return -1; // Недостаточно данных
            }

            double maxVal = currentBuffer.Max();
            double minVal = currentBuffer.Min();

            if (((maxVal - minVal) / (float)maxVal) <= 0.05)
            {
                return maxVal;  // Если колебания в пределах 5%, считаем это максимумом
            }

            return -1;  // Максимум еще не найден
        }

        public bool ProcessCurrent(double newCurrent, out double stableMax)
        {
            stableMax = FindStableMaximum(newCurrent);

            if (stableMax != -1)
            {
                Console.WriteLine($"Стабильный максимум найден: {stableMax}");
                return true;  // Максимум найден, можно начинать расчет границ
            }

            return false;  // Еще нет стабильного максимума
        }

        /*public void CalculateBoundaries(double stableMax)
        {
            lowCurrentThresholdBound = (stableMax * 0.89);  // 89% от максимального тока
            highCurrentThresholdBound = (stableMax * 0.95);  // 91% от максимального тока
            currentThreshold = (stableMax * 0.999);
        }*/
        public void CalculateBoundaries(double stableMax)
        {
            var (lowCoef, highCoef, thresholdCoef) = GetCoefficients(stableMax);

            lowCurrentThresholdBound = stableMax * lowCoef;
            highCurrentThresholdBound = stableMax * highCoef;
            currentThreshold = stableMax * thresholdCoef;

            Console.WriteLine($"Рассчитаны границы: {lowCurrentThresholdBound} - {highCurrentThresholdBound}, уставка {currentThreshold}");
        }


        public void UpdateCurrent(double newCurrent)
        {
            double stableMax;

            if (ProcessCurrent(newCurrent, out stableMax))
            {
                CalculateBoundaries(stableMax);  // Вызываем расчет границ
                contact = true;
                boundariesSet = true;  // После первого выполнения больше не вызываем UpdateCurrent
            }
        }


        private double GetCurrent3()
        {
            if (dataStorage.GetChannelLength() < 2)
            {
                return 5;
            }

            double current3 = 0;
            lock (this)
            {
                for (int i = 0; i < 2; i++)
                {
                    double raw = dataStorage.GetChannelData(0)[dataStorage.GetChannelLength() - 1 - i].Y;
                    current3 += ModBusValueConverter.ConvertToAmperValue(raw);
                }
            }

            current3 = current3 / 2;

            return current3;
        }

        public void RunMotor()
        {
            string command = "SET " + SYNTHTIME.ToString() + " " + KZDELAY.ToString() 
                + " " + currentThreshold.ToString() + " " + lowCurrentThresholdBound.ToString() 
                + " " + highCurrentThresholdBound.ToString();
            serialPort.Open();
            Thread.Sleep(2000);
            Console.WriteLine(command);
            serialPort.WriteLine(command);
            command = "RUN";
            Thread.Sleep(500);
            serialPort.WriteLine(command);
            Console.WriteLine(command);
            Thread.Sleep(500);
            var listenningThread = new Thread(() => ControlMotor());
            working = true;
            listenningThread.Start();
        }
        public void StopMotor()
        {
            working = false;
            serialPort.WriteLine("STOP");
            Thread.Sleep(500);
            serialPort.Close();
            contact = false;
            boundariesSet = false;
            currentThreshold = 1000;
            lowCurrentThresholdBound = 10;
            currentBuffer.Dequeue();
        }

        public void ControlMotor()
        {
            while (working)
            {
                current = GetCurrent3();
                if (!contact && !boundariesSet)  // Если границы еще не установлены
                {
                    UpdateCurrent(current);
                }
                voltage = ModBusValueConverter.ConvertToVoltValue(voltage);
                string command = "CUR " + current.ToString();
                Console.WriteLine(command);
                serialPort.WriteLine(command);
                Thread.Sleep(50);
                if (contact)
                {
                    command = "SET " + SYNTHTIME.ToString() + " " + KZDELAY.ToString()
                + " " + ((int)currentThreshold).ToString() + " " + ((int)lowCurrentThresholdBound).ToString()
                + " " + ((int)highCurrentThresholdBound).ToString();
                    serialPort.WriteLine(command);
                    Thread.Sleep(50);
                    contact = false;
                }
            }
            Thread.Sleep(500);
            
        }
        
    }
}


/*using System;
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
using ModBusTPU.Services.Settings.MotorSettings;

namespace ModBusTPU.Models.Modbus
{
    internal class Motor
    {
        private DataStorage dataStorage;
        private bool working = false;

        static string PORT = "COM13";
        static int BAUDRATE = 9600;
        static byte ADDRESS = 1;

        static SerialPort serialPort;
        static ModbusSerialMaster master;

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
            this.dataStorage = dataStorage;
            this.settings = settings;
            serialPort = new SerialPort(settings.PORT, settings.BAUDRATE, Parity.None, 8, StopBits.One);
            serialPort.ReadTimeout = 500;
            serialPort.WriteTimeout = 500;
        }

        public void WriteRegister(ushort register, ushort value)
        {
            master.WriteSingleRegister(settings.ADDRESS, register, value);
            Thread.Sleep(settings.WRITEDELAY);
        }

        public ushort? ReadRegister(ushort register)
        {
            try
            {
                return master.ReadHoldingRegisters(settings.ADDRESS, register, 1)[0];
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
                WriteRegister(0x0105, settings.SPEED);
                WriteRegister(0x0100, 1);

                lock (this)
                {
                    current = dataStorage.GetChannelData(0).Last().Y;
                    voltage = dataStorage.GetChannelData(1).Last().Y;
                }

                current = ModBusValueConverter.ConvertToAmperValue(current);
                voltage = ModBusValueConverter.ConvertToVoltValue(voltage);
                Console.WriteLine($"{current} {voltage}");
            }
            Console.WriteLine("КЗ!!!!");
            WriteRegister(0x0100, 3);
            Thread.Sleep(settings.KZDELAY);
            Console.WriteLine("KZDELAY прошел");
            while (working)
            {
                current = GetCurrent3();
                voltage = ModBusValueConverter.ConvertToVoltValue(voltage);

                if (current < settings.currentThreshold - settings.lowCurrentThresholdBound)
                {
                    WriteRegister(0x0105,settings.SPEED);
                    WriteRegister(0x0100, 1);
                }
                else if (current > settings.currentThreshold + settings.highCurrentThresholdBound)
                {
                    WriteRegister(0x0105, settings.SPEED);
                    WriteRegister(0x0100, 0);
                }
                else
                {
                    WriteRegister(0x0105, settings.SPEED);
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
                }
                Thread.Sleep(ITERATIONDELAY);
            }

            WriteRegister(0x0103, 10000);
            WriteRegister(0x0105, settings.REVERSESPEED);
            WriteRegister(0x0100, 0);
            Thread.Sleep(settings.REVERSDELAY);
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
}*/

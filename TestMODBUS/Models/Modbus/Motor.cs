/*using ModBusTPU.Models.Data;
using ModBusTPU.Services.Settings.MotorSettings;
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

        private MotorSettingsContainer settingsContainer;


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



        public Motor(DataStorage dataStorage, MotorSettingsContainer container)
        {
            serialPort = new SerialPort(container.PORT, container.BAUDRATE, Parity.None, 8, StopBits.One);
            serialPort.ReadTimeout = 500;
            serialPort.WriteTimeout = 500;

            this.settingsContainer = container;
            this.dataStorage = dataStorage;

            container.PropertyChanged += UpdateDate;
        }

        private void UpdateDate(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            serialPort.PortName = this.settingsContainer.PORT;
            serialPort.BaudRate = this.settingsContainer.BAUDRATE;
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
        }
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
            serialPort.PortName = settingsContainer.PORT;

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
}*/


using ModBusTPU.Models.Data;
using ModBusTPU.Services.Settings.MotorSettings;
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
        private bool boundariesSet = false;

        static SerialPort serialPort;

        private double voltage = 80;

        private const int SYNTHTIME = 120000;
        private const int KZDELAY = 3000;

        public MotorSettingsContainer MotorSettings { get; }

        public Motor(DataStorage dataStorage, MotorSettingsContainer motorSettings)
        {
            MotorSettings = motorSettings;
            this.dataStorage = dataStorage;

            serialPort = new SerialPort(motorSettings.PORT, motorSettings.BAUDRATE, Parity.None, 8, StopBits.One);
            serialPort.ReadTimeout = 500;
            serialPort.WriteTimeout = 500;

            MotorSettings.PropertyChanged += MotorPortUpdatesHander;

        }

        private void MotorPortUpdatesHander(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                serialPort.PortName = MotorSettings.PORT;
                serialPort.BaudRate = MotorSettings.BAUDRATE;
            } catch(Exception _)
            {

            }
        }

        private double GetVoltage()
        {
            double sum = 0;

            lock (this)
            {
                var channelData = dataStorage.GetChannelData(1);
                int length = channelData.Count;

                if (length < 2)
                    return 80;

                for (int i = 0; i < 2; i++)
                {
                    double raw = channelData[length - 1 - i].Y;
                    sum += ModBusValueConverter.ConvertToVoltValue(raw);
                }
            }

            return sum / 2;
        }

        public void RunMotor()
        {
            string command = "SET " + SYNTHTIME.ToString() + " " + KZDELAY.ToString();
            serialPort.Open();
            Thread.Sleep(2000);
            Console.WriteLine(command);
            serialPort.WriteLine(command);

            Thread.Sleep(500);
            serialPort.WriteLine("RUN");
            Console.WriteLine("RUN");

            Thread.Sleep(500);
            working = true;

            var listenningThread = new Thread(() => ControlMotor());
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
        }

        public void ControlMotor()
        {
            while (working)
            {
                voltage = GetVoltage();

                //voltage = ModBusValueConverter.ConvertToVoltValue(voltage);
                string command = "CUR " + voltage.ToString();
                Console.WriteLine(command);
                serialPort.WriteLine(command);
                Thread.Sleep(50);


            }
            Thread.Sleep(200);
        }

    }
}
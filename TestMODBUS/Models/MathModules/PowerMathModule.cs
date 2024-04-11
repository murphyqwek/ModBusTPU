using LiveCharts.Defaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMODBUS.Models.Data;

namespace TestMODBUS.Models.MathModules
{
    static public class PowerMathModule
    {
        static public List<Point> Apply(List<Point> TokPoints, List<Point> VoltPoints)
        {
            if (TokPoints.Count != VoltPoints.Count)
                throw new ArgumentException($"{TokPoints} Count must be equal {VoltPoints} Count");
            List<Point> PowerPoints = new List<Point>();

            for(int i = 0; i < TokPoints.Count; i++)
                PowerPoints.Add(new Point(TokPoints[i].X, CountKV(TokPoints[i].Y , VoltPoints[i].Y)));

            return PowerPoints;
        }

        static public List<ObservablePoint> Apply(List<ObservablePoint> TokPoints, List<ObservablePoint> VoltPoints)
        {
            if (TokPoints.Count != VoltPoints.Count)
                throw new ArgumentException($"{TokPoints} Count must be equal {VoltPoints} Count");
            List<ObservablePoint> PowerPoints = new List<ObservablePoint>();

            for (int i = 0; i < TokPoints.Count; i++)
                PowerPoints.Add(new ObservablePoint(TokPoints[i].X, CountKV(TokPoints[i].Y, VoltPoints[i].Y)));

            return PowerPoints;
        }

        static public double Count(double Tok, double Volt) => Tok * Volt;

        static public double CountKV(double Tok, double Volt) => Volt * Tok / 1000;
        static public double CountKV(double Vvat) => Vvat / 1000;
    }
}

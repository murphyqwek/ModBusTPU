using LiveCharts.Defaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBusTPU.Models.MathModules
{
    static public class EnergyMathModule
    {
        static public List<ObservablePoint> Apply(List<ObservablePoint> TokPoints, List<ObservablePoint> VoltPoints)
        {
            if (TokPoints.Count != VoltPoints.Count)
                throw new ArgumentException($"{TokPoints} Count must be equal {VoltPoints} Count");
            List<ObservablePoint> PowerPoints = new List<ObservablePoint>();

            if (TokPoints.Count == 0)
                return PowerPoints;
            //TokPoints[0].Y * VoltPoints[0].Y * TokPoints[0].X
            PowerPoints.Add(new ObservablePoint(TokPoints[0].X, CountWithMilliseconds(TokPoints[0].Y, VoltPoints[0].Y, TokPoints[0].X)));

            for (int i = 1; i < TokPoints.Count; i++)
            {
                PowerPoints.Add(new ObservablePoint(TokPoints[i].X,
                                                    CountWithMilliseconds(TokPoints[i].Y, VoltPoints[i].Y, TokPoints[i].X - TokPoints[i - 1].X)));
            }
            return PowerPoints;
        }

        static public double CountWithMilliseconds(double Tok, double Volt, double DeltaTime) => Tok * Volt * DeltaTime / 1000 / 1000 / 3600;
    }
}

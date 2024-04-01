using LiveCharts.Defaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMODBUS.Models.MathModules
{
    static public class EnergyMathModule
    {
        static public List<ObservablePoint> Apply(List<ObservablePoint> TokPoints, List<ObservablePoint> VoltPoints)
        {
            if (TokPoints.Count != VoltPoints.Count)
                throw new ArgumentException($"{TokPoints} Count must be equal {VoltPoints} Count");
            List<ObservablePoint> PowerPoints = new List<ObservablePoint>();

            for (int i = 0; i < TokPoints.Count; i++)
                PowerPoints.Add(new ObservablePoint(TokPoints[i].X, TokPoints[i].Y * VoltPoints[i].Y * TokPoints[i].X));

            return PowerPoints;
        }
    }
}

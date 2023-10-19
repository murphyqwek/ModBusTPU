using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMODBUS.Models.Data
{
    public class Point
    {
        public readonly double X, Y;

        public Point(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}

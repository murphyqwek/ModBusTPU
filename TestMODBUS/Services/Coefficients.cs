using ModBusTPU.Models.Coefficients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ModBusTPU.Services
{
    public static class Coefficients
    {
        #region Public Attributes
        public static double HolostMove { get => _holostMove; }
        public static double AmperKoeff { get => _amperKoeff; }
        public static double VoltKoeff { get => _voltKoeff; }
        public static double KoeffValueChannel { get => _koeffValueChannel; }
        #endregion


        private static double _holostMove = 0.623;
        private static double _amperKoeff = 200;
        private static double _voltKoeff = 124.5;
        private static double _koeffValueChannel = 0.00030517578125;


        public static void SetCoefficients(double HolostMove, double AmperKoeff, double VoltKoeff, double KoeffValueChannel)
        {
            _holostMove = HolostMove;
            _amperKoeff = AmperKoeff;
            _voltKoeff = VoltKoeff;
            _koeffValueChannel = KoeffValueChannel;
        }

        public static void SetCoefficients(List<double> NewCoefficients)
        {
            if(NewCoefficients == null)
                throw new ArgumentNullException(nameof(NewCoefficients));

            if (NewCoefficients.Count != 4)
                throw new ArgumentException("Not all new coefficient included");

            SetCoefficients(NewCoefficients[0], NewCoefficients[1],
                            NewCoefficients[2], NewCoefficients[3]);
        }

        public static void SetCoefficients(CoefficientProfile Profile)
        {
            _holostMove = Profile.HolostMove;
            _amperKoeff = Profile.AmperKoeff;
            _voltKoeff = Profile.VoltKoeff;
            _koeffValueChannel = Profile.KoeffValueChannel;
        }
    }
}

using ModBusTPU.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TestMODBUS.Services.Channels
{
    public static class ChannelTypeColors
    {
        #region RawData
        private static Brush RawDataBackroundChosen = new SolidColorBrush(Color.FromRgb(247, 245, 245));
        private static Brush RawDataForegroundChosen = new SolidColorBrush(Color.FromRgb(35, 36, 35));

        private static Brush RawDataBackgroundUnchosen = new SolidColorBrush(Color.FromRgb(185, 189, 186));
        private static Brush RawDataForegroundUnchosen = new SolidColorBrush(Color.FromRgb(35, 36, 35));
        #endregion

        #region Tok
        private static Brush TokBackroundChosen = new SolidColorBrush(Color.FromRgb(28, 7, 217));
        private static Brush TokForegroundChosen = new SolidColorBrush(Color.FromRgb(250, 250, 255));


        private static Brush TokBackgroundUnchosen = new SolidColorBrush(Color.FromRgb(138, 126, 247));
        private static Brush TokForegroundUnchosen = new SolidColorBrush(Color.FromRgb(35, 36, 35));

        #endregion

        #region Volt
        private static Brush VoltBackroundChosen = new SolidColorBrush(Color.FromRgb(252, 8, 8));
        private static Brush VoltForegroundChosen = new SolidColorBrush(Color.FromRgb(250, 250, 255));


        private static Brush VoltBackgroundUnchosen = new SolidColorBrush(Color.FromRgb(250, 112, 112));
        private static Brush VoltForegroundUnchosen = new SolidColorBrush(Color.FromRgb(35, 36, 35));
        #endregion

        public static Brush GetBackgroundColor(int Channel, bool IsChosen)
        {
            var Type = ChannelTypeList.GetChannelType(Channel);
            return GetBackgroundColor(Type, IsChosen);
        }

        public static Brush GetBackgroundColor(ChannelType Type, bool IsChosen)
        {
            switch (Type)
            {
                case ChannelType.Regular:
                    if (IsChosen)
                        return RawDataBackroundChosen;
                    else
                        return RawDataBackgroundUnchosen;
                case ChannelType.Tok:
                    if (IsChosen)
                        return TokBackroundChosen;
                    else
                        return TokBackgroundUnchosen;
                case ChannelType.Volt:
                    if (IsChosen)
                        return VoltBackroundChosen;
                    else
                        return VoltBackgroundUnchosen;
                default:
                    throw new Exception("Uncatched Channel Type");
            }
        }

        public static Brush GetForegroundColor(int Channel, bool IsChosen)
        {
            var Type = ChannelTypeList.GetChannelType(Channel);
            return GetForegroundColor(Type, IsChosen);
        }

        public static Brush GetForegroundColor(ChannelType Type, bool IsChosen)
        {
            switch (Type)
            {
                case ChannelType.Regular:
                    if (IsChosen)
                        return RawDataForegroundChosen;
                    else
                        return RawDataForegroundUnchosen;
                case ChannelType.Tok:
                    if (IsChosen)
                        return TokForegroundChosen;
                    else
                        return TokForegroundUnchosen;
                case ChannelType.Volt:
                    if (IsChosen)
                        return VoltForegroundChosen;
                    else
                        return VoltForegroundUnchosen;
                default:
                    throw new Exception("Uncatched Channel Type");
            }
        }
    }
}

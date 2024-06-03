using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ModBusTPU.Extensions
{
    public class IsChannelSelectedExtension
    {
        public static readonly DependencyProperty IsChannelSelectedProperty = DependencyProperty.RegisterAttached(
            "IsChannelSelected",
            typeof(bool),
            typeof(IsChannelSelectedExtension),
            new PropertyMetadata(false));

        public static void SetIsChannelSelected(UIElement element, bool value)
        {
            element.SetValue(IsChannelSelectedProperty, value);
        }

        public static bool GetIsChannelSelected(UIElement element)
        {
            return (bool)element.GetValue(IsChannelSelectedProperty);
        }
    }
}

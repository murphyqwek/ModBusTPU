using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TestMODBUS.Extensions
{
    internal class ExtraDataFieldTooltipText
    {
        public static readonly DependencyProperty ExtraDataFieldTooltipTextProperty = DependencyProperty.RegisterAttached(
            "ExtraDataFieldTooltipText",
            typeof(string),
            typeof(ExtraDataFieldTooltipText),
            new PropertyMetadata(""));

        public static void SetExtraDataFieldTooltipText(UIElement element, string value)
        {
            element.SetValue(ExtraDataFieldTooltipTextProperty, value);
        }

        public static string GetExtraDataFieldTooltipText(UIElement element)
        {
            return (string)element.GetValue(ExtraDataFieldTooltipTextProperty);
        }
    }
}

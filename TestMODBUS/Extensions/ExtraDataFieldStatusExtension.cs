using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TestMODBUS.ViewModels.ExportViewModels;

namespace TestMODBUS.Extensions
{
    public class ExtraDataFieldStatusExtension
    {
         public static readonly DependencyProperty ExtraDataFieldStatusProperty = DependencyProperty.RegisterAttached(
            "ExtraDataFieldStatus",
            typeof(ExtraDataFieldStatus),
            typeof(ExtraDataFieldStatusExtension),
            new PropertyMetadata(ExtraDataFieldStatus.NotAllChannelsChosen));

        public static void SetExtraDataFieldStatus(UIElement element, ExtraDataFieldStatus value)
        {
            element.SetValue(ExtraDataFieldStatusProperty, value);
        }

        public static ExtraDataFieldStatus GetExtraDataFieldStatus(UIElement element)
        {
            return (ExtraDataFieldStatus)element.GetValue(ExtraDataFieldStatusProperty);
        } 
         
    }
}

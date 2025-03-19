using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace ModBusTPU.ViewModels.Field
{
    public class FieldTemplateSelector : DataTemplateSelector
    {
        public DataTemplate StringTemplate { get; set; }
        public DataTemplate IntTemplate { get; set; }
        public DataTemplate ByteTemplate { get; set; }
        public DataTemplate UshortTemplate { get; set; }
        public DataTemplate BoolTemplate { get; set; }
        public DataTemplate DoubleTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var field = item as Field;
            if (field == null) return base.SelectTemplate(item, container);

            if (field.ValueType == typeof(string))
                return StringTemplate;
            if (field.ValueType == typeof(int))
                return IntTemplate;
            if (field.ValueType == typeof(byte))
                return ByteTemplate;
            if (field.ValueType == typeof(ushort))
                return UshortTemplate;
            if (field.ValueType == typeof(bool))
                return BoolTemplate;
            if (field.ValueType == typeof(double))
                return DoubleTemplate;

            return base.SelectTemplate(item, container);
        }
    }

}

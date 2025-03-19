using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBusTPU.ViewModels.Field
{
    public class Field : INotifyPropertyChanged
    {
        private string _name;
        private readonly Func<object> _getValue; // Делегат для получения значения
        private readonly Action<object> _setValue; // Делегат для установки значения

        public Field(string name, Func<object> getValue, Action<object> setValue)
        {
            _name = name;
            _getValue = getValue ?? throw new ArgumentNullException(nameof(getValue));
            _setValue = setValue ?? throw new ArgumentNullException(nameof(setValue));
        }

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public object Value
        {
            get => _getValue(); // Получаем значение из внешнего поля
            set
            {
                object oldValue = _getValue();
                if (!Equals(oldValue, value)) // Сравниваем старое и новое значение
                {
                    _setValue(value); // Устанавливаем значение во внешнее поле
                    OnPropertyChanged(nameof(Value)); // Уведомляем об изменении
                }
            }
        }

        public Type ValueType => _getValue()?.GetType(); // Тип берется из текущего значения

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}

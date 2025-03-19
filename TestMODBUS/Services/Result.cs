using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBusTPU.Services
{
    internal class Result<T>
    {
        public T Value { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage);

        private Result(T value, string errorMessage)
        {
            Value = value;
            ErrorMessage = errorMessage ?? string.Empty;
        }

        public static Result<T> Success(T value) => new Result<T>(value, string.Empty);
        public static Result<T> Failure(string errorMessage) => new Result<T>(default, errorMessage);
    }
}

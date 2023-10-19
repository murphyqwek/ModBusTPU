using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMODBUS.Exceptions
{
    public class FileIsAlreadyOpenException : Exception
    {
        public FileIsAlreadyOpenException(string FilePath) : base(string.Format("Файл по пути {0} уже открыт", FilePath)) {  }
    }
}

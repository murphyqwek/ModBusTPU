using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBusTPU.Exceptions
{
    public class FileAlreadyExist : Exception
    {
        public FileAlreadyExist(string Path) : base($"Файл по пути {Path} уже создан") { }
    }
}

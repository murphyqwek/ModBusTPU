using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMODBUS.Services
{
    public static class RegisrtyService
    {
        private const string MainFolder = "SOFTWARE";
        private const string AppFolder = "TPU ModBus";

        public const string ChannelFolder = "Channels";

        private static string GetFolderPath(string FolderName)
        {
            return $@"{MainFolder}\{AppFolder}\{FolderName}";
        }

        public static string GetField(string FolderName, string FieldName, bool CreateIfNotExist = false)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(GetFolderPath(FolderName)))
            {
                object Value = key?.GetValue(FieldName);

                if(Value == null && CreateIfNotExist)
                    SetField(FolderName, FieldName, null);

                if (Value != null)
                    return Value.ToString();
                else 
                    return string.Empty;
            }
        }

        public static void SetField(string FolderName, string FieldName, object Value)
        {
            using(RegistryKey key = Registry.CurrentUser.CreateSubKey(GetFolderPath(FolderName)))
            {
                key?.SetValue(FieldName, Value);
            }
        }
    }
}

using ModBusTPU.Exceptions;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModBusTPU.Models.Coefficients
{
    public static class CoefficientProfileFileManager
    {
        private static readonly string PROFILE_FILES_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                                                         "ModBusTPU");
        private const string EXTENSION = ".cfp";

        private static void CreateFolder()
        {
            if(!Directory.Exists(PROFILE_FILES_PATH))
                Directory.CreateDirectory(PROFILE_FILES_PATH);
        }

        private static string GetFullProfilePath(string Name) => Path.Combine(PROFILE_FILES_PATH, Name + EXTENSION);

        public static List<string> GetAllProfilesNames()
        {
            CreateFolder();
            List<string> Profiles = new List<string>();
            foreach(string path in Directory.GetFiles(PROFILE_FILES_PATH).ToList())
            {
                if(!path.EndsWith(EXTENSION)) continue;

                var Name = Path.GetFileNameWithoutExtension(path);

                Profiles.Add(Name);
            }

            return Profiles;
        }

        public static void SaveProfile(CoefficientProfile Profile, string Name, bool ForceSave)
        {
            CreateFolder();

            string Path = GetFullProfilePath(Name);
            if (File.Exists(Path) && !ForceSave)
                throw new FileAlreadyExist(Path);

            if (File.Exists(Path))
                File.Delete(Path);

            string Text = $"\"{nameof(CoefficientProfile.HolostMove)}\" = {Profile.HolostMove}\n";
            Text += $"\"{nameof(CoefficientProfile.AmperKoeff)}\" = {Profile.AmperKoeff}\n";
            Text += $"\"{nameof(CoefficientProfile.VoltKoeff)}\" = {Profile.VoltKoeff}\n";
            Text += $"\"{nameof(CoefficientProfile.KoeffValueChannel)}\" = {Profile.KoeffValueChannel}";

            File.WriteAllText(Path, Text);
        }

        private static CoefficientProfile ParseProfile(string Text)
        {
            int GetIndexByName(string name)
            {
                switch(name) 
                {
                    case nameof(CoefficientProfile.HolostMove):
                        return 0;
                    case nameof(CoefficientProfile.AmperKoeff):
                        return 1;
                    case nameof(CoefficientProfile.VoltKoeff):
                        return 2;
                    case nameof(CoefficientProfile.KoeffValueChannel):
                        return 3;
                    default:
                        throw new ArgumentException("Неизвестный коэффициент");
                }
            }

            double[] coeffs = new double[4];

            int count = 0;
            foreach(var line in Text.Split('\n'))
            {
                var match = Regex.Match(line, @"""[\w\d]+""\s*=\s*\d+[,.]?\d*");

                if (!match.Success)
                    continue;

                var matchArgument = Regex.Match(match.Value, @"(?<="")[\w\d]+(?="")");
                var matchValue = Regex.Match(match.Value, @"(?<=\s*=\s*)\d+[,.]?\d*");

                count++;
                string name = matchArgument.Value;
                string strValue = matchValue.Value.Replace('.', ',');
                double value = Convert.ToDouble(strValue);

                coeffs[GetIndexByName(name)] = value;
            }

            if (count != 4)
                throw new ProfileIsDamaged();

            return new CoefficientProfile(coeffs[0], coeffs[1],
                                          coeffs[2], coeffs[3]);
        }

        public static CoefficientProfile GetProfileData(string Name)
        {
            CreateFolder();
            string Path = GetFullProfilePath(Name);

            if (!File.Exists(Path))
                throw new ProfileDoesNotExist();

            string text = File.ReadAllText(Path);

            try
            {
                return ParseProfile(text);
            }
            catch
            {
                throw new ProfileIsDamaged();
            }
        }
    }
}

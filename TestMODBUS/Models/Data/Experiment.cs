using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModBusTPU.Models.Data
{
    public class Experiment
    {
        public Dictionary<string, string> Commentaries { get; }
        public DataStorage DataStorage { get; }

        public Experiment(DataStorage DataStorage)
        {
            this.DataStorage = DataStorage;
        }

        public Experiment(DataStorage DataStorage, IList<string> FieldNames)
        {
            this.DataStorage = DataStorage;
            foreach (var Field in FieldNames)
                Commentaries.Add(Field, "");
        }

        [JsonConstructor]
        public Experiment(Dictionary<string, string> Commentaries, DataStorage DataStorage)
        {
            this.Commentaries = Commentaries;
            this.DataStorage = DataStorage;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TestMODBUS.Models.Data
{
    //Дата-класс, который хранит данные со всех каналов
    public class DataStorage
    {
        public const int MaxChannelCount = 8;

        private List<ObservableCollection<Point>> ChannelsData;
        public Dictionary<string, ObservableCollection<Point>> ExtraData;

        public DataStorage() 
        {
            ChannelsData = new List<ObservableCollection<Point>>();
            ExtraData = new Dictionary<string, ObservableCollection<Point>>();
            for(int i = 0; i < MaxChannelCount; i++)
            {
                ChannelsData.Add(new ObservableCollection<Point>());
            }
        }

        [JsonConstructor]
        public DataStorage(List<ObservableCollection<Point>> channelsData, Dictionary<string, ObservableCollection<Point>> extraData)
        {
            TrimLastPoints(channelsData);
            ChannelsData = channelsData;
            ExtraData = extraData;

            if(ChannelsData.Count > MaxChannelCount)
            {
                ChannelsData.Clear();
            }
            for(int i = ChannelsData.Count; i < MaxChannelCount; i++)
            {
                ChannelsData.Add(new ObservableCollection<Point>());
            }
        }

        private void TrimLastPoints(List<ObservableCollection<Point>> channelsData)
        {
            if (channelsData.Count == 0)
                return;

            //Получаем наименьшее количество полученных точек
            int minLenght = channelsData[0].Count;
            foreach(var Channel in channelsData)
            {
                minLenght = Math.Min(Channel.Count, minLenght);
            }

            if (minLenght == channelsData.Count)
                return;

            foreach (var Channel in channelsData)
            {
                if (Channel.Count == minLenght)
                    continue;
                int ChannelLenght = Channel.Count;
                for (int i = minLenght; i < ChannelLenght; i++)
                    Channel.RemoveAt(Channel.Count - 1);
            }
        }

        public int GetChannelLength()
        {
            return ChannelsData[0].Count;
        }

        public int GetChannelLastPointIndex()
        {
            return ChannelsData[0].Count - 1;
        }

        public void Clear()
        {
            for (int i = 0; i < 8; i++)
            {
                ChannelsData[i].Clear();
            }
            ExtraData.Clear();
        }

        public double GetLastTime()
        {
            if (ChannelsData[0].Count == 0)
                return 0.0;

            return ChannelsData[0].Last().X;
        }

        public double GetLastExtraDataTime(ObservableCollection<Point> ExtraData)
        {
            if (ExtraData.Count == 0)
                return -1;

            return ExtraData.Last().X;
        }

        public ObservableCollection<Point> GetExtraData(string key, IList<int> UsingChannels)
        {
            key = GetKeyToExtraData(key, UsingChannels);

            if (!ExtraData.ContainsKey(key))
                ExtraData.Add(key, new ObservableCollection<Point>());
            return ExtraData[key];
        }

        private string GetKeyToExtraData(string key, IList<int> UsingChannels) 
        {
            string output = key;
            foreach (var channel in UsingChannels)
            {
                output = output + "_" + channel.ToString();
            }
            return output;
        }

        public ObservableCollection<Point> GetChannelData(int Channel)
        {
            if(Channel < 0 || Channel >= ChannelsData.Count)
                throw new ArgumentOutOfRangeException(nameof(Channel));

            return ChannelsData[Channel];
        }

        public void AddNewPoint(Point point, int Channel) 
        {
            if (Channel < 0 || Channel > MaxChannelCount - 1)
                throw new ArgumentOutOfRangeException("Channel");
            ChannelsData[Channel].Add(point);
        }

        public int GetMaxChannelsCount() => MaxChannelCount;

        public List<ObservableCollection<Point>> GetAllChannels() => ChannelsData;

        public Dictionary<string, ObservableCollection<Point>> GetAllExtraData() => ExtraData;

        public void UnsingToChannel(int Channel, NotifyCollectionChangedEventHandler Handler) => ChannelsData[Channel].CollectionChanged -= Handler;

        public void SignToChannel(int Channel, NotifyCollectionChangedEventHandler Handler) => ChannelsData[Channel].CollectionChanged += Handler;

        public void UnsingToAllChannels(NotifyCollectionChangedEventHandler Handler)
        {
            for (int i = 0; i < MaxChannelCount; i++)
                UnsingToChannel(i, Handler);
        }
    }
}

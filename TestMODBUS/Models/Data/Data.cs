using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMODBUS.Models.Data
{
    //Дата-класс, который хранит данные со всех каналов
    public class Data
    {
        public List<ObservableCollection<Point>> ChannelsData;

        public Data() 
        {
            ChannelsData = new List<ObservableCollection<Point>>();
            for(int i = 0; i < 8; i++)
            {
                ChannelsData.Add(new ObservableCollection<Point>());
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
        }

        public double GetLastTime()
        {
            if (ChannelsData[0].Count == 0)
                return 0.0;

            return ChannelsData[0].Last().X;
        }

        public ObservableCollection<Point> GetChannelData(int Channel)
        {
            if(Channel < 0 || Channel >= ChannelsData.Count)
                throw new ArgumentOutOfRangeException(nameof(Channel));

            return ChannelsData[Channel];
        }

        public void AddNewPoint(Point point, int Channel) 
        {
            if (Channel < 0 || Channel > 7)
                throw new ArgumentOutOfRangeException("Channel");
            ChannelsData[Channel].Add(point);
        }
    }
}

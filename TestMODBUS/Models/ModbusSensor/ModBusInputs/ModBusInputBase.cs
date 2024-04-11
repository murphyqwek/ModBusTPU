using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using TestMODBUS.Models.Data;
using TestMODBUS.Models.ModbusSensor.ChartDataPrepatations;

namespace TestMODBUS.Models.ModbusSensor.ModBusInputs
{
    public abstract class ModBusInputBase
    {
        protected ModbusSensorController _controller;

        public ModBusInputBase(ModbusSensorController Controller) 
        { 
            _controller = Controller;
        }

        public ModBusInputBase(ModbusSensorController Controller, IEnumerable<int> Channels)
        {
            _controller = Controller;

            if (Channels == null)
                return;

            foreach (int Channel in Channels)
                AddNewChannel(Channel);
        }

        public virtual void Detach()
        {
            _controller.DetachInputModule(DataStorageCollectionChangedHandler);
            _controller = null;
        }

        public abstract void AddNewChannel(int Channel);

        public abstract void RemoveChannel(int Channel);

        protected virtual void ClearChannels()
        {
            _controller.ClearChannels();
        }

        protected virtual void UpdateSeries()
        {
            _controller.UpdateChart(true);
        }

        public virtual bool Start()
        {
            if (!CheckAllChannelsChosen())
                return false;
            _controller.StartDrawing();
            return true;
        }

        public virtual void Stop() 
        {
            //_chart.StopDrawing();
            _controller.StopDrawing();
        }

        public virtual void StopAndMoveToStart()
        {
            _controller.StopDrawingAndMoveToStart();
        }

        public virtual void ChangeWindowPosition(double CurrentX)
        {
            _controller.MoveWindow(CurrentX);
        }

        protected void ResignDataStorageLastUpdateChannel(int PreviousLastSignChannel = -1)
        {
            /*
             * Подписываясь на изменение последнего канала, мы гарантируем, что все предыдещие каналы, которые отображает данный чарт, были обновлены
             * тем самым оптимизируя отрисовку, отоборажая только тогда, когда изменился последний канал
            */
            int lastChannel = _controller.GetLastChannel();

            if (PreviousLastSignChannel != -1)
                _controller.UnsignToChannelUpdation(PreviousLastSignChannel, DataStorageCollectionChangedHandler);
                //_dataStorage.GetChannelData(LastSignChannel).CollectionChanged -= DataStorageCollectionChangedHandler;
            if (lastChannel != -1)
                _controller.SignToChannelUpdation(lastChannel, DataStorageCollectionChangedHandler);
                //_dataStorage.GetChannelData(lastChannel).CollectionChanged += DataStorageCollectionChangedHandler;
        }

        protected void DataStorageCollectionChangedHandler(object sedner, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                //Когда в массив были добавлены новые данные
                case NotifyCollectionChangedAction.Add:
                    UpdateSeries();
                    break;
                //Когда массив был очищен
                case NotifyCollectionChangedAction.Reset:
                    ClearChannels();
                    break;
            }
        }

        public abstract bool CheckAllChannelsChosen();
    }
}

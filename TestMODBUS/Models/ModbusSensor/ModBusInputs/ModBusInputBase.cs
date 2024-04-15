using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using TestMODBUS.Exceptions;
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
            _controller.UpdateChartAfterNewChannelAdded();
        }

        public ModBusInputBase(ModbusSensorController Controller, IEnumerable<int> Channels)
        {
            _controller = Controller;

            if (Channels == null)
                return;

            foreach (int Channel in Channels)
                AddNewChannel(Channel);
            _controller.UpdateChartAfterNewChannelAdded();
        }

        public virtual void DetachFromController()
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
            if(_controller != null )
                _controller.UpdateChart(true);
        }

        public virtual void Start()
        {
            if (!CheckAllChannelsChosen())
                throw new NotAllChannelsChosen();
            _controller.StartDrawing();
        }

        public void DetachFromDataStorage() => _controller.DetachImputModuleFromDataStorage(DataStorageCollectionChangedHandler);

        public void ResignToAllUsingChannels()
        {
            ResignDataStorageLastUpdateChannel();

            _controller.UpdateChartAfterNewChannelAdded();
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

            if (PreviousLastSignChannel >= 0)
                _controller.UnsignToChannelUpdation(PreviousLastSignChannel, DataStorageCollectionChangedHandler);
            if (lastChannel != -1)
                _controller.SignToChannelUpdation(lastChannel, DataStorageCollectionChangedHandler);
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

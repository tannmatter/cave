using Cave.Utils;

namespace Cave.DeviceControllers.Projectors
{
    public abstract class Projector : Device, IDisplayInputSelectable, IDisplayMutable, IAudio
    {
        /* IDisplay */
        public virtual Task DisplayPowerOn() { throw new NotImplementedException(); }
        public virtual Task DisplayPowerOff() { throw new NotImplementedException(); }

        /* IInputSelectable */
        public virtual Task SelectInput( object input ) { throw new NotImplementedException(); }

        /* IDisplayInputSelectable */
        public virtual Task PowerOnSelectInput( object obj ) { throw new NotImplementedException(); }

        /* IDisplayMutable */
        public virtual Task DisplayMute( bool muted ) { throw new NotImplementedException(); }
        public virtual Task<bool> DisplayIsMuted() { throw new NotImplementedException(); }

        /* IAudio */
        public virtual Task AudioVolumeUp() { throw new NotImplementedException(); }
        public virtual Task AudioVolumeDown() { throw new NotImplementedException(); }
        public virtual Task AudioMute(bool muted) { throw new NotImplementedException(); }
        public virtual Task<bool> AudioIsMuted() { throw new NotImplementedException(); }

        /* Projector */
        public virtual Task<object?> GetPowerState() { throw new NotImplementedException(); }
        public virtual Task<object?> GetInputSelection() { throw new NotImplementedException(); }

        protected Projector( string deviceName, string address, int port )
            : base(deviceName)
        {
            this.Address = address;
            this.Port = port;
        }

        public string Address { get; protected set; }
        public int Port { get; protected set; }
        public List<string>? InputsAvailable { get; protected set; }


        /* Device */
        //public override Task Initialize() { throw new NotImplementedException(); }

        /* IObservable<DeviceStatus> implementation */
        //public override IDisposable Subscribe(IObserver<DeviceStatus> observer) { throw new NotImplementedException(); }
    }
}

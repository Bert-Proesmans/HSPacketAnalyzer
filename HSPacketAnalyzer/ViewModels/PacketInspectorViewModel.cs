using MyToolkit.Mvvm;

namespace HSPacketAnalyzer.ViewModels
{
    internal class PacketInspectorViewModel : ViewModelBase
    {
        #region FIELDS
        private string _packetPath;
        #endregion

        public string Title => "In progress";
        public int Width => 800;
        public int Height => 450;

        public void Initialize(string loadFromPath /* CUSTOM DEPENDENCIES HERE */)
        {
            base.Initialize();

            _packetPath = loadFromPath;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
        }

        protected override void OnUnloaded()
        {
            base.OnUnloaded();
        }

        #region PACKET_CONTROL
        protected void AddPacket() { }
        protected void RemovePacket() { }
        protected void GetPacket() { }
        protected void SetPackets() { }
        #endregion

        #region PACKET_INSPECTION
        protected void AnalyzePackets() { }
        #endregion
    }
}

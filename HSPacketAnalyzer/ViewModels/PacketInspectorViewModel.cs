using HSPacketAnalyzer.Helpers;
using MyToolkit.Collections;
using MyToolkit.Mvvm;

namespace HSPacketAnalyzer.ViewModels
{
	internal class PacketInspectorViewModel : ViewModelBase
	{
		#region FIELDS
		private string _packetPath;

		private readonly MtObservableCollection<PacketOverviewViewModel> _packetItems;
		#endregion

		public string Title => "In progress";
		public int Width => 1200;
		public int Height => 650;

		public MtObservableCollection<PacketOverviewViewModel> PacketItems { get { return _packetItems; } }

		public PacketInspectorViewModel()
		{
			_packetItems = new MtObservableCollection<PacketOverviewViewModel>();
		}

		public void Initialize(string loadFromPath /* CUSTOM DEPENDENCIES HERE */)
		{
			base.Initialize();

			_packetPath = loadFromPath;
		}

		protected override void OnLoaded()
		{
			base.OnLoaded();

			SetSamplePackets();
		}

		protected override void OnUnloaded()
		{
			base.OnUnloaded();
		}

		#region PACKET_CONTROL
		protected void SetSamplePackets()
		{
			var samplePackets = new PacketOverviewViewModel[]
			{
				new PacketOverviewViewModel()
				{
					PacketId = GuidGenerator.GenerateSequential(),
					ParentPacketId = null,
					OrdinalIndex = 0,
					TypeName = "A",
					Comment = "NA - A",
				},
				new PacketOverviewViewModel()
				{
					PacketId = GuidGenerator.GenerateSequential(),
					ParentPacketId = null,
					OrdinalIndex = 1,
					TypeName = "B",
					Comment = "NA - B",
				}
			};

			_packetItems.Clear();
			_packetItems.AddRange(samplePackets);

			//foreach (PacketOverviewViewModel item in samplePackets)
			//{
			//	_packetItems.Add(item);
			//}
		}

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

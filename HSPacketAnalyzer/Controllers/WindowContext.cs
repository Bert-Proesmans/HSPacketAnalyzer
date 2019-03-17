using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Foundation.Collections;
using HSPacketAnalyzer.Services;
using PacketModels;
using PacketModels.Packets;
using PacketModels.Packets.BNET;
using PacketModels.Packets.Pegasus;

namespace HSPacketAnalyzer.Controllers
{
	internal class WindowContext : IContext
	{
		#region Private Fields

		private string _packetPath;
		private readonly ExtendedObservableCollection<Payload> _payloads;
		private readonly ExtendedObservableCollection<PacketBase> _packets;

		#endregion

		#region Public Constructors

		private WindowContext()
		{
			_payloads = new ExtendedObservableCollection<Payload>();
			_packets = new ExtendedObservableCollection<PacketBase>();
		}

		#endregion

		#region Public Properties

		public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public string SupportedVersion => throw new NotImplementedException();

		public string CurrentVersion => throw new NotImplementedException();

		public int PayloadCount => throw new NotImplementedException();

		public ObservableCollection<PacketBase> Packets => _packets;

		#endregion

		#region Public Methods

		public static WindowContext Create() => new WindowContext();

		public Task Initialise(string filePath)
		{
			_packetPath = filePath;

			SetSamplePackets();

			return null;
		}

		public virtual void AddPacket() { }
		// public virtual void RemovePacket() { }
		// public virtual void GetPacket() { }
		public virtual void SetPackets() { }
		public virtual void AnalyzePackets() { }

		#endregion

		#region Protected Methods

		protected void SetSamplePackets()
		{
			var samplePackets = new PacketBase[] {
				new BNetPacket(null, false, new RPCHeader(-1, -1, -1), null),
				// new PegPacket(null, false, -1, null),
			};

			_packets.Clear();
			_packets.AddRange(samplePackets);
		}

		#endregion

		#region Private Methods

		#endregion
	}
}

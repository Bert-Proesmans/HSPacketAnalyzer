using System;
using System.Collections.Generic;
using System.Diagnostics;
using Foundation.Collections;
using PacketModels.Packets;

namespace PacketModels
{
	internal class Payload
	{
		public PayloadDirection Direction { get; }
		public PayloadStream StreamKind { get; }
		public DateTime TransmissionDate { get; }
		public uint BodyTypeHash { get; }
		public byte[] Body { get; }

		private readonly ExtendedObservableCollection<PacketBase> _payloadPackets;
		public IReadOnlyCollection<PacketBase> PayloadPackets => _payloadPackets;

		public Payload(PayloadDirection direction, PayloadStream streamKind, DateTime transmissionDate, uint bodyTypeHash, byte[] body)
		{
			_payloadPackets = new ExtendedObservableCollection<PacketBase>();

			Direction = direction;
			StreamKind = streamKind;
			TransmissionDate = transmissionDate;
			BodyTypeHash = bodyTypeHash;
			Body = body;
		}

		public void MarkUnrecognized()
		{
			var unrecognizedPacket = new UnrecognizedPacket(Body);
			_payloadPackets.Clear();
			_payloadPackets.Add(unrecognizedPacket);
		}

		public void ClearPackets()
		{
			_payloadPackets.Clear();
		}

		public void AddPacket(PacketBase packet)
		{
			Debug.Assert(packet != null);
			_payloadPackets.Add(packet);
		}
	}
}

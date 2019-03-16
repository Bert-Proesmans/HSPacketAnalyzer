namespace PacketModels.Packets.BNET
{
	internal class BNetPacket : PacketBase
	{
		public RPCHeader Header { get; private set; }

		public BNetPacket(byte[] serializedObject, bool hidden, RPCHeader header, object decodedObject) : 
			base(PacketType.BattleNet, serializedObject, hidden, decodedObject)
		{
			Header = header;
		}
	}
}

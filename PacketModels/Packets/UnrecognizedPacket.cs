namespace PacketModels.Packets
{
	internal class UnrecognizedPacket : PacketBase
	{
		public UnrecognizedPacket(byte[] serializedObject) : base(PacketType.Unknown, serializedObject, false, null)
		{
		}
	}
}

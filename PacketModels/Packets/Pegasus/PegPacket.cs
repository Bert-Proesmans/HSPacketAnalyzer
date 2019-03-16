namespace PacketModels.Packets.Pegasus
{
	internal class PegPacket : PacketBase
	{
		public int ObjectType { get; private set; }

		public PegPacket(byte[] serializedObject, bool hidden, int objectType, object decodedObject) : 
			base(PacketType.Pegasus, serializedObject, hidden, decodedObject)
		{
			ObjectType = objectType;
		}
	}
}

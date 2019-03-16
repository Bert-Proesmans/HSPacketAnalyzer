namespace PacketModels.Packets
{
	public abstract class PacketBase
    {
        public PacketType Type { get; private set; }

        public dynamic DecodedObject { get; private set; }

        public byte[] SerializedObject { get; private set; }

        public bool IsHidden { get; private set; }

        protected PacketBase(PacketType type, byte[] serializedObject, bool hidden, dynamic decodedObject)
        {
            Type = type;
            DecodedObject = decodedObject;
            SerializedObject = serializedObject;
            IsHidden = hidden;
        }
    }
}

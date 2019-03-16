namespace PacketModels.Packets.BNET
{
	public class RPCHeader
    {
        public int ServiceId { get; private set; }
        public int MethodId { get; private set; }
        public int Token { get; private set; }

        public RPCHeader() : this(-1, -1, -1) { }

        public RPCHeader(int serviceId, int methodId, int token)
        {
            ServiceId = serviceId;
            MethodId = methodId;
            Token = token;
        }
    }
}

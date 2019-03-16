using System;
using System.Collections.Generic;
using HSPacketAnalyzer.Services;

namespace HSPacketAnalyzer.Controllers
{
	internal class WindowContext : IContext
	{
		private WindowContext() { }

		public string Name => throw new NotImplementedException();

		public string SupportedVersion => throw new NotImplementedException();

		public string CurrentVersion => throw new NotImplementedException();

		public int PacketCount => throw new NotImplementedException();

		public IReadOnlyCollection<bool> Packets => throw new NotImplementedException();

		public static WindowContext Create()
		{
			return null;
		}
	}
}

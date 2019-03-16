using System.Collections.Generic;

namespace HSPacketAnalyzer.Services
{
	internal interface IContext
	{
		string Name { get; }

		string SupportedVersion { get; }

		string CurrentVersion { get; }

		int PacketCount { get; }

		IReadOnlyCollection<bool> Packets { get; } // TODO;
	}
}

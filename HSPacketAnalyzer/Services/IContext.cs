using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PacketModels.Packets;

namespace HSPacketAnalyzer.Services
{
	internal interface IContext: IDisposable
	{
		string Name { get; set; }

		string SupportedVersion { get; }

		string CurrentVersion { get; }

		int PayloadCount { get; }

		string LibPath { get; set; }

		ObservableCollection<PacketBase> Packets { get; }

		Task Initialise(string filePath);

		Task<bool> RebuildAnalyzerAsync();
	}
}

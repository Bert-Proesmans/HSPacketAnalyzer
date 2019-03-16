using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PacketModels.Packets;

namespace HSPacketAnalyzer.Services
{
	internal interface IContext
	{
		string Name { get; set; }

		string SupportedVersion { get; }

		string CurrentVersion { get; }

		int PayloadCount { get; }

		ObservableCollection<PacketBase> Packets { get; }

		Task Initialise(string filePath);
	}
}

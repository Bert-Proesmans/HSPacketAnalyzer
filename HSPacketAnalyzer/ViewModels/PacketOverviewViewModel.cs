using System;
using MyToolkit.Model;

namespace HSPacketAnalyzer.ViewModels
{
	internal class PacketOverviewViewModel : GraphObservableObject
	{
		public Guid PacketId { get; set; }

		public Guid? ParentPacketId { get; set; }

		public int OrdinalIndex { get; set; }

		public string TypeName { get; set; }

		public string Comment { get; set; }

		public bool ValidDecoded { get; set; }
	}
}

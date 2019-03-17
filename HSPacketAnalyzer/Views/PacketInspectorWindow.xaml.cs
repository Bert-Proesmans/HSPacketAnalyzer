using System.Windows;
using HSPacketAnalyzer.ViewModels;

namespace HSPacketAnalyzer.Views
{
	/// <summary>
	/// Interaction logic for PacketInspectorWindow.xaml
	/// </summary>
	public partial class PacketInspectorWindow : Window
	{
		internal PacketInspectorViewModel ViewModel => (PacketInspectorViewModel)DataContext;

		public PacketInspectorWindow(string loadFromPath)
		{
			InitializeComponent();

			ViewModel.Initialize(loadFromPath);
			Loaded += delegate { ViewModel.CallOnLoaded(); };
			Unloaded += delegate { ViewModel.CallOnUnloaded(); };
		}
	}
}

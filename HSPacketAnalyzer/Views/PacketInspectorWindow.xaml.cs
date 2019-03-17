using System.IO;
using System.Windows;
using HSPacketAnalyzer.ViewModels;
using HSPacketAnalyzer.Views.Modals;

namespace HSPacketAnalyzer.Views
{
	/// <summary>
	/// Interaction logic for PacketInspectorWindow.xaml
	/// </summary>
	public partial class PacketInspectorWindow : Window
	{
		#region Public Constructors

		public PacketInspectorWindow(string loadFromPath)
		{
			InitializeComponent();

			ViewModel.Initialize(loadFromPath);
			Loaded += delegate { ViewModel.CallOnLoaded(); };
			Unloaded += delegate { ViewModel.CallOnUnloaded(); };

			ViewModel.PropertyChanged += ViewModel_PropertyChanged;
		}

		#endregion

		#region Public Properties

		internal PacketInspectorViewModel ViewModel => (PacketInspectorViewModel)DataContext;

		#endregion

		#region Private Methods

		#region Event Handlers

		private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(PacketInspectorViewModel.LibPath):
					{
						if (string.IsNullOrEmpty(ViewModel.LibPath) || !Directory.Exists(ViewModel.LibPath))
						{
							var dialog = new PickLibraryPathDialog()
							{
								Owner = this,
							};

							dialog.ShowDialog();

							if (dialog.DialogResult == true)
							{
								string path = dialog.ViewModel.LibPath;
								bool store = dialog.ViewModel.SetAsDefault;
								ViewModel.UpdateLibPath(path, store);
							}
							else
							{
								Close();
							}
						}
					}
					break;
			}
		}

		#endregion

		#endregion
	}
}

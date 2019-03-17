using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HSPacketAnalyzer.ViewModels.Dialogs;
using Microsoft.Win32;

namespace HSPacketAnalyzer.Views.Modals
{
	/// <summary>
	/// Interaction logic for PickLibraryPathDialog.xaml
	/// </summary>
	public partial class PickLibraryPathDialog : Window
	{
		public PickLibraryPathDialog()
		{
			InitializeComponent();
		}

		internal PickLibraryPathViewModel ViewModel
		{
			get => (PickLibraryPathViewModel)DataContext;
			set => DataContext = value;
		}

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			if(!IsValid(this)) { return; }

			DialogResult = true;
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}

		private void BrowseButton_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new System.Windows.Forms.FolderBrowserDialog()
			{
				Description = "In Progress",
				ShowNewFolderButton = false,
			};
			/*
			var dialog = new OpenFileDialog()
			{
				Filter = "Folders|\n",
				AddExtension = false,
				CheckFileExists = false,
				DereferenceLinks = true,
				Multiselect = false,
			};
			*/

			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				libPathTextBox.Text = dialog.SelectedPath;
			}
		}

		// Validate all dependency objects in a window
		private bool IsValid(DependencyObject node)
		{
			// Check if dependency object was passed
			if (node != null)
			{
				// Check if dependency object is valid.
				// NOTE: Validation.GetHasError works for controls that have validation rules attached 
				bool isValid = !Validation.GetHasError(node);
				if (!isValid)
				{
					// If the dependency object is invalid, and it can receive the focus,
					// set the focus
					if (node is IInputElement)
					{
						Keyboard.Focus((IInputElement)node);
					}

					return false;
				}
			}

			// If this dependency object is valid, check all child dependency objects
			return LogicalTreeHelper.GetChildren(node).OfType<DependencyObject>().All(IsValid);

			// All dependency objects are valid
		}
	}
}

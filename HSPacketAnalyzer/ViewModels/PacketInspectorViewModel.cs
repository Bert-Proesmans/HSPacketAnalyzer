using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using Foundation.Collections;
using HSPacketAnalyzer.Helpers;
using HSPacketAnalyzer.Services;
using MyToolkit.Command;
using MyToolkit.Composition;
using PacketModels.Packets;
using Serilog;

namespace HSPacketAnalyzer.ViewModels
{
	internal class PacketInspectorViewModel : MyToolkit.Mvvm.ViewModelBase
	{
		#region Private FIELDS

		private readonly ExtendedObservableCollection<PacketOverviewViewModel> _packetItems;
		private readonly IContext _context;

		#endregion


		#region Public Constructors

		public PacketInspectorViewModel() : this(ServiceLocator.Default.Resolve<IContextRepository>()) { }

		public PacketInspectorViewModel(IContextRepository ctxtFactory)
		{
			_context = ctxtFactory.NewContext();

			_packetItems = new ExtendedObservableCollection<PacketOverviewViewModel>();
			PacketView = new ReadOnlyObservableCollectionView<PacketOverviewViewModel, int>(_packetItems, true);

			_context.Packets.CollectionChanged += Packets_CollectionChanged;

			DebugCommand = new RelayCommand(InsertPacket);
		}

		private void InsertPacket()
		{
			_packetItems.Add(TransformPacketModel(null));
		}

		#endregion

		#region Public Properties

		public string Title => "In progress";
		public int Width => 1200;
		public int Height => 650;

        public ReadOnlyObservableCollectionView<PacketOverviewViewModel, int> PacketView { get; }

        public ICommand DebugCommand { get; }

        #endregion


        #region Public Methods

        public void Initialize(string loadFromPath /* CUSTOM DEPENDENCIES HERE */)
		{
			_context.Initialise(loadFromPath);
		}

		public override void HandleException(Exception exception)
		{
			Log.Error(exception, "Unhandled exception during async task!");
		}

		#endregion

		#region Protected Methods

		protected override void OnLoaded()
		{
		}

		protected override void OnUnloaded()
		{
		}

		#endregion

		#region Private Methods

		private PacketOverviewViewModel TransformPacketModel(PacketBase packet)
		{
			// TODO; Proper implementation!
			return new PacketOverviewViewModel()
			{
				PacketId = GuidGenerator.GenerateSequential(),
				ParentPacketId = null,
				OrdinalIndex = 0,
				TypeName = "A",
				Comment = "NA - A",
			};
		}

		private void Packets_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					{
						int startIndex = e.NewStartingIndex == -1 ? _packetItems.Count : e.NewStartingIndex;
						IEnumerable<PacketOverviewViewModel> transformedItems = e.NewItems.Cast<PacketBase>().Select(TransformPacketModel);
						_packetItems.InsertRange(startIndex, transformedItems);
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					{
						if (e.NewStartingIndex == -1)
						{
							IEnumerable<PacketOverviewViewModel> oldItems = e.OldItems.Cast<PacketBase>().Select(TransformPacketModel);
							_packetItems.RemoveRange(oldItems);
							break;
						}
						_packetItems.RemoveAtRange(e.NewStartingIndex, e.OldItems.Count);
					}
					break;
				case NotifyCollectionChangedAction.Replace:
					{
						if (e.NewStartingIndex == -1)
						{
							throw new NotImplementedException();
							// break;
						}
						int startIndex = e.NewStartingIndex;
						IEnumerable<PacketOverviewViewModel> transformedItems = e.NewItems.Cast<PacketBase>().Select(TransformPacketModel);
						_packetItems.ReplaceRange(startIndex, transformedItems);
					}
					break;
				case NotifyCollectionChangedAction.Move:
					{
						_packetItems.MoveRange(e.OldStartingIndex, e.NewStartingIndex, e.NewItems.Count);
					}
					break;
				case NotifyCollectionChangedAction.Reset:
					{
						_packetItems.Clear();
					}
					break;

			}
		}

		#endregion
	}
}

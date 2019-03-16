using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Foundation.Collections
{
	internal class ObservableCollectionView<T, SortKey> : ReadOnlyObservableCollectionView<T, SortKey>
		where SortKey : IComparable<SortKey>
	{
		#region Public Constructors

		public ObservableCollectionView() : this(new ExtendedObservableCollection<T>(), true)
		{
		}

		public ObservableCollectionView(IList<T> items, bool isTracking) : base(items, isTracking)
		{
		}

		public ObservableCollectionView(ObservableCollection<T> collection, bool isTracking) : base(collection, isTracking)
		{
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// The collection that holds all original items.
		/// </summary>
		public ObservableCollection<T> Source => _itemCollection;

		#endregion
	}
}

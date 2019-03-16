using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Foundation.Collections
{
	// NOTE; Maybe integrate Item (T) Property tracking?
	internal class ReadOnlyObservableCollectionView<T, SortKey> : ICollectionView<T, SortKey>,
		IEnumerable<T>, IReadOnlyList<T>, IReadOnlyCollection<T>, IDisposable,
		INotifyCollectionChanged, INotifyPropertyChanged
		where SortKey : IComparable<SortKey>
	{
		#region Protected Fields

		protected ObservableCollection<T> _itemCollection;
		protected List<T> _filteredCollection;

		private bool _isTracking;
		private int? _limit;
		private int? _offset;
		private Func<T, bool> _filter;
		private bool _ascending;
		private Func<T, SortKey> _order;

		#endregion

		#region Public Constructors

		public ReadOnlyObservableCollectionView(IList<T> items, bool isTracking) : this(new ObservableCollection<T>(items), isTracking) { }

		public ReadOnlyObservableCollectionView(ObservableCollection<T> collection, bool isTracking)
		{
			SyncRoot = new object();
			_itemCollection = collection;
			_filteredCollection = new List<T>();
			_isTracking = isTracking;

			// WARN; Do not forget to dispose the handlers!
			_itemCollection.CollectionChanged += OnItemCollectionChanged;
			((INotifyPropertyChanged)_itemCollection).PropertyChanged += OnItemCollectionPropertyChanged;
		}

		#endregion

		#region Public Events

		#region INotifyCollectionChanged Implementation
		event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
		{
			add { CollectionChanged += value; }
			remove { CollectionChanged -= value; }
		}
		#endregion

		#region INotifyPropertyChanged Implementation
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add { PropertyChanged += value; }
			remove { PropertyChanged -= value; }
		}
		#endregion

		public virtual event NotifyCollectionChangedEventHandler CollectionChanged;

		public virtual event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Protected Events

		#endregion

		#region Public Properties

		public IReadOnlyList<T> Items => _filteredCollection;

		public int Count
		{
			get
			{
				lock (SyncRoot)
				{
					return _filteredCollection.Count;
				}
			}
		}

		public object SyncRoot { get; }

		public bool IsSynchronized => true;

		public T this[int index]
		{
			get
			{
				lock (SyncRoot) { return _filteredCollection[index]; }
			}
		}

		public bool IsTracking
		{
			get => _isTracking;
			set
			{
				if (_isTracking != value)
				{
					_isTracking = value;
					RaisePropertyChanged();
				}

				if (_isTracking)
				{
					Refresh();
				}
			}
		}
		public int? Limit
		{
			get => _limit;
			set
			{
				if (_limit != value)
				{
					_limit = value;
					RaisePropertyChanged();
					Refresh();
				}
			}
		}
		public int? Offset
		{
			get => _offset;
			set
			{
				if (_offset != value)
				{
					_offset = value;
					RaisePropertyChanged();
					Refresh();
				}
			}
		}
		public bool Ascending
		{
			get => _ascending;
			set
			{
				if (_ascending != value)
				{
					_ascending = value;
					RaisePropertyChanged();
					Refresh();
				}
			}
		}

		public Func<T, bool> Filter
		{
			get => _filter;
			set
			{
				if (_filter != value)
				{
					_filter = value;
					RaisePropertyChanged();
					Refresh();
				}
			}
		}

		public Func<T, SortKey> Order
		{
			get => _order;
			set
			{
				if (_order != value)
				{
					_order = value;
					RaisePropertyChanged();
					Refresh();
				}
			}
		}

		#endregion

		#region Public Methods

		public IEnumerator<T> GetEnumerator()
		{
			lock (SyncRoot)
			{
				return _filteredCollection.GetEnumerator();
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			lock (SyncRoot)
			{
				return _filteredCollection.GetEnumerator();
			}
		}

		public void CopyTo(Array array, int index)
		{
			lock (SyncRoot)
			{
				((ICollection)_filteredCollection).CopyTo(array, index);
			}
		}

		public void Refresh()
		{
			if (!IsTracking)
			{
				return;
			}

			lock (SyncRoot)
			{
				ProcessItems();
			}
		}

		public void Dispose()
		{
			_itemCollection.CollectionChanged -= OnItemCollectionChanged;
			((INotifyPropertyChanged)_itemCollection).PropertyChanged -= OnItemCollectionPropertyChanged;
		}

		#endregion

		#region Protected Methods

		protected virtual void ProcessItems()
		{
			// TODO; Make more robust by handling null and edge cases!
			IEnumerable<T> source = _itemCollection;
			if (_filter != null)
			{
				source = source.Where(_filter);
			}

			if (_order != null)
			{
				source = source.OrderBy(_order);
			}

			if (_offset != null)
			{
				source = source.Skip(_offset.Value);
			}

			if (_limit != null)
			{
				source = source.Take(_limit.Value);
			}

			var filtered = source.ToList();
			// TODO; Smarter insertion algorithm!
			_filteredCollection.Clear();
			_filteredCollection.InsertRange(0, filtered);

			RaisePropertyChanged(nameof(Count));
			RaisePropertyChanged(nameof(Items));
			OnCollectionReset();
			OnCollectionRangeAdded(filtered, 0);
		}

		#endregion

		#region Private Methods

		private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
		{
			CollectionChanged?.Invoke(this, args);
		}

		private void OnCollectionReset()
		{
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		private void OnCollectionRangeAdded(IList<T> newItems, int startingIndex)
		{
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItems, startingIndex));
		}

		private void OnItemCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			// TODO; Optimize refresh depending on event arguments.
			Refresh();
		}

		private void OnItemCollectionPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			// NOP?
			Refresh();
		}

		#endregion
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Foundation.Collections
{
	[Serializable]
	[DebuggerDisplay("Count = {Count}")]
	// Observablecollection reference: 
	// https://github.com/Microsoft/referencesource/blob/60a4f8b853f60a424e36c7bf60f9b5b5f1973ed1/System/compmod/system/collections/objectmodel/observablecollection.cs
	internal sealed class ExtendedObservableCollection<T> : ObservableCollection<T>
	{
		#region Private Fields

		// nameof() trick doesn't work for object indexing properties!
		// We need a correct value to raise a change event on so UI controls can update
		// their internal list.
		private const string WPFIndexerName = "Item[]";

		#endregion

		#region Public Constructors

		public ExtendedObservableCollection() : base() { }
		public ExtendedObservableCollection(List<T> list) : base(list) { }
		public ExtendedObservableCollection(IEnumerable<T> collection) : base(collection) { }

		#endregion

		#region Public Methods

		public void AddRange(IEnumerable<T> collection) => InsertRange(Items.Count, collection);

		public void InsertRange(int insertionIndex, IEnumerable<T> collection)
		{
			CheckReentrancy();

			if (collection == null)
			{
				throw new ArgumentNullException(nameof(collection));
			}

			if (Items.IsReadOnly)
			{
				throw new NotSupportedException("Collection source is read-only");
			}

			if (Items is List<T> list)
			{
				list.InsertRange(insertionIndex, collection);
			}
			else
			{
				foreach (T item in collection)
				{
					Items.Add(item);
				}
			}

			OnPropertyChanged(nameof(Count));
			OnPropertyChanged(WPFIndexerName);
			OnCollectionRangeAdded(insertionIndex, MakeList(collection));
		}

		public void RemoveRange(IEnumerable<T> collection)
		{
			CheckReentrancy();

			if (collection == null)
			{
				throw new ArgumentNullException(nameof(collection));
			}

			if (Items.IsReadOnly)
			{
				throw new NotSupportedException("Collection source is read-only");
			}

			bool dirty = false;

			// TODO; More performant algorithm to remove grouped items from the list
			// in one iteration of Items.
			foreach (T item in collection)
			{
				Items.Remove(item);
				dirty = true;
			}

			if (dirty)
			{
				OnPropertyChanged(nameof(Count));
				OnPropertyChanged(WPFIndexerName);
				OnCollectionRangeRemoved(-1, MakeList(collection));
			}
		}

		public void RemoveAtRange(int removalIndex, int count)
		{
			CheckReentrancy();

			if (Items.IsReadOnly)
			{
				throw new NotSupportedException("Collection source is read-only");
			}

			if (removalIndex < 0 || removalIndex > (Items.Count - count))
			{
				throw new ArgumentOutOfRangeException(nameof(removalIndex));
			}

			if (count < 0 || count > (Items.Count - removalIndex))
			{
				throw new ArgumentOutOfRangeException(nameof(count));
			}

			var oldItems = new List<T>();
			for (int i = 0; i < count; ++i)
			{
				int index = removalIndex + i;
				T oldItem = Items[index];
				oldItems.Add(oldItem);
			}

			if (Items is List<T> list)
			{
				list.RemoveRange(removalIndex, count);
			}
			else
			{
				for (int i = 0; i < count; ++i)
				{
					int index = removalIndex;
					Items.RemoveAt(index);
				}
			}

			OnPropertyChanged(nameof(Count));
			OnPropertyChanged(WPFIndexerName);
			OnCollectionRangeRemoved(removalIndex, oldItems);
		}

		/// <summary>
		/// Moves a range of elements from the old index to the new index.
		/// </summary>
		/// <remarks>
		/// <para>This operation is logically seen as a remove followed by an add.
		/// This behaviour is parallel to the expectations attached to 
		/// <see cref="NotifyCollectionChangedEventArgs"/>
		/// </para>
		/// <para>
		/// The newStartingIndex represents the index in the intermidate list state; where
		/// the original items have been removed already.
		/// </para>
		/// </remarks>
		/// <param name="oldStartingIndex">The index from where items are removed</param>
		/// <param name="newStartingIndex">The index where items are re-inserted</param>
		/// <param name="count">The amount of items to move</param>
		public void MoveRange(int oldStartingIndex, int newStartingIndex, int count)
		{
			CheckReentrancy();

			if (Items.IsReadOnly)
			{
				throw new NotSupportedException("Collection source is read-only");
			}

			if (count < 0 || count > (Items.Count - oldStartingIndex))
			{
				throw new ArgumentOutOfRangeException(nameof(count));
			}

			if (oldStartingIndex < 0 || oldStartingIndex > Items.Count)
			{
				throw new ArgumentOutOfRangeException(nameof(oldStartingIndex));
			}

			if (newStartingIndex < 0 || newStartingIndex > (Items.Count - count))
			{
				throw new ArgumentOutOfRangeException(nameof(newStartingIndex));
			}

			if (count == 0)
			{
				return;
			}

			var oldItems = new List<T>();
			for (int i = 0; i < count; ++i)
			{
				int index = oldStartingIndex;
				T oldItem = Items[index];
				oldItems.Add(oldItem);
				Items.RemoveAt(index);
			}

			if (Items is List<T> list)
			{
				list.InsertRange(newStartingIndex, oldItems);
			}
			else
			{
				for (int i = 0; i < count; ++i)
				{
					int index = newStartingIndex + i;
					T item = oldItems[i];
					Items.Insert(index, item);
				}
			}

			OnPropertyChanged(WPFIndexerName);
			OnCollectionMoved(newStartingIndex, oldStartingIndex, oldItems);
		}

		public void ReplaceRange(int startingIndex, IEnumerable<T> collection)
		{
			CheckReentrancy();

			if (collection == null)
			{
				throw new ArgumentNullException(nameof(collection));
			}

			if (Items.IsReadOnly)
			{
				throw new NotSupportedException("Collection source is read-only");
			}

			IList<T> newItems = MakeList(collection);
			int sourceItemCount = newItems.Count;
			if (startingIndex < 0 || startingIndex > (Items.Count - sourceItemCount))
			{
				throw new ArgumentOutOfRangeException(nameof(startingIndex));
			}


			var replaced = new List<T>();
			for (int i = 0; i < sourceItemCount; ++i)
			{
				int index = startingIndex + i;
				T oldItem = Items[index];
				replaced.Add(oldItem);
			}

			if (Items is List<T> list)
			{
				list.RemoveRange(startingIndex, sourceItemCount);
				list.InsertRange(startingIndex, newItems);
			}
			else
			{
				for (int i = 0; i < sourceItemCount; ++i)
				{
					int index = startingIndex + i;
					T item = newItems[i];
					Items.Insert(index, item);
					Items.RemoveAt((index + 1));
				}
			}

			OnPropertyChanged(WPFIndexerName);
			OnCollectionReplaced(startingIndex, newItems, replaced);
		}

		public void ReInitialize(IEnumerable<T> collection)
		{
			CheckReentrancy();

			if (collection == null)
			{
				throw new ArgumentNullException(nameof(collection));
			}

			var preItemCopy = new List<T>(Items);
			Items.Clear();

			if (Items is List<T> list)
			{
				list.InsertRange(0, collection);
			}
			else
			{
				foreach (T item in collection)
				{
					Items.Add(item);
				}
			}

			OnPropertyChanged(nameof(Count));
			OnPropertyChanged(WPFIndexerName);
			OncollectionReInitialized(preItemCopy, MakeList(collection));
		}

		#endregion

		#region Private Methods

		private IList<T> MakeList(IEnumerable<T> collection)
		{
			if (collection is IList<T> list)
			{
				return list;
			}
			else
			{
				return new List<T>(collection);
			}
		}

		private void OnPropertyChanged(string propertyName)
		{
			OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
		}

		private void OnCollectionRangeAdded(int startingIndex, IList<T> newItems)
		{
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (IList)newItems, startingIndex));
		}

		private void OnCollectionRangeRemoved(int index, IList<T> oldItems)
		{
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, (IList)oldItems, index));
		}

		private void OnCollectionReplaced(int startingIndex, IList<T> newItems, IList<T> oldItems)
		{
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, (IList)newItems, (IList)oldItems, startingIndex));
		}

		private void OnCollectionMoved(int newStartingIndex, int oldStartingIndex, IList<T> items)
		{
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, (IList)items, newStartingIndex, oldStartingIndex));
		}


		private void OncollectionReInitialized(IList<T> oldItems, IList<T> newItems)
		{
			int oldCount = oldItems.Count;
			int newCount = newItems.Count;

			if (oldCount > 0 && newCount > 0)
			{
				int replacedItemCount = Math.Min(oldCount, newCount);
				var eventArgs = new NotifyCollectionChangedEventArgs(
					NotifyCollectionChangedAction.Replace,
					newItems.Take(replacedItemCount).ToList(),
					oldItems.Take(replacedItemCount).ToList(),
					0);
				OnCollectionChanged(eventArgs);
			}

			if (oldCount < newCount)
			{
				int insertionIndex = oldCount;
				var eventArgs = new NotifyCollectionChangedEventArgs(
					NotifyCollectionChangedAction.Add,
					newItems.Skip(insertionIndex).ToList(),
					insertionIndex);
				OnCollectionChanged(eventArgs);
			}
			else
			{ // oldCount > newCount
				int removedIndex = newCount;
				var eventArgs = new NotifyCollectionChangedEventArgs(
					NotifyCollectionChangedAction.Remove,
					oldItems.Skip(removedIndex).ToList(),
					removedIndex);
				OnCollectionChanged(eventArgs);
			}
		}

		#endregion
	}
}

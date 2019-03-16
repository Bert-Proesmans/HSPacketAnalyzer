using System;
using System.Collections;
using System.Collections.ObjectModel;

namespace Foundation.Collections
{
	/// <summary>
	/// Provides a view into a collection object, like <see cref="ObservableCollection{T}"/>.
	/// </summary>
	internal interface ICollectionView<T, Key>: ICollection
		where Key: IComparable<Key>
	{
		/// <summary>Gets or sets a value indicating whether the view should automatically be updated when needed. 
		/// Disable this flag when doing multiple of operations on the underlying collection. 
		/// Enabling this flag automatically updates the view if needed. </summary>
		bool IsTracking { get; set; }

		/// <summary>Gets or sets the maximum number of items in the view. </summary>
		int? Limit { get; set; }

		/// <summary>Gets or sets the offset from where the results a selected. </summary>
		int? Offset { get; set; }

		/// <summary>Gets or sets a value indicating whether to sort ascending or descending. </summary>
		bool Ascending { get; set; }

		/// <summary>Gets or sets the filter (a Func{TItem, bool} object). </summary>
		Func<T, bool> Filter { get; set; }

		/// <summary>Gets or sets the order (a Func{TItem, object} object). </summary>
		Func<T, Key> Order { get; set; }

		/// <summary>Refreshes the view. </summary>
		void Refresh();
	}
}

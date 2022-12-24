#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace System.Collections.Generic
{
    /// <summary>
    /// Provides a generic collection that supports data binding and additionally supports sorting.
    /// See http://msdn.microsoft.com/en-us/library/ms993236.aspx
    /// If the elements are IComparable it uses that; otherwise compares the ToString()
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    public class SortableBindingList<T> : BindingList<T>
    {
        private bool _isSorted;
        private ListSortDirection _sortDirection = ListSortDirection.Ascending;
        private PropertyDescriptor _sortProperty;

        /// <summary>
        /// Gets a value indicating whether the list supports sorting.
        /// </summary>
        protected override bool SupportsSortingCore
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the list is sorted.
        /// </summary>
        protected override bool IsSortedCore
        {
            get { return _isSorted; }
        }

        /// <summary>
        /// Gets the direction the list is sorted.
        /// </summary>
        protected override ListSortDirection SortDirectionCore
        {
            get { return _sortDirection; }
        }

        /// <summary>
        /// Gets the property descriptor that is used for sorting the list if sorting is implemented in a derived class; otherwise, returns null
        /// </summary>
        protected override PropertyDescriptor SortPropertyCore
        {
            get { return _sortProperty; }
        }

        /// <summary>
        /// Removes any sort applied with ApplySortCore if sorting is implemented
        /// </summary>
        protected override void RemoveSortCore()
        {
            _sortDirection = ListSortDirection.Ascending;
            _sortProperty = null;
        }

        /// <summary>
        /// Sorts the items if overridden in a derived class
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="direction"></param>
        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            _sortProperty = prop;
            _sortDirection = direction;

            if (_sortProperty == null) return; //nothing to sort on
            List<T> list = Items as List<T>;
            if (list == null) return;

            list.Sort(delegate(T lhs, T rhs)
            {
                object lhsValue = lhs == null ? null : _sortProperty.GetValue(lhs);
                object rhsValue = rhs == null ? null : _sortProperty.GetValue(rhs);
                int result = 0;
                if (lhsValue == null)
                    result = -1;
                else if (rhsValue == null)
                    result = 1;
                else
                {
                    if (lhsValue is IComparable)
                        result = ((IComparable)lhsValue).CompareTo(rhsValue);
                    else if (!lhsValue.Equals(rhsValue))//not comparable, compare ToString
                        result = lhsValue.ToString().CompareTo(rhsValue.ToString());
                }
                if (_sortDirection == ListSortDirection.Descending)
                    result = -result;
                return result;
            });

            _isSorted = true;
            //fire an event that the list has been changed.
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }
    }
}
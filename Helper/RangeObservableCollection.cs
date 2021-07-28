using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Helpers
{
    public class ObservableCollection<T> : System.Collections.ObjectModel.ObservableCollection<T>
    {
        private bool _suppressNotification = false;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!_suppressNotification)
            {
                base.OnCollectionChanged(e);
            }
        }

        public void AddRange(IEnumerable<T> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            _suppressNotification = true;

            foreach (var item in list)
            {
                Add(item);
            }
            _suppressNotification = false;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}

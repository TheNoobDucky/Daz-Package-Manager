using System;
using System.Collections;
using System.Windows.Data;

namespace Helpers
{
    public class StringCompareHelper : IComparer
    {
        private static StringComparer comparer = StringComparer.Ordinal;

        public int Compare(object x, object y)
        {
            if (x is CollectionViewGroup xg && y is CollectionViewGroup yg)
            {
                if (xg.Name == null )
                {
                    return 1;
                }
                if (yg.Name == null)
                {
                    return -1;
                }
                if (xg.Name is string xs && yg.Name is string ys)
                {
                    // higher group number have lower priority
                    return comparer.Compare(xs, ys);
                }
            }
            throw new NotImplementedException();
        }
    }
}

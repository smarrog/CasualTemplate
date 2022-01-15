using System;
using System.Collections.Generic;

namespace MessagePackTools {
    public class DisplayTypeComparer : IComparer<IDisplayType> {
        public static DisplayTypeComparer Instance { get; private set; } = new();

        public int Compare(IDisplayType x, IDisplayType y) {
            if (ReferenceEquals(x, y)) {
                return 0;
            }

            if (ReferenceEquals(null, y)) {
                return 1;
            }

            if (ReferenceEquals(null, x)) {
                return -1;
            }

            return string.Compare(x.DisplayString, y.DisplayString, StringComparison.OrdinalIgnoreCase);
        }
    }
}
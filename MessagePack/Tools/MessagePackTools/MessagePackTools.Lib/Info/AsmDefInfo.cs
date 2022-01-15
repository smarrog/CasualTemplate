using System;
using System.Collections.Generic;

namespace MessagePackTools {
    public class AsmDefInfo {
        public string Path;
        public HashSet<FileInfo> FileInfos = new();
        public SortedSet<string> AsmDefReferenceNames = new(StringComparer.OrdinalIgnoreCase);
        public SortedSet<TypeInfo> Types = new(DisplayTypeComparer.Instance);
        public SortedSet<TypeReferenceInfo> RefTypes = new(DisplayTypeComparer.Instance);
    }
}
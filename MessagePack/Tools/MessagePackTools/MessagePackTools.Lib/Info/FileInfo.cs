using Microsoft.CodeAnalysis;

namespace MessagePackTools {
    public class FileInfo {
        public string Path => SyntaxTree.FilePath;

        public int Hash;
        public SyntaxTree SyntaxTree;
        public string AsmDefPath;

        public override string ToString() => $"{Path} : {Hash}";
    }
}
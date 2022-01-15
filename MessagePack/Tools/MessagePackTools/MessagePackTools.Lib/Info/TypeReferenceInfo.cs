using Microsoft.CodeAnalysis;

namespace MessagePackTools {
    public class TypeReferenceInfo : IDisplayType {
        private string? _displayString = null;
        public string DisplayString => _displayString ??= Type.ToDisplayString();
        
        public ITypeSymbol Type;
        public string AsmDefPath;
        public string FilePath;

        public override string ToString() => $"{DisplayString} : {AsmDefPath}::{FilePath}";
    }
}
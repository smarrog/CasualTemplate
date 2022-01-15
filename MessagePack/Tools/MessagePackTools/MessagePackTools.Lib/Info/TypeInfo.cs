using Microsoft.CodeAnalysis;

namespace MessagePackTools {
    public class TypeInfo : IDisplayType {
        private string? _displayString = null;
        public string DisplayString => _displayString ??= Type.ToDisplayString();

        public ITypeSymbol Type;
        public int Hash;
    }
}
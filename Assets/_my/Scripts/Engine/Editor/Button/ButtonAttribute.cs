using System.Reflection;
using UnityEngine;

namespace Smr.Editor {
    public class ButtonAttribute : PropertyAttribute {
        public readonly string _methodName;
        public readonly string _buttonName;
        public readonly bool _useValue;
        public readonly BindingFlags _flags;

        public ButtonAttribute(string methodName, string buttonName, bool useValue = false, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance) {
            _methodName = methodName;
            _buttonName = buttonName;
            _useValue = useValue;
            _flags = flags;
        }
        public ButtonAttribute(string methodName, bool useValue, BindingFlags flags) : this(methodName, methodName, useValue, flags) {}
        public ButtonAttribute(string methodName, bool useValue) : this(methodName, methodName, useValue) {}
        public ButtonAttribute(string methodName, string buttonName, BindingFlags flags) : this(methodName, buttonName, false, flags) {}
        public ButtonAttribute(string methodName, BindingFlags flags) : this(methodName, methodName, false, flags) {}
        public ButtonAttribute(string methodName) : this(methodName, methodName) {}
    }
}
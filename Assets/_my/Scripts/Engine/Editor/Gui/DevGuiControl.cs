using System;
using System.Collections.Generic;
using UnityEngine;

namespace Smr.Editor {
    public static class DevGuiControl {
        private static readonly Dictionary<string, Action> _registeredControls = new();


        public static void BeginControl() {
            _registeredControls.Clear();
        }

        public static void EndControl() {
            CheckRegisteredControls();
        }

        public static void RegisterNextControlEnter(string controlName, Action action) {
            GUI.SetNextControlName(controlName);
            _registeredControls[controlName] = action;
        }

        public static void CheckRegisteredControls() {
            if (_registeredControls.Count == 0) {
                return;
            }

            var enteredControlName = EnteredControlName();
            if (enteredControlName == null) {
                return;
            }

            if (_registeredControls.ContainsKey(enteredControlName)) {
                _registeredControls[enteredControlName]();
            }
        }

        public static string EnteredControlName() {
            // TODO: sometimes it executes twice; may be, because there is several executions at frame
            if (Event.current.isKey && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter)) {
                return GUI.GetNameOfFocusedControl();
            }
            return null;
        }
    }
}
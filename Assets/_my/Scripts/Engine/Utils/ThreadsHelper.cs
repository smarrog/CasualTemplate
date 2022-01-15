using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Smr.Utils {
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public static class ThreadsHelper {
        [field : ThreadStatic]
        public static bool IsMain { get; }

        private static readonly Queue<Action> _mainThreadEditorActions = new Queue<Action>();


        static ThreadsHelper() {
            IsMain = true;
#if UNITY_EDITOR
            EditorApplication.update += EditorUpdate;
#endif
        }

        private static void EditorUpdate() {
            lock (_mainThreadEditorActions) {
                while (_mainThreadEditorActions.Count > 0) {
                    try {
                        _mainThreadEditorActions.Dequeue()();
                    } catch (Exception ex) {
                        Debug.Log("Queued main thread action exception: " + ex.Message + "\n" + ex.StackTrace);
                    }
                }
            }
        }

        public static void InvokeEditorOnMain(Action action) {
            if (action == null) {
                return;
            }

            // TODO: add stack information for debug
            lock (_mainThreadEditorActions) {
                _mainThreadEditorActions.Enqueue(action);
            }
        }
    }
}
using System.Linq;
using System.Text;
using UnityEngine;

namespace Smr.Extensions {
    public static class GameObjectExtension {
        public static string HierarchyToString(this GameObject go) {
            if (go == null) {
                return string.Empty;
            }

            var sb = new StringBuilder(go.name);
            var t = go.transform;

            while (t.parent != null) {
                t = t.parent;
                sb.Insert(0, t.gameObject.name + "/");
            }
            return sb.ToString();
        }

        public static T GetOrAddComponent<T>(this GameObject go) where T : Component {
            if (go == null) {
                return null;
            }
            T component = go.GetComponent<T>();
            if (component == null) {
                component = go.AddComponent<T>();
            }
            return component;
        }

        public static bool IsDestroyed(this MonoBehaviour obj) { //object is not instantiated in the scene
            return obj == null || obj.gameObject == null;
        }

        public static void SmartDestroy(this GameObject go) {
            if (Application.isEditor) {
                Object.DestroyImmediate(go);
            } else {
                Object.Destroy(go);
            }
        }

        public static void SetActiveSafe(this GameObject go, bool value) {
            if (go) {
                go.SetActive(value);
            }
        }

        public static void SetLayer(this GameObject go, int layer, bool isRecursive = true) {
            go.layer = layer;
            if (!isRecursive) {
                return;
            }

            foreach (var child in go.transform.Cast<Transform>()) {
                SetLayer(child.gameObject, layer);
            }
        }

        public static void DestroySafe(this GameObject go) {
            if (!go) {
                return;
            }

            if (Application.isPlaying) {
                Object.Destroy(go);
            } else {
                Object.DestroyImmediate(go);
            }
        }
    }
}
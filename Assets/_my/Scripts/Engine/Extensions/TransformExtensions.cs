using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Smr.Extensions {
    public static class TransformExtensions {
        public static List<Transform> GetChildren(this Transform transform) {
            return transform.Cast<Transform>().ToList();
        }
        
        public static List<GameObject> GetChildrenAsGo(this Transform transform) {
            return transform
                .GetChildren()
                .Select(child => child.gameObject)
                .ToList();
        }

        public static void RemoveAllChildren(this Transform transform) {
            foreach (var child in transform.Cast<Transform>().ToArray()) {
                if (Application.isPlaying) {
                    Object.Destroy(child.gameObject);
                } else {
                    Object.DestroyImmediate(child.gameObject);
                }
            }
        }

        public static IEnumerable<T> GetComponentsFromDirectDescendants<T>(this Transform transform) where T : Object {
            return transform
                .GetChildren()
                .Select(child => child.GetComponent<T>())
                .Where(component => component);
        }
    }
}
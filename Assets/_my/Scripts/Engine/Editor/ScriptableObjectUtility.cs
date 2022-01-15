using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Smr.Editor {
    public static class ScriptableObjectUtility {
        public static T CreateAsset<T>() where T : ScriptableObject {
            return ScriptableObject.CreateInstance<T>();
        }

        public static void SaveAsset<T>(T asset, string path, string assetName = null) where T : ScriptableObject {
            if (string.IsNullOrEmpty(assetName)) {
                assetName = "New " + typeof(T);
            }

            var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + assetName + ".asset");

            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
        
        public static T[] GetAll<T>() where T : ScriptableObject {
            var guids = AssetDatabaseUtility.GetGuids<T>();
            var a = new T[guids.Length];
            for (int i = 0; i < guids.Length; i++) { // probably could get optimized 
                a[i] = AssetDatabaseUtility.GetByGuid<T>(guids[i]);
            }

            return a;
        }

        public static IEnumerable<T> GetAllAtPath<T>(string path) where T : ScriptableObject {
            var assetPaths = AssetDatabase.GetAllAssetPaths().Where(p => p.StartsWith(path));
            foreach (var assetPath in assetPaths) {
                var obj = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (obj != null) {
                    yield return obj;
                }
            }
        }

        public static T GetOrCreateAtPath<T>(string path) where T:ScriptableObject {
            var existing = AssetDatabase.LoadAssetAtPath<T>(path);
            return existing ? existing : ScriptableObject.CreateInstance<T>();
        }
        
        public static string GetName<T>(T asset) where T : ScriptableObject {
            var path = AssetDatabaseUtility.GetPath(asset);
            return Path.GetFileNameWithoutExtension(path);
        }

        public static T GetFirst<T>() where T : ScriptableObject {
            var guids = AssetDatabaseUtility.GetGuids<T>();
            return guids.Length == 0 ? null : AssetDatabaseUtility.GetByGuid<T>(guids[0]);
        }
    }
}
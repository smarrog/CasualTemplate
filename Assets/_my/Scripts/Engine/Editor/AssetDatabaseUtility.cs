using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Smr.Editor {
    public static class AssetDatabaseUtility {
        public static bool AssetExists<T>(string path) where T : Object {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            return asset != null;
        }

        public static void CreateOrReplaceAsset(string path, Object newAsset) {
            var oldAsset = AssetDatabase.LoadAssetAtPath<Object>(path);
            if (oldAsset != null) {
                EditorUtility.CopySerialized(newAsset, oldAsset);
                EditorUtility.SetDirty(oldAsset);
                AssetDatabase.SaveAssetIfDirty(oldAsset);
            } else {
                Directory.CreateDirectory(Path.Combine(Application.dataPath, "..", Path.GetDirectoryName(path)));
                AssetDatabase.CreateAsset(newAsset, path);
            }
        }

        public static string[] GetGuids(Type type) {
            return AssetDatabase.FindAssets($"t: {type.Name}");
        }

        public static string[] GetGuids<T>() where T : Object {
            return AssetDatabase.FindAssets($"t: {typeof(T).Name}");
        }

        public static string[] GetGuids<T>(params string[] searchInFolders) where T : Object {
            return AssetDatabase.FindAssets($"t: {typeof(T).Name}", searchInFolders);
        }

        public static string GetGuid(Object asset) {
            string guid = null;
            if (asset != null) {
                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out guid, out long _);
            }
            return guid;
        }

        public static string GetPath<T>(T asset) where T : Object {
            if (asset == null) {
                return null;
            }
            string guid = GetGuid(asset);
            return guid != null ? AssetDatabase.GUIDToAssetPath(guid) : null;
        }

        public static Object GetByGuid(string guid, Type type) {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            return AssetDatabase.LoadAssetAtPath(path, type);
        }

        public static T GetByGuid<T>(string guid) where T : Object {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }

        public static T GetInstance<T>() where T : Object {
            return GetAllInstances<T>().FirstOrDefault();
        }

        public static IEnumerable<T> GetAllInstances<T>() where T : Object {
            return AssetDatabase
                .FindAssets($"t: {typeof(T).Name}")
                .Select(GetByGuid<T>)
                .Where(asset => asset != null);
        }

        public static IEnumerable<T> GetAllInstances<T>(params string[] searchInFolders) where T : Object {
            return AssetDatabase
                .FindAssets($"t: {typeof(T).Name}", searchInFolders)
                .Select(GetByGuid<T>)
                .Where(asset => asset != null);
        }

        public static Object[] GetAllInstances(Type type) {
            var guids = GetGuids(type);
            var a = new Object[guids.Length];
            for (int i = 0; i < guids.Length; i++) { // probably could get optimized 
                a[i] = GetByGuid(guids[i], type);
            }

            return a;
        }
    }
}
#if SMR_ADDRESSABLES
#if UNITY_EDITOR
using System.Linq;
using Smr.Common;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Smr.AddressableAssets.Editor {
    public static class AddressableAssets {
        public static bool IsAssetMarkedAsAddressable(AssetReference assetRef) {
            return IsAssetMarkedAsAddressable(assetRef.AssetGUID);
        }

        public static bool IsAssetMarkedAsAddressable(Object obj) {
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj, out var guid, out long _);
            return IsAssetMarkedAsAddressable(guid);
        }

        public static bool IsAssetMarkedAsAddressable(string guid) {
            if (!AddressableAssetSettingsDefaultObject.SettingsExists) {
                return false;
            }
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var entry = settings.FindAssetEntry(guid);
            return entry != null;
        }

        public static bool IsAssetInDefaultGroup(Object obj) {
            if (!AddressableAssetSettingsDefaultObject.SettingsExists) {
                return false;
            }
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj, out var guid, out long _);
            return IsAssetInDefaultGroup(guid);
        }

        public static bool IsAssetInDefaultGroup(string guid) {
            if (!AddressableAssetSettingsDefaultObject.SettingsExists) {
                return false;
            }
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var entry = settings.FindAssetEntry(guid);
            return AddressableEntries.IsEntryInDefaultGroup(entry);
        }

        public static string GetAssetAddress(Object obj) {
            var entry = AddressableEntries.GetEntry(obj);
            return entry?.address;
        }

        public static TValue LoadAssetInEditor<TValue>(AddressableKey key) where TValue : Object {
            if (key.AssetRef != null) {
                return key.AssetRef.editorAsset as TValue;
            }
            return LoadAssetInEditor<TValue>(key.ToString());
        }

        private static TValue LoadAssetInEditor<TValue>(string address) where TValue : Object {
            var allEntries = AddressableEntries.GetAll();
            var foundEntry = allEntries.FirstOrDefault(entry => entry.address == address);
            if (foundEntry == null) {
                return null;
            }

            if (foundEntry.MainAsset is TValue foundAsset) {
                return foundAsset;
            }
            if (foundEntry.MainAsset != null) {
                EngineDependencies.Logger.GetChannel(LogChannel.Addressables).LogError($"Wrong addressable asset type. Expected: {typeof(TValue)}, got: {foundEntry.MainAsset.GetType()}");
            }
            return null;
        }

        public static void SetupAssetLabel(Object obj, string label, bool enable, bool force = false, bool postEvent = true) {
            var entry = AddressableEntries.GetEntry(obj);
            entry.SetLabel(label, enable, force, postEvent);
            AddressableAssetSettingsDefaultObject.Settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryModified, entry, true);
            AssetDatabase.SaveAssets();
        }
    }
}
#endif
#endif
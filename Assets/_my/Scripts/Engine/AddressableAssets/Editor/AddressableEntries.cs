#if SMR_ADDRESSABLES
#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using Smr.Extensions;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Smr.AddressableAssets.Editor {
    public static class AddressableEntries {
        public static List<AddressableAssetEntry> GetAll() {
            var allEntries = new List<AddressableAssetEntry>();
            var settings = AddressableAssetSettingsDefaultObject.Settings;

            foreach (var assetGroup in settings.groups) {
                if (!assetGroup.IsNull()) {
                    foreach (var assetEntry in assetGroup.entries) {
                        if (assetEntry != null) {
                            allEntries.Add(assetEntry);
                        }
                    }
                }
            }
            return allEntries;
        }

        public static IEnumerable<AddressableAssetEntry> GetInGroup(string groupName) {
            var group = AddressableGroups.Get(groupName);
            return group != null ? group.entries : Enumerable.Empty<AddressableAssetEntry>();
        }

        public static AddressableAssetEntry GetEntry(Object obj) {
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj, out var guid, out long _);
            return AddressableAssetSettingsDefaultObject.Settings.FindAssetEntry(guid);
        }

        public static bool HasEntryForAddress(string address) {
            var allEntries = GetAll();
            var foundEntry = allEntries.FirstOrDefault(e => e.address == address);
            return foundEntry != null;
        }

        public static AddressableAssetEntry CreateEntry(
            string assetPath,
            string address,
            AddressableAssetGroup group = null,
            IEnumerable<string> labels = null
        ) {
            var guid = AssetDatabase.AssetPathToGUID(assetPath);
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (!group) {
                group = settings.DefaultGroup;
            }
            var entry = settings.CreateOrMoveEntry(guid, group, false, false);
            entry.address = address;
            if (labels != null) {
                entry.labels.UnionWith(labels);
            }
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryCreated, entry, true);
            AssetDatabase.SaveAssets();
            return entry;
        }

        public static bool IsEntryInDefaultGroup(AddressableAssetEntry entry) {
            if (!AddressableAssetSettingsDefaultObject.SettingsExists) {
                return false;
            }
            var defaultGroup = AddressableAssetSettingsDefaultObject.Settings.DefaultGroup;
            return entry.parentGroup == defaultGroup;
        }
    }
}
#endif
#endif
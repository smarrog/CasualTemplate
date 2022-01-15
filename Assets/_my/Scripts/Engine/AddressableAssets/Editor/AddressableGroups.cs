#if SMR_ADDRESSABLES
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Smr.AddressableAssets.Editor {
    public static class AddressableGroups {
        public static AddressableAssetGroup GetOrCreate(string groupName, string templateName = null) {
            var group = Get(groupName);

            if (!group) {
                AddressableAssetGroup templateGroup = null;
                if (!string.IsNullOrEmpty(templateName)) {
                    templateGroup = Get(templateName);
                }
                group = AddressableAssetSettingsDefaultObject.Settings.CreateGroup(
                    groupName,
                    false,
                    false,
                    false,
                    templateGroup != null ? templateGroup.Schemas : null
                );
            }
            return group;
        }

        public static AddressableAssetGroup Get(string groupName) {
            return AddressableAssetSettingsDefaultObject.Settings.FindGroup(groupName);
        }

        public static void SetGroup(Object obj, string groupName) {
            var entry = AddressableEntries.GetEntry(obj);
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var assetGroup = GetOrCreate(groupName);
            settings.CreateOrMoveEntry(entry.guid, assetGroup, false, false);
            AddressableAssetSettingsDefaultObject.Settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entry, true);
            AssetDatabase.SaveAssets();
        }
    }
}
#endif
#endif
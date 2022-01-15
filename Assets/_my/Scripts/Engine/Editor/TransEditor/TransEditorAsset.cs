using UnityEditor;
using UnityEngine;

namespace Smr.Editor {
    public class TransEditorAsset<T> where T : Object {
        private readonly string _prefsKey;

        private readonly T _defEditorAsset;
        private bool _isAssetReceived;

        private T _asset;
        public T Asset {
            get {
                if (!_isAssetReceived) {
                    var guid = EditorPrefs.GetString(_prefsKey);

                    _asset = string.IsNullOrEmpty(guid)
                        ? _defEditorAsset
                        : AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));

                    _isAssetReceived = true;
                }
                return _asset;
            }
            set {
                if (_asset != value) {
                    _asset = value;

                    var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(value));
                    EditorPrefs.SetString(_prefsKey, guid);
                }
            }
        }


        public TransEditorAsset(string prefsKey, T defaultAsset = null) {
            _prefsKey = prefsKey;
            _defEditorAsset = defaultAsset;
        }

        public static implicit operator bool(TransEditorAsset<T> transAsset) => transAsset.Asset;
    }
}
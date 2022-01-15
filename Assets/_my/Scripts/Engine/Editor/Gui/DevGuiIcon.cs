using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Smr.Editor {
    public static class DevGuiIcon {
        private const string ASSETS_PATH = "Assets/Dev/Icons/";
        private static readonly Dictionary<string, Texture2D> _cache = new Dictionary<string, Texture2D>();

        public static Texture2D RedS => GetTexture("RedS");
        public static Texture2D GrayS => GetTexture("GrayS");
        public static Texture2D Warning => GetTexture("Warning");
        public static Texture2D Wrench => GetTexture("wrench");
        public static Texture2D FlagOn => GetTexture("flag_on");
        public static Texture2D FlagOff => GetTexture("flag_off");
        public static Texture2D Add => GetTexture("add");
        public static Texture2D CheckGreen => GetTexture("check_green");
        public static Texture2D CheckGray => GetTexture("check_gray");
        public static Texture2D ArrowRight => GetTexture("arrow_right");
        public static Texture2D ArrowLeft => GetTexture("arrow_left");
        public static Texture2D Edit => GetTexture("edit");

        private static Texture2D _blackTexture;
        public static Texture2D BlackTexture {
            get {
                if (_blackTexture == null) {
                    _blackTexture = new Texture2D(1, 1);
                    _blackTexture.SetPixel(0, 0, Color.black);
                    _blackTexture.Apply();
                }
                return _blackTexture;
            }
        }


        public static Texture2D GetTexture(string iconName) {
            if (!_cache.ContainsKey(iconName)) {
                var iconPath = ASSETS_PATH + iconName + ".png";

                _cache[iconName] = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
                if (_cache[iconName] == null) {
                    Debug.LogWarning("[DevGui]: can't find editor icon at: " + iconPath);
                }
            }
            return _cache[iconName];
        }
    }
}
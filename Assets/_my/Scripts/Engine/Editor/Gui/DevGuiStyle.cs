using UnityEditor;
using UnityEngine;

namespace Smr.Editor {
    public static class DevGuiStyle {
        public static GUIStyle MenuItem => GUI.skin.FindStyle("MenuItem");
        public static GUIStyle SpaceStyle => new GUIStyle { stretchWidth = false, alignment = TextAnchor.MiddleCenter, margin = new RectOffset(4, -4, 2, 2)};
        
        // Buttons
        public static GUIStyle GetButton(float width, float height) => new GUIStyle(GUI.skin.button) { fixedWidth = width, fixedHeight = height };
        public static GUIStyle MicroButton => GetButton(18, 15);
        public static GUIStyle Width40Button => new GUIStyle(GUI.skin.button) { fixedWidth = 40 };
        public static GUIStyle Width55Button => new GUIStyle(GUI.skin.button) { fixedWidth = 55 };
        public static GUIStyle Width100Button => new GUIStyle(GUI.skin.button) { fixedWidth = 100 };
        public static GUIStyle ListButtonLeft => new GUIStyle(EditorStyles.toolbarButton) { alignment = TextAnchor.MiddleLeft };
        public static GUIStyle ListButtonLeftSelected => new GUIStyle(ListButtonLeft) { normal = { background = Texture2D.grayTexture }};
        
        // texts
        public static GUIStyle BoldText => new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold };
        public static GUIStyle CenteredText => new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
        public static GUIStyle RightAlignedText => new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleRight };
        
        
        // lists
        private static Texture2D _oddBackTexture;
        private static bool _isListItemOdd;

        public static GUIStyle EvenListLine => new GUIStyle(GUI.skin.label) { fixedHeight = 17 };
        public static GUIStyle OddListLine => new GUIStyle(EvenListLine) { normal = { background = OddBackTexture } };

        public static void StartListLines() {
            _isListItemOdd = true;
        }

        public static GUIStyle GetNextListLine() {
            _isListItemOdd = !_isListItemOdd;
            return _isListItemOdd ? OddListLine : EvenListLine;
        }

        private static Texture2D OddBackTexture {
            get {
                if (!_oddBackTexture) {
                    _oddBackTexture = new Texture2D(1, 1);
                    _oddBackTexture.SetPixel(0, 0, UnityBackgroundColor - new Color32(6, 6, 6, 0));
                    _oddBackTexture.Apply();
                }
                return _oddBackTexture;
            }
        }
        private static Color UnityBackgroundColor => EditorGUIUtility.isProSkin
            ? new Color32(56, 56, 56, 255)
            : new Color32(194, 194, 194, 255);
    }
}
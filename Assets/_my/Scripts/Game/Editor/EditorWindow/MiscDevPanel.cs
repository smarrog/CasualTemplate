using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace Game.Editor {
    [UsedImplicitly]
    public class MiscDevPanel : AbstractDevPanel {
        public override void GUIWindow() {
            GUILayout.Space(3);
            GuiSceneButton(EditorConstants.MAIN_SCENE_PATH);
            GuiSceneButton(EditorConstants.PRELOAD_SCENE_PATH);
            
            if (GUILayout.Button("Select settings")) {
                var settingsAsset = AssetDatabase.LoadAssetAtPath<Settings>(EditorConstants.SETTINGS_PATH);
                Selection.activeObject = settingsAsset;
                EditorWindowHelper.ShowInspectorEditorWindow();
            }
        }
    }
}
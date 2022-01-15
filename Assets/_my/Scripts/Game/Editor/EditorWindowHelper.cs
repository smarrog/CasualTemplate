using UnityEditor;

namespace Game.Editor {
    public class EditorWindowHelper {
        public static void ShowInspectorEditorWindow() {
            var inspectorWindowTypeName = "UnityEditor.InspectorWindow";
            ShowEditorWindowWithTypeName(inspectorWindowTypeName);
        }

        public static void ShowSceneEditorWindow() {
            var sceneWindowTypeName = "UnityEditor.SceneView";
            ShowEditorWindowWithTypeName(sceneWindowTypeName);
        }

        public static void ShowEditorWindowWithTypeName(string windowTypeName) {
            var windowType = typeof(UnityEditor.Editor).Assembly.GetType(windowTypeName);
            EditorWindow.GetWindow(windowType);
        }
    }
}
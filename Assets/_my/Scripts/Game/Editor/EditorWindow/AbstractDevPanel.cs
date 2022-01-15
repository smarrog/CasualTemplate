using System;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Editor {
    public abstract class AbstractDevPanel : IDisposable {
        public virtual void Init() {}
        public virtual void GUIWindow() {}
        public virtual void Refresh() {}

        public void Dispose() {
            OnDestroy();
        }

        protected virtual void OnDestroy() {}

        protected void GuiSceneButton(string scenePath) {
            if (Application.isPlaying) {
                return;
            }

            var sceneName = Path.GetFileNameWithoutExtension(scenePath);
            var isSampleSceneOpened = IsSceneOpened(sceneName);
            if (isSampleSceneOpened) {
                return;
            }

            if (GUILayout.Button($"Open scene \"{sceneName}\"")) {
                EditorSceneManager.OpenScene(scenePath);
            }
        }

        private static bool IsSceneOpened(string sceneName) {
            for (int i = 0; i < SceneManager.sceneCount; i++) {
                var openedScene = SceneManager.GetSceneAt(i);
                if (openedScene.name == sceneName) {
                    return true;
                }
            }
            return false;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Smr.Editor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Game.Editor {
    [InitializeOnLoad]
    public class DevelopWindow : EditorWindow {
        private const int HEADER_SPACING = 7;
        private const int HEADER_BUTTONS_MARGIN = 2;
        private const int TAB_BUTTON_IN_A_ROW = 5;
        
        private static readonly TransEditorString _lastOpenedDevPanel = new("last.opened.DevPanel", "");
        
        private static bool _isInitialized;
        private static Dictionary<Type, string> _editorTypesNamed;
        private static Dictionary<Type, AbstractDevPanel> _editorsHash;
        private static AbstractDevPanel _currentPanel;
        private static readonly DevGuiScroll _frameScroll = new();
        
        static DevelopWindow() {
            TryInit();
        }

        private static void TryInit() {
            if (_isInitialized) {
                return;
            }
            
            var rawTypes = ReflectionHelper.GetDerivedTypes(typeof(AbstractDevPanel), new[] { "Assembly-CSharp" });
            _editorTypesNamed = rawTypes.ToDictionary(
                t => t,
                t => t.Name.Replace("DevPanel", ""));
            _editorsHash = new Dictionary<Type, AbstractDevPanel>();

            _isInitialized = true;
        }
        
        [MenuItem("Tools/Develop %`", false, 3000)]
        private static void ShowDevelopWindow() {
            ShowAndFocus();
        }
        
        [DidReloadScripts]
        private static void OnScriptsReloaded() {
            _isInitialized = false;
            TryInit();
        }

        private static void ShowAndFocus() {
            var window = GetWindow<DevelopWindow>();
            window.ShowUtility();
            window.Focus();
        }
        
        private void OnEnable() {
            titleContent = new GUIContent("Dev");
            
            TryRestore();
        }

        private void TryRestore() {
            var lastOpenedType = _editorTypesNamed.Keys.FirstOrDefault(t => t.FullName == _lastOpenedDevPanel.Value);
            SetEditor(lastOpenedType);
        }
        
        private void OnGUI() {
            GuiHeader();
            
            if (_currentPanel != null) {
                _frameScroll.Draw(_currentPanel.GUIWindow);
            }
        }

        private void GuiHeader() {
            using (new GUILayout.HorizontalScope()) {
                GUILayout.Space(HEADER_SPACING);

                ToolbarSelect<MiscDevPanel>();
            }

            DevGui.HorizontalLine();
            GUILayout.Space(5);
        }

        private void ToolbarSelect<T>() where T : AbstractDevPanel {
            var type = typeof(T);
            if (!_editorTypesNamed.TryGetValue(type, out var editorName)) {
                return;
            }

            var buttonLabel = new GUIContent(editorName);
            var isVisible = _currentPanel != null && _currentPanel.GetType() == type;
            var buttonWidth = GUILayout.Width(GetHeaderButtonWidth());
            var newVisibility = GUILayout.Toggle(isVisible, buttonLabel, EditorStyles.toolbarButton, buttonWidth);

            if (newVisibility != isVisible) {
                GUIUtility.keyboardControl = 0;
            }
            if (!isVisible && newVisibility) {
                SetEditor(type);
            }

            GUILayout.Space(HEADER_BUTTONS_MARGIN);
        }

        private float GetHeaderButtonWidth() {
            // window size without borders and space, divided by 5 elements in row
            return (position.width - HEADER_SPACING * 3) / TAB_BUTTON_IN_A_ROW - HEADER_BUTTONS_MARGIN;
        }


        private void SetEditor(Type type) {
            if (type == null) {
                _currentPanel = null;
                _lastOpenedDevPanel.Value = "";
                titleContent = new GUIContent("Dev");
                return;
            }
            
            if (!_editorsHash.ContainsKey(type)) {
                _editorsHash[type] = Activator.CreateInstance(type) as AbstractDevPanel;
                _editorsHash[type].Init();
            } else {
                _editorsHash[type].Refresh();
            }

            _currentPanel = _editorsHash[type];
            _lastOpenedDevPanel.Value = type.FullName;
            titleContent = new GUIContent("Dev: " + _editorTypesNamed[type]);
        }
    }
}
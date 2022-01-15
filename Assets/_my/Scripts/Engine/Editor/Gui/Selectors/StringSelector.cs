using System;
using System.Collections.Generic;
using System.Linq;
using Smr.Extensions;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Smr.Editor {
    public class StringSelector : PopupWindowContent {

        private static int _controlId = -1;

        private static bool _havePendingValue;
        private static string _pendingValue;

        private readonly SearchField _search = new();
        private string _searchText = "";
        private Vector2 _scrollPos = Vector2.zero;

        private readonly List<string> _options;
        private List<string> _filteredOptions;

        private bool _shouldClose;


        public static string GetSelectedValue(string value, int controlId) {
            if (_havePendingValue && controlId == _controlId) {
                value = _pendingValue;

                _pendingValue = null;
                _havePendingValue = false;
            }

            return value;
        }

        public StringSelector(IEnumerable<string> values, int controlId) {
            _options = values.ToList();
            _filteredOptions = _options;
            _controlId = controlId;
        }

        public override void OnGUI(Rect rect) {
            int border = 4;
            int topPadding = 4;
            int searchHeight = 20;
            var searchRect = new Rect(border, topPadding, rect.width - border * 2, searchHeight);

            DrawSearch(searchRect);
            DrawValues();

            if (_shouldClose) {
                GUIUtility.hotControl = 0;
                editorWindow.Close();
            }
        }

        public override Vector2 GetWindowSize() {
            return new Vector2(300, 400);
        }

        public override void OnOpen() {
            _search.SetFocus();
            base.OnOpen();
        }

        private void DrawSearch(Rect rect) {
            string newSearchText = _search.OnGUI(rect, _searchText);
            if (_searchText != newSearchText) {
                _searchText = newSearchText;
                UpdateFilteredValues();
            }
            GUILayout.Space(rect.height);
        }

        private void UpdateFilteredValues() {
            _filteredOptions = string.IsNullOrWhiteSpace(_searchText)
                ? _options
                : _options.FindAll(s => s.IsFilled() && s.Contains(_searchText, StringComparison.InvariantCultureIgnoreCase));
        }

        private void DrawValues() {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            foreach (var value in _filteredOptions) {
                if (GUILayout.Button(value, DevGuiStyle.ListButtonLeft)) {
                    _pendingValue = value;
                    _havePendingValue = true;
                    _shouldClose = true;
                }
            }

            EditorGUILayout.EndScrollView();
        }

    }
}
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace Smr.Editor {
    public class DevGuiScroll {
        private Vector2 _viewPos;
        public float AreaHeightCrop { get; private set; }

        public void Draw([NotNull] Action innerFrame) {
            var lastRect = GUILayoutUtility.GetLastRect();

            _viewPos = EditorGUILayout.BeginScrollView(_viewPos);
            {
                AreaHeightCrop += lastRect.yMax;

                try {
                    innerFrame.Invoke();
                } catch (Exception ex) {
                    if (ex is ExitGUIException) {
                        throw;
                    }
                    GUILayout.TextArea("Exception occured:\n" + ex.Message + "\n\n" + ex.StackTrace, GUILayout.ExpandHeight(true));
                    Debug.LogException(ex);
                }

                AreaHeightCrop -= lastRect.yMax;
            }
            EditorGUILayout.EndScrollView();
        }
    }

    public class DevGuiLimitedScroll {
        private readonly float _rowHeight;

        private readonly TransEditorFloat _heightPrefs;
        private readonly DevDraggableEdge _draggableEdge;
        private readonly Vector2 _heightRange;
        private float _height;

        private Vector2 _viewPos;

        private int _visibleRowsLimit;


        public DevGuiLimitedScroll(float rowHeight, int visibleRowsLimit) {
            _rowHeight = rowHeight;

            _visibleRowsLimit = visibleRowsLimit;
            _height = visibleRowsLimit * rowHeight;
        }

        public DevGuiLimitedScroll(float rowHeight, Vector2 heightRange, string name) {
            _rowHeight = rowHeight;

            _heightPrefs = new TransEditorFloat(name, heightRange.x);
            _draggableEdge = new DevDraggableEdge(OnDragged);
            _heightRange = heightRange;
            _height = _heightPrefs;

            _visibleRowsLimit = Mathf.CeilToInt(_height / _rowHeight);
        }

        public void Draw<T>([NotNull] IList<T> items, [NotNull] Action<T> drawAction) {
            int itemsCount = items.Count;

            bool useScroller = itemsCount > _visibleRowsLimit;
            if (useScroller) {
                _viewPos = EditorGUILayout.BeginScrollView(_viewPos, GUILayout.Height(_height));

                float contentHeight = itemsCount * _rowHeight;
                if (_viewPos.y > contentHeight - _height) {
                    _viewPos.y = contentHeight - _height;
                }
            } else {
                _viewPos = Vector2.zero;
            }

            int minVisibleIndex = Mathf.Max(Mathf.CeilToInt(_viewPos.y / _rowHeight) - 1, 0);
            int maxVisibleIndex = Mathf.Min(itemsCount - 1, minVisibleIndex + _visibleRowsLimit + 1);

            if (minVisibleIndex > 0) {
                EditorGUILayout.GetControlRect(GUILayout.Height(minVisibleIndex * _rowHeight));
            }

            for (int i = minVisibleIndex; i <= maxVisibleIndex; i++) {
                try {
                    drawAction(items[i]);
                } catch (Exception ex) {
                    GUILayout.TextArea($"Exception occured:\n{ex.Message}\n\n{ex.StackTrace}", GUILayout.ExpandHeight(true));
                    Debug.LogException(ex);
                }
            }

            if (maxVisibleIndex < itemsCount - 1) {
                EditorGUILayout.GetControlRect(GUILayout.Height((itemsCount - 1 - maxVisibleIndex) * _rowHeight));
            }

            if (useScroller) {
                EditorGUILayout.EndScrollView();
            }

            _draggableEdge?.Draw();
        }

        private void OnDragged(float offset) {
            _height = Mathf.Clamp(_height + offset, _heightRange.x, _heightRange.y);
            _visibleRowsLimit = Mathf.CeilToInt(_height / _rowHeight);

            _heightPrefs.Value = _height;
        }
    }
}
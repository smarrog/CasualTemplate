using System;
using UnityEditor;
using UnityEngine;

namespace Smr.Editor {
    public class DevDraggableEdge {
        private static readonly int _controlId = nameof(DevDraggableEdge).GetHashCode();

        private const float GUI_HEIGHT = 5;
        private const float GUI_KNOB_WIDTH = 42;

        private readonly Action<float> _onDragged;

        private float _lastDragY;
        private bool _isDragging;


        public DevDraggableEdge(Action<float> onDragged) {
            _onDragged = onDragged;
        }

        public void Draw() {
            var pos = EditorGUILayout.GetControlRect(false, GUI_HEIGHT);
            int controlId = GUIUtility.GetControlID(_controlId, FocusType.Passive, pos);

            var e = Event.current;
            var eventType = e.GetTypeForControl(controlId);
            var mPos = e.mousePosition;

            switch (eventType) {
                case EventType.MouseDown:
                    if (pos.Contains(mPos)) {
                        GUIUtility.hotControl = controlId;
                        e.Use();

                        _isDragging = true;
                        _lastDragY = mPos.y;
                    }
                    break;

                case EventType.MouseMove:
                    if (pos.Contains(mPos)) {
                        e.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    if (_isDragging) {
                        e.Use();

                        float offset = mPos.y - _lastDragY;
                        if (Mathf.Abs(offset) > .5f) {
                            _lastDragY = mPos.y;

                            _onDragged(offset);
                        }
                    }
                    break;

                case EventType.MouseUp:
                    if (_isDragging) {
                        GUIUtility.hotControl = 0;
                        e.Use();

                        _isDragging = false;
                    }
                    break;

                case EventType.Repaint:
                    DrawGui(pos);
                    break;
            }
        }

        private void DrawGui(Rect pos) {
            EditorGUIUtility.AddCursorRect(pos, MouseCursor.ResizeVertical);

            float xLeft = pos.center.x - GUI_KNOB_WIDTH / 2;
            float xRight = xLeft + GUI_KNOB_WIDTH;
            float yTop = pos.center.y - 1;
            float yBottom = yTop + 2;

            Handles.color = Color.gray;
            Handles.DrawLine(new Vector2(xLeft, yTop), new Vector2(xRight, yTop));
            Handles.DrawLine(new Vector2(xLeft, yBottom), new Vector2(xRight, yBottom));
        }
    }
}
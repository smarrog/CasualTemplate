using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smr.Common;
using Smr.Extensions;
using Smr.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Smr.Components {
    public class ExtendedGraphicRaycaster : GraphicRaycaster {
        // default value is important!
        [SerializeField]
        private bool isRaycastsEnabled = true;
        // property is needed for breakpoints.
        public bool IsRaycastsEnabled {
            get { return isRaycastsEnabled; }
            set { isRaycastsEnabled = value; }
        }

        [SerializeField]
        private bool m_LogRaycasts = false;
        // i'm leaving this here in case of bugs because it provides good enough logs

        [SerializeField]
        [Tooltip("This checkbox should be enabled on everything except windows canvas")]
        private bool AddSelectablesOnly = true;


        [SerializeField]
        [Tooltip("in world units")]
        private float maxTouchMisHit = 20;

        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList) {
            if (!IsRaycastsEnabled)
                return;

            StringBuilder sb = new StringBuilder("On " + gameObject.HierarchyToString() + " raycast happened\n");
            if (m_LogRaycasts) {
                LogEventData(eventData, sb);
                LogReslutsList("before raycast", resultAppendList, sb);
            }

            var currList = new List<RaycastResult>();
            base.Raycast(eventData, currList);
            foreach (var curr in currList)
                if (AddSelectablesOnly)
                    AddIfHasSelectable(resultAppendList, curr);
                else
                    resultAppendList.Add(curr);
            //SetToMaxSortOrder(resultAppendList, currList);

            if (m_LogRaycasts)
                LogReslutsList("after raycast", resultAppendList, sb);

            if (resultAppendList.Any(SelectableIsInteractable)) {
                // remove background that takes priority
                resultAppendList.RemoveAll(SelectableIsNotInteractable);
            } else {
                var allSelectable = gameObject.GetComponentsInChildren<Selectable>();

                bool selectableFound = AddClosestSelectable(eventData, resultAppendList, allSelectable);
                if (selectableFound)
                    resultAppendList.RemoveAll(SelectableIsNotInteractable);
                // this removes background from hit if a user touched near the button, because background takes priority i don't know why

                if (m_LogRaycasts) {
                    LogSelectables(allSelectable, sb);
                    LogReslutsList("after adding", resultAppendList, sb);
                }
            }

            if (m_LogRaycasts && eventData.eligibleForClick)
                EngineDependencies.Logger.Log(sb.ToString());
        }

        private static void AddIfHasSelectable(List<RaycastResult> results, RaycastResult curr) {
            var selectable = curr.gameObject.GetComponentInParent<Selectable>();
            var scroll = curr.gameObject.GetComponentInParent<ScrollRect>();

            GameObject go = null;
            if (scroll != null)
                go = scroll.gameObject;
            // порядок важен, кнопка перекрывает скролл!
            if (selectable != null)
                go = selectable.gameObject;
            if (go == null)
                go = curr.gameObject;

            results.Add(new RaycastResult() {
                depth = curr.depth,
                distance = curr.distance,
                // самая важная строчка
                gameObject = go,
                index = curr.index,
                module = curr.module,
                screenPosition = curr.screenPosition,
                sortingLayer = curr.sortingLayer,
                sortingOrder = curr.sortingOrder,
                worldNormal = curr.worldNormal,
                worldPosition = curr.worldPosition
            });
        }

        private static void SetToMaxSortOrder(List<RaycastResult> resultAppendList, List<RaycastResult> currList) {
            var maxSortOrder = int.MinValue;
            foreach (var result in resultAppendList)
                if (result.module.sortOrderPriority > maxSortOrder)
                    maxSortOrder = result.module.sortOrderPriority;

            foreach (var curr in currList)
                if (curr.module.sortOrderPriority > maxSortOrder)
                    maxSortOrder = curr.module.sortOrderPriority;

            var maxList = new List<RaycastResult>();
            foreach (var result in resultAppendList)
                if (maxSortOrder == result.module.sortOrderPriority)
                    maxList.Add(result);
            foreach (var curr in currList)
                if (maxSortOrder == curr.module.sortOrderPriority)
                    maxList.Add(curr);
            resultAppendList.Clear();
            resultAppendList.AddRange(maxList);
        }

        private static bool SelectableIsNotInteractable(RaycastResult result) {
            return !SelectableIsInteractable(result);
        }

        private static bool SelectableIsInteractable(RaycastResult result) {
            if (result.gameObject == null)
                return false;
            var selectable = result.gameObject.GetComponentInParent<Selectable>();
            if (selectable == null)
                return false;
            return selectable.IsInteractable();
        }

        private bool AddClosestSelectable(
            PointerEventData eventData,
            List<RaycastResult> resultAppendList,
            Selectable[] allSelectable
        ) {
            float closestDistanceSq = float.MaxValue;
            Selectable closestSelectable = null;

            foreach (var selectable in allSelectable) {
                if (!selectable.IsInteractable())
                    continue;

                bool isBlockedByMask = false;
                var filters = FindFilters(selectable);
                if (filters.Count > 0) {
                    foreach (var filter in filters) {
                        if (filter is Mask && !(filter as Mask).enabled)
                            continue;
                        isBlockedByMask |=
                            !filter.IsRaycastLocationValid(eventData.position, eventData.pressEventCamera);
                    }
                }
                if (isBlockedByMask)
                    continue;

                Vector3[] corners = GetCorners(selectable);
                float maxDistanceSq = maxTouchMisHit * maxTouchMisHit;
                var overrider = selectable.GetComponent<SelectableBorderOverrider>();
                if (overrider != null)
                    maxDistanceSq = overrider.maxTouchMishit * overrider.maxTouchMishit;

                for (int i = 0; i < 3; ++i) {
                    float distanceSq = MathUtils.SquareDistanceFromPointToSection(eventData.position, corners[i],
                        corners[i + 1]);
                    if (distanceSq < maxDistanceSq) {
                        if (closestDistanceSq > distanceSq) {
                            closestSelectable = selectable;
                            closestDistanceSq = distanceSq;
                        }
                    }
                }
            }

            if (closestSelectable != null) {
                RaycastResult result = CreateRaycastResult(eventData, closestSelectable, resultAppendList.Count);
                resultAppendList.Add(result);
                eventData.pointerCurrentRaycast = result;
                eventData.pointerPressRaycast = result;
                return true;
            }
            return false;
        }

        private static Vector3[] GetCorners(Selectable selectable) {
            Vector3[] corners = new Vector3[4];
            if (selectable.targetGraphic == null)
                (selectable.transform as RectTransform).GetWorldCorners(corners);
            else
                selectable.targetGraphic.rectTransform.GetWorldCorners(corners);
            return corners;
        }

        private List<ICanvasRaycastFilter> FindFilters(Selectable selectable) {
            Transform t = selectable.transform;
            var filters = new List<ICanvasRaycastFilter>();
            while (t.parent != null && t.parent != transform) {
                var filter = t.GetComponent<ICanvasRaycastFilter>();
                if (filter != null)
                    filters.Add(filter);
                t = t.parent;
            }
            return filters;
        }

        private RaycastResult CreateRaycastResult(PointerEventData eventData, Selectable selectable, int resultIndex) {
            var result = new RaycastResult();
            result.module = this;
            result.index = resultIndex;
            result.gameObject = selectable.gameObject;
            result.screenPosition = eventData.position;
            if (selectable.targetGraphic != null) {
                var canvasRenderer = selectable.targetGraphic.GetComponent<CanvasRenderer>();
                if (canvasRenderer != null)
                    result.depth = canvasRenderer.absoluteDepth;
            }

            return result;
        }

        private void LogSelectables(Selectable[] allSelectable, StringBuilder sb) {
            sb.AppendLine();
            sb.AppendLine("interactive selectables in this");
            foreach (var selectable in allSelectable) {
                if (!selectable.IsInteractable())
                    continue;
                Vector3[] corners = GetCorners(selectable);

                sb.AppendLine(selectable.gameObject.HierarchyToString() + " at " + ListToString(corners, ", ", ""));
            }
        }

        private void LogReslutsList(string title, List<RaycastResult> resultAppendList, StringBuilder sb) {
            sb.AppendLine();
            sb.AppendLine(title);
            sb.Append(ListToString(resultAppendList, Environment.NewLine, Environment.NewLine));
        }

        private static void LogEventData(PointerEventData eventData, StringBuilder sb) {
            sb.AppendLine("Pointer event data:" + eventData.ToString());
            sb.AppendLine();
        }

        private string ListToString<T>(IEnumerable<T> list, string elementsSeparator, string startEndSeparator) {
            if (list == null)
                return "(null)";

            StringBuilder sb =
                new StringBuilder(list.GetType().Name + " of " + list.Count() + " elements:{" + startEndSeparator);
            foreach (var element in list)
                sb.Append(element.ToString() + elementsSeparator);
            sb.Append("}" + startEndSeparator);
            return sb.ToString();
        }
    }
}
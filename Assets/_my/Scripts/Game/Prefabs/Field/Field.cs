using System.Collections.Generic;
using Smr.Components;
using Smr.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game {
    public class Field : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler {
        [Header("Parameters")]
        [SerializeField] private GridElement _elementPrefab;
        [SerializeField] private Transform _container;
        [SerializeField] private GraphicRaycaster _graphicRaycaster;
        [SerializeField] private Image _draggableImage;
        
        [Header("Debug")]
        [SerializeField, ReadOnly] private int _draggedElementIndex;
        [SerializeField, ReadOnly] private int _highlightedElementIndex;
        
        private GridElement HighlightedElement => _highlightedElementIndex != -1 ? _elements[_highlightedElementIndex] : null;
        private GridElement DraggedElement => _draggedElementIndex >= 0 ? _elements[_draggedElementIndex] : null;
        private bool IsDragging => _draggedElementIndex != -1;

        private bool _isInitialized;
        private readonly List<GridElement> _elements = new();
        private readonly List<RaycastResult> _raycastResults = new();

        private void OnDestroy() {
            if (_isInitialized) {
                UnSubscribe();
            }
        }

        public void Init() {
            if (_isInitialized) {
                return;
            }
            
            _isInitialized = true;
            _draggedElementIndex = -1;
            _highlightedElementIndex = -1;
            _elements.Clear();
            _raycastResults.Clear();
            
            _container.RemoveAllChildren();

            for (var i = 0; i < App.FieldLogic.SlotsAmount; ++i) {
                var element = Instantiate(_elementPrefab, _container);
                element.Init(i);
                _elements.Add(element);
            }
            
            Subscribe();
        }
        
        public void OnPointerDown(PointerEventData eventData) {
            if (IsDragging) {
                return;
            }
            
            var elementIndex = GetGridElementUnderPointerIndex(eventData);
            if (elementIndex == -1) {
                return;
            }
            
            var slotInfo = App.FieldLogic.GetSlotInfo(elementIndex);
            if (slotInfo.IsGift) {
                App.UiLogic.ShowGiftWindow(elementIndex);
                return;
            }

            if (!slotInfo.IsFilledWithElement) {
                return;
            }
            
            _draggedElementIndex = elementIndex;
            DraggedElement.SetSelected(true);
            _draggableImage.enabled = true;
            _draggableImage.sprite = DraggedElement.CurrentSprite;
            
            OnPointerMove(eventData);
        }
        
        public void OnPointerMove(PointerEventData eventData) {
            if (!IsDragging) {
                return;
            }
            
            _draggableImage.transform.position = eventData.position;
            
            var elementIndex = GetGridElementUnderPointerIndex(eventData);
            if (_highlightedElementIndex == elementIndex) {
                return;
            }
            
            ResetCurrentHighlight();
            CheckIfNeedToBeHighlighted(elementIndex);
        }
        
        public void OnPointerUp(PointerEventData eventData) {
            if (!IsDragging) {
                return;
            }
            
            var result = App.FieldLogic.MoveTo(_draggedElementIndex, _highlightedElementIndex);
            App.SignalBus.Fire(new MoveSignal(result));
            
            DraggedElement.SetSelected(false);
            ResetCurrentDrag();
            ResetCurrentHighlight();
        }

        private void UpdateHandler(float deltaTime) {
            foreach (var element in _elements) {
                element.UpdateHandler(deltaTime);
            }
            
            var spawnDeltaTime = deltaTime;
            if (App.GiftLogic.Active == GiftType.SpawnSpeed) {
                spawnDeltaTime *= App.Settings.Meta.Gifts.SpawnSpeedMultiplier;
            }
            App.FieldLogic.AccelerateSpawn(spawnDeltaTime);
            SpawnIfNeed();
        }

        private void ResetCurrentDrag() {
            _draggedElementIndex = -1;
            _draggableImage.enabled = false;
            _draggableImage.sprite = null;
        }

        private void ResetCurrentHighlight() {
            if (_highlightedElementIndex == -1) {
                return;
            }
            
            HighlightedElement.SetIsHighlighted(false);
            _highlightedElementIndex = -1;
        }

        private void CheckIfNeedToBeHighlighted(int index) {
            var canBeMoved = App.FieldLogic.CanMoveTo(_draggedElementIndex, index);
            if (!canBeMoved) {
                return;
            }
            
            _highlightedElementIndex = index;
            HighlightedElement.SetIsHighlighted(true);
        }

        private int GetGridElementUnderPointerIndex(PointerEventData eventData) {
            _raycastResults.Clear();
            _graphicRaycaster.Raycast(eventData, _raycastResults);
            foreach (var raycastResult in _raycastResults) {
                var gridElement = raycastResult.gameObject.GetComponentInParent<GridElement>();
                if (gridElement) {
                    return gridElement.Index;
                }
            }
            return -1;
        }

        private void SpawnIfNeed() {
            if (App.FieldLogic.TimeFromLastSpawn < App.FieldLogic.SpawnInterval) {
                return;
            }

            var isSpawned = App.FieldLogic.TryToSpawn(out var spawnIndex);
            if (!isSpawned) {
                return;
            }
            
            if (_highlightedElementIndex != spawnIndex) {
                return;
            }
            
            ResetCurrentHighlight();
            CheckIfNeedToBeHighlighted(spawnIndex);
        }

        private void Subscribe() {
            App.Scheduler.RegisterUpdate(UpdateHandler);

            App.SignalBus.Subscribe<ElementLevelChangedSignal>(OnElementLevelChangedSignal);
            App.SignalBus.Subscribe<ElementUnlockedSignal>(OnElementUnlockBoughtSignal);
            App.SignalBus.Subscribe<ResetProgressSignal>(OnResetProgressSignal);
        }

        private void UnSubscribe() {
            App.Scheduler?.UnregisterUpdate(UpdateHandler);
            
            App.SignalBus.Unsubscribe<ElementLevelChangedSignal>(OnElementLevelChangedSignal);
            App.SignalBus.Unsubscribe<ElementUnlockedSignal>(OnElementUnlockBoughtSignal);
            App.SignalBus.Unsubscribe<ResetProgressSignal>(OnResetProgressSignal);
        }

        private void OnElementLevelChangedSignal(ElementLevelChangedSignal signal) {
            _elements[signal.Index].UpdateSlotInfo();
        }

        private void OnElementUnlockBoughtSignal(ElementUnlockedSignal signal) {
            _elements[signal.Index].UpdateSlotInfo();
        }

        private void OnResetProgressSignal(ResetProgressSignal signal) {
            UnSubscribe();
            
            _isInitialized = false;

            Init();
        }
    }
}
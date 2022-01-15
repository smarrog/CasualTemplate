using System;

namespace Smr.Utils {
    public class QueueWithMaxSize<T> {
        private readonly Deque<T> _deque = new();

        public int Count => _deque.Count;
        public int Length => _deque.Count;
        public int MaxSize { get; private set; }

        public QueueWithMaxSize(int maxSize = int.MaxValue) {
            SetMaxSize(maxSize);
        }

        public void Clear() {
            _deque.Clear();
        }

        public bool Contains(T value) {
            return _deque.Contains(value);
        }

        public void SetMaxSize(int value) {
            MaxSize = Math.Max(0, value);
            CheckSize();
        }

        public T Dequeue(bool isSafe = false) {
            return _deque.TakeLast(isSafe);
        }

        public void Enqueue(T value) {
            _deque.AddLast(value);
            CheckSize();
        }

        public T Peek(T value, bool isSafe = false) {
            return isSafe ? _deque.LastSafe : _deque.Last;
        }

        private void CheckSize() {
            while (Count > MaxSize) {
                _deque.TakeLast();
            }
        }
    }
}
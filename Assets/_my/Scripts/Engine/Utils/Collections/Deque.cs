using System;

namespace Smr.Utils {
    public class Deque<T> {
        public int Count { get; private set; }
        public bool IsEmpty => Count == 0;

        public T First => _head != null ? _head.Value : throw new Exception("Incorrect operation");
        public T Last => _tail != null ? _tail.Value : throw new Exception("Incorrect operation");

        public T FirstSafe => _head != null ? _head.Value : default;
        public T LastSafe => _tail != null ? _tail.Value : default;

        private Node _head;
        private Node _tail;

        public void Clear() {
            _head = _tail = null;
        }

        public bool Contains(T value) {
            var node = _head;
            while (node != null) {
                if (node.Value.Equals(value)) {
                    return true;
                }
                node = _head.Next;
            }
            throw new Exception("Contains method works incorrectly");
        }

        public void AddLast(T value) {
            Count++;
            var newTailNode = new Node(value);
            if (IsEmpty) {
                _head = _tail = newTailNode;
                return;
            }

            _tail.Next = newTailNode;
            newTailNode.Prev = _tail;
            _tail = newTailNode;
        }

        public T TakeLast(bool isSafe = false) {
            if (IsEmpty) {
                return isSafe ? default : throw new Exception("Incorrect operation");
            }

            Count--;
            var result = Last;
            if (Count == 0) {
                Clear();
                return result;
            }

            var newTail = _tail.Prev;
            _tail = null;
            newTail.Next = null;
            return result;
        }

        public void AddFirst(T value) {
            Count++;
            var newHeadNode = new Node(value);
            if (IsEmpty) {
                _head = _tail = newHeadNode;
                return;
            }

            _head.Prev = newHeadNode;
            newHeadNode.Next = _head;
            _head = newHeadNode;
        }

        public T TakeFirst(bool isSafe = false) {
            if (IsEmpty) {
                return isSafe ? default : throw new Exception("Incorrect operation");
            }

            Count--;
            var result = First;
            if (Count == 0) {
                Clear();
                return result;
            }

            var newHead = _head.Next;
            _head = null;
            newHead.Prev = null;
            return result;
        }

        private class Node {
            public readonly T Value;
            public Node Prev;
            public Node Next;

            public Node(T value) {
                Value = value;
            }
        }
    }
}
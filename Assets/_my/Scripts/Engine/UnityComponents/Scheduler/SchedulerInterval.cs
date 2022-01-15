using System;
using System.Numerics;

namespace Smr.Components {
    public class SchedulerInterval {
        private readonly bool _needRandom;
        private readonly float _min;
        private readonly float _max;
        
        public SchedulerInterval(float interval) {
            _needRandom = false;
            _min = _max = interval;
        }
        
        public SchedulerInterval(Vector2 interval) {
            _needRandom = true;
            _min = interval.X;
            _max = interval.Y;
        }

        public float GetNextInterval() {
            if (!_needRandom) {
                return _min;
            }
            
            return (float) (_min + new Random().NextDouble() * (_max - _min));
        }
    }
}
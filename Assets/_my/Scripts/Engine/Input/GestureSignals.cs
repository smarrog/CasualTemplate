using UnityEngine;

namespace Smr.Input {
    public class TapGestureSignal {
        public float X;
        public float Y;
    }
    
    public class DoubleTapGestureSignal {
        public float X;
        public float Y;
    }
    
    public class PanGestureSignal {
        public float X;
        public float Y;
    }
    
    public class PanGestureEndedSignal {
    }
    
    public class ScaleGestureSignal {
        public float Scale;
    }
    
    public class ScaleGestureEndedSignal {
    }

    public class LongTapBeganGestureSignal {
        public float X;
        public float Y;
    }

    public class LongTapEndedGestureSignal {
        public float X;
        public float Y;
    }

    public class SwipeSignal {
        public Vector3 Direction;
    }
}
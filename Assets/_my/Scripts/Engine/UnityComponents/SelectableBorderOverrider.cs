using UnityEngine;
using UnityEngine.UI;

namespace Smr.Components {
    [RequireComponent(typeof(Selectable))]
    internal class SelectableBorderOverrider : MonoBehaviour {
        //Extended graphics raycaster use this value to control collider size
        //zero if original collider size needed
        [SerializeField] public float maxTouchMishit = 0;
    }
}
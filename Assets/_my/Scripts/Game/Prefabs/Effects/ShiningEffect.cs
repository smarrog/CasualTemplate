using Smr.Components;
using UnityEngine;

namespace Game {
    [RequireComponent(typeof(RenderToImage))]
    public class ShiningEffect : MonoBehaviour {
        private RenderToImage RenderToImage {
            get {
                if (!_renderToImage) {
                    _renderToImage = GetComponent<RenderToImage>();
                    _renderToImage.SetSize(256, 256);
                }
                return _renderToImage;
            }
        }
        private RenderToImage _renderToImage;

        private void OnEnable() {
            RenderToImage.StartRender();
        }

        private void OnDisable() {
            RenderToImage.StopRender();
        }
    }
}
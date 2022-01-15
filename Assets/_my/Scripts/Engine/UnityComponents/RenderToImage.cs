using UnityEngine;
using UnityEngine.UI;

namespace Smr.Components {
    public class RenderToImage : MonoBehaviour {
        [SerializeField] private Camera _camera;
        [SerializeField] private RawImage _image;

        private RenderTexture _texture;

        private void Awake() {
            SetSize(Screen.width, Screen.height);
            StopRender();
        }

        private void OnDestroy() {
            DestroyTexture();
        }

        public void SetSize(int width, int height) {
            DestroyTexture();
            _texture = new RenderTexture(width, height, 32, RenderTextureFormat.ARGB32);
            _image.texture = _texture;
        }

        public void StartRender() {
            _image.gameObject.SetActive(true);
            _camera.targetTexture = _texture;
        }

        public void StopRender() {
            _image.gameObject.SetActive(false);
            _camera.targetTexture = null;
        }

        private void DestroyTexture() {
            if (_texture != null) {
                Destroy(_texture);
            }
        }
    }
}
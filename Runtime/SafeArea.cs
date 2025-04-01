using UnityEngine;

namespace HarryUtils {
    public class SafeArea : MonoBehaviour {
        // ==================== VARIABLES ===================
        #region Private Variables
        [SerializeField] private RectTransform rectTransform;
        #endregion
        
        // ==================== START ====================
        private void Start() {
            if (rectTransform == null) {
                TryGetComponent(out rectTransform);
            }

            ApplySafeArea();
        }
        
        // ==================== METHODS ====================
        private void ApplySafeArea() {
            Rect _safeArea = Screen.safeArea;

            // Convertimos el area segura en 'Canvas Space'
            var _minAnchor = _safeArea.position;
            var _maxAnchor = _safeArea.position + _safeArea.size;
            _minAnchor.x /= Screen.width;
            _minAnchor.y /= Screen.height;
            _maxAnchor.x /= Screen.width;
            _maxAnchor.x /= Screen.height;

            // Aplicamos el area segura al 'RectTransform'
            rectTransform.anchorMin = _minAnchor;
            rectTransform.anchorMax = _maxAnchor;
        }
    }
}
using UnityEngine;
using TMPro;

namespace HarryUtils.UI {
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class VersionLabel : MonoBehaviour {
        // ==================== VARIABLES ===================
        #region Private Variables
        [SerializeField] private TextMeshProUGUI label;
        #endregion
        
        // ==================== INICIO ====================
        private void Start() {
            if (label == null) TryGetComponent(out label);
            label.SetText($"v{Application.version}");
        }
    }
}
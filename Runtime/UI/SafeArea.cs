using System;
using UnityEngine;

namespace Game.General {
    [RequireComponent(typeof(RectTransform))]
    public class SafeArea : MonoBehaviour {
        // ==================== VARIABLES ===================
        #region Private Variables
        // Values
        /*
        private static int cachedStatusBarHeight = -1;
        private static int cachedNavBarHeight = -1;

        private bool wasKeyboardVisible;
        */

        // References
        private RectTransform safeAreaRect;
        private Rect lastSafeArea;
        #endregion

        // ==================== START ====================
        private void Awake() {
            TryGetComponent(out safeAreaRect);
            ApplySafeArea();
        }

        private void Update() {
            /*
                bool _isKeyboardVisible = TouchScreenKeyboard.visible;

                if (!_isKeyboardVisible && wasKeyboardVisible) {
                    ApplySafeArea();
                }

                wasKeyboardVisible = _isKeyboardVisible;

                if (!_isKeyboardVisible && Screen.safeArea != lastSafeArea) {
                    ApplySafeArea();
                }
            */
            if (Screen.safeArea != lastSafeArea) {
                ApplySafeArea();
            }
        }

        // ==================== METHODS ====================
        private void ApplySafeArea() {
            if (!safeAreaRect) {
                throw new NullReferenceException("There's no Rect Transform to adjust");
            }

            Rect _safeArea = Screen.safeArea;
            /*
#if UNITY_ANDROID && !UNITY_EDITOR
            if (Mathf.Approximately(_safeArea.y, 0) && Mathf.Approximately(_safeArea.height, Screen.height)) {
                int _statusBarHeight = GetStatusBarHeight();
                int _navBarHeight = GetNavigationBarHeight();

                if (_statusBarHeight > 0 || _navBarHeight > 0) {
                    _safeArea.y = _statusBarHeight;
                    _safeArea.height = Screen.height - _statusBarHeight - _navBarHeight;
                }
            }
#endif
            */

            lastSafeArea = _safeArea;

            Vector2 _anchorMin = _safeArea.position;
            Vector2 _anchorMax = _safeArea.position + _safeArea.size;

            _anchorMin.x /= Screen.width;
            _anchorMin.y /= Screen.height;
            _anchorMax.x /= Screen.width;
            _anchorMax.y /= Screen.height;

            safeAreaRect.anchorMin = _anchorMin;
            safeAreaRect.anchorMax = _anchorMax;
        }

        /*
        public static int GetStatusBarHeight() {
            if (cachedStatusBarHeight >= 0) {
                return cachedStatusBarHeight;
            }

            using (AndroidJavaClass _unityPlayer = new("com.unity3d.player.UnityPlayer")) {
                using (AndroidJavaObject _activity = _unityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) {
                    using (AndroidJavaObject _window = _activity.Call<AndroidJavaObject>("getWindow")) {
                        using (AndroidJavaObject _view = _window.Call<AndroidJavaObject>("getDecorView")) {
                            using (AndroidJavaObject _rect = new("android.graphics.Rect")) {
                                _view.Call("getWindowVisibleDisplayFrame", _rect);
                                cachedStatusBarHeight = _rect.Get<int>("top");
                            }
                        }
                    }
                }
            }

            return cachedStatusBarHeight;
        }

        public static int GetNavigationBarHeight() {
            if (cachedNavBarHeight >= 0) {
                return cachedNavBarHeight;
            }

#if UNITY_ANDROID && !UNITY_EDITOR
            using (AndroidJavaClass _unityPlayer = new("com.unity3d.player.UnityPlayer")) {
                using (AndroidJavaObject _activity = _unityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) {
                    using (AndroidJavaObject _resources = _activity.Call<AndroidJavaObject>("getResources")) {
                        int _resourceId = _resources.Call<int>("getIdentifier", "navigation_bar_height", "dimen", "android");
                        cachedNavBarHeight = _resourceId > 0 ? _resources.Call<int>("getDimensionPixelSize", _resourceId) : 0;
                    }
                }
            }
#endif
            return cachedNavBarHeight;
        }
        */
    }
}
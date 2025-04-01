using System.Collections.Generic;
using UnityEngine;

namespace HarryUtils.Timer {
    public static class TimerBasketExtensions {
        public static Timer AddToBasket<T>(this Timer _timer, T _key) {
            TimerBasket<T>.Get.AddToList(_key, _timer);
            return _timer;
        }
    }
        
    public class TimerBasket<T> : MonoBehaviour {
        // ==================== VARIABLES ===================
        #region Public Variables
        public static TimerBasket<T> Get {
            get {
                if (timerBasket == null) {
                    GameObject _timerBasket = new("Timer Basket");
                    timerBasket = _timerBasket.AddComponent<TimerBasket<T>>();
                    DontDestroyOnLoad(timerBasket);
                }

                return timerBasket;
            }
        }

        public Dictionary<T, Timer> Timers => timers;
        #endregion

        #region Private Variables
        private static TimerBasket<T> timerBasket;
        private Dictionary<T, Timer> timers;
        #endregion

        // ==================== METHODS ====================
        public void AddToList(T _key, Timer _timer) {
            timers ??= new();
            
            if (timers.ContainsKey(_key)) Debug.LogWarning($"Ya existe el temporizador que se intenta añadir: {_key}");
            else timers.Add(_key, _timer);
        }

        public void RemoveFromList(T _key) {
            if (!timers.ContainsKey(_key)) Debug.LogWarning($"No existe el temporizador que se intenta eliminar: {_key}");
            else timers.Remove(_key);
        }
    }
}
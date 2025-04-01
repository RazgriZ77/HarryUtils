using System;
using UnityEngine;

namespace HarryUtils.Timer {
    public class Timer {
        // ==================== VARIABLES ===================
        #region Public Variables
        public bool Finished { get; private set; } = false;
        public float ElapsedTime { get; private set; } = 0f;
        #endregion
        
        #region Private Variables
        protected float duration = 0f;
        protected bool inverted = false;

        protected Action callback;
        #endregion
        
        // ==================== METHODS ====================
        public Timer(float _duration, Action _callback = null, bool _inverted = false) {
            ElapsedTime = _inverted ? _duration : 0f;

            duration = _inverted ? 0 : _duration;
            inverted = _inverted;
            Finished = false;

            callback = _callback;
        }

        public Timer WithCallback(Action _callback) {
            callback += _callback;
            return this;
        }
        
        public void Ticks() {
            if (Finished) return;
            
            if (inverted) Regressive();
            else Progressive();

            void Progressive() {
                if (duration > ElapsedTime) ElapsedTime += Time.deltaTime;
                else {
                    ElapsedTime = duration;
                    Finished = true;

                    callback?.Invoke();
                }
            }

            void Regressive() {
                if (ElapsedTime > 0) ElapsedTime -= Time.deltaTime;
                else {
                    ElapsedTime = 0;
                    Finished = true;

                    callback?.Invoke();
                }
            }
        }
    }
}
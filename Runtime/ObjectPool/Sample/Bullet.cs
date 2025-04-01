using UnityEngine;

namespace HarryUtils.ObjectPool.Sample {
    public class Bullet : MonoBehaviour {
        public interface IProtocol {
            void OnStop(Bullet _bullet);
        }
        
        // ==================== VARIABLES ===================
        #region Private Variables
        private IProtocol Delegate { get; set; }
        private float speed = 0f;
        #endregion

        // ==================== START ====================
        void FixedUpdate() {
            transform.position += transform.forward * speed;
        }

        // ==================== METHODS ====================
        public Bullet SetDelegate(IProtocol _delegate) {
            Delegate = _delegate;
            return this;
        }
        
        public Bullet Shoot(Transform _point, float _speed, float _lifeTime) {
            transform.position = _point.position;
            transform.forward = _point.forward;
            
            speed = _speed;
            Invoke(nameof(Stop), _lifeTime);

            return this;
        }

        // =================================================
        
        private void Stop() {
            speed = 0f;
            Delegate?.OnStop(this);
        }
    }
}
using UnityEngine;

namespace HarryUtils.ObjectPool {
    public class LimitedObjectPool<T> : ObjectPool<T> where T : Component {
        // ==================== VARIABLES ===================
        #region Private Variables
        private readonly int maxSize;
        #endregion
        
        // ==================== START ====================
        public LimitedObjectPool(T _prefab, int _size, int _maxSize) : base(_prefab, _size) {
            maxSize = _maxSize;
        }

        // ==================== METHODS ====================
        public override T GetObject() {
            foreach (T _obj in pool) {
                if (_obj.gameObject.activeInHierarchy) continue;
                _obj.gameObject.SetActive(true);

                return _obj;
            }

            if (pool.Count < maxSize) {
                T _newObj = Object.Instantiate(prefab);
                _newObj.gameObject.SetActive(true);
                pool.Add(_newObj);

                return _newObj;

            } else {
                var _oldestObj = pool[0];
                pool.RemoveAt(0);
                pool.Add(_oldestObj);

                return _oldestObj;
            }
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace HarryUtils.ObjectPool {
    public class ObjectPool<T> where T : Component {
        // ==================== VARIABLES ===================
        #region Private Variables
        protected readonly List<T> pool;
        protected readonly T prefab;
        #endregion
        
        // ==================== START ====================
        public ObjectPool(T _prefab, int _size) {
            prefab = _prefab;
            pool = new List<T>();

            for (int i = 0; i < _size; i++) {
                T _obj = Object.Instantiate(prefab);
                _obj.gameObject.SetActive(false);
                pool.Add(_obj);
            }
        }
        
        // ==================== METHODS ====================
        public virtual T GetObject() {
            foreach (T _obj in pool) {
                if (_obj.gameObject.activeInHierarchy) continue;
                _obj.gameObject.SetActive(true);
                return _obj;
            }

            T _newObj = Object.Instantiate(prefab);
            _newObj.gameObject.SetActive(true);
            pool.Add(_newObj);

            return _newObj;
        }

        public void ReleaseObject(T _obj) {
            _obj.gameObject.SetActive(false);
        }
    }
}
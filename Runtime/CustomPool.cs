using UnityEngine.Pool;

namespace HarryUtils {
    public abstract class CustomPool<T1, T2> where T1 : class {
        public ObjectPool<T1> pool;
        public T2 parentClass;

        public CustomPool(T2 _parentClass) {
            parentClass = _parentClass;
            pool = new ObjectPool<T1>(OnCreate, OnGet, OnRelease);
        }

        public T1 Get() => pool.Get();

        protected abstract T1 OnCreate();
        protected abstract void OnGet(T1 _entity);
        protected abstract void OnRelease(T1 _entity);
    }
}
namespace HarryUtils.Singleton {
    public abstract class SingletonClass<T> {
        public static T Instance { get; private set; }
        public static void SetInstance(T _instance) => Instance = _instance;
    }
}
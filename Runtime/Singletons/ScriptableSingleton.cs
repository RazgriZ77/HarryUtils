using UnityEngine;

namespace HarryUtils.Singleton {
    public abstract class ScriptableSingleton<T> : ScriptableObject where T : ScriptableObject {
        public static T Instance;
        public static void SetInstance(T _instance) => Instance = _instance;
    }
}
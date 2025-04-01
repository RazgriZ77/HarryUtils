using UnityEngine;

namespace HarryUtils {
    public abstract class ScriptableSingleton<T> : ScriptableObject where T : ScriptableObject {
        public static T Instance;
    }
}
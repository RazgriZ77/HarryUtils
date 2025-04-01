using UnityEngine;

namespace HarryUtils.Singleton {
	public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
		#region Public Variables
		public static bool IsAwake => instance != null;
		public static T Instance {
			get {
				if (instance == null) {
					instance = (T)FindFirstObjectByType(typeof(T));

					if (instance == null) {
						string _goName = typeof(T).ToString();
						GameObject _go = GameObject.Find(_goName);
						
						if (_go == null) _go = new() {
							name = _goName
						};
						
						instance = _go.AddComponent<T>();
					}
				}

				return instance;
			}
		}
		#endregion

		#region Private Variables
		private static T instance = null;
		#endregion
	}
}
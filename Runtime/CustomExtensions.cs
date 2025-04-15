using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

using Newtonsoft.Json;

namespace HarryUtils {
    public static class CustomExtensions {
        public const string MatchEmailPattern = 
		@"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
		+ @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
		+ @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
		+ @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";
        
        #region GameObject
        /// <summary> This method is used to hide the GameObject in the Hierarchy view. </summary>
        public static void HideInHierarchy(this GameObject _gameObject) => _gameObject.hideFlags = HideFlags.HideInHierarchy;
        
        /// <summary> Gets a component of the given type attached to the GameObject. If that type of component does not exist, it adds one. </summary>
        /// <remarks>
        /// This method is useful when you don't know if a GameObject has a specific type of component,
        /// but you want to work with that component regardless. Instead of checking and adding the component manually,
        /// you can use this method to do both operations in one line.
        /// </remarks>
        /// <typeparam name="T">The type of the component to get or add.</typeparam>
        /// <param name="_gameObject">The GameObject to get the component from or add the component to.</param>
        /// <returns>The existing component of the given type, or a new one if no such component exists.</returns> 
        public static T GetOrAdd<T>(this GameObject _gameObject) where T : Component {
            T _component = _gameObject.GetComponent<T>();
            if (!_component) _component = _gameObject.AddComponent<T>();

            return _component;
        }

        /// <summary> Returns the object itself if it exists, null otherwise. </summary>
        /// <remarks>
        /// This method helps differentiate between a null reference and a destroyed Unity object. Unity's "== null" check
        /// can incorrectly return true for destroyed objects, leading to misleading behaviour. The OrNull method use
        /// Unity's "null check", and if the object has been marked for destruction, it ensures an actual null reference is returned,
        /// aiding in correctly chaining operations and preventing NullReferenceExceptions.
        /// </remarks>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="_obj">The object being checked.</param>
        /// <returns>The object itself if it exists and not destroyed, null otherwise.</returns>
        public static T OrNull<T>(this T _obj) where T : UnityEngine.Object => _obj ? _obj : null;

        /// <summary> Sets the layer of a GameObject and its children </summary>
        public static void SetLayerRecursively(this GameObject _gameObject, int _layer) {
            _gameObject.layer = _layer;

            for (int i = 0; i < _gameObject.transform.childCount; i++) {
                _gameObject.transform.GetChild(i).gameObject.SetLayerRecursively(_layer);
            }
        }
        #endregion

        #region Vector2
        public static int GetRandomInBetween(this Vector2Int _vector2, Unity.Mathematics.Random _random) => _random.NextInt(_vector2.x, _vector2.y + 1);
        public static float GetRandomInBetween(this Vector2 _vector2, Unity.Mathematics.Random _random) => _random.NextFloat(_vector2.x, _vector2.y);
        #endregion

        #region Vector3
        /// <summary> Sets any 'x', 'y', 'z' values of a Vector3 </summary>
        public static Vector3 With(this Vector3 _vector, float? _x = null, float? _y = null, float? _z = null) {
            return new Vector3(_x ?? _vector.x, _y ?? _vector.y, _z ?? _vector.z);
        }

        /// <summary> Add anything to the 'x', 'y', 'z' values of a Vector3 </summary>
        public static Vector3 Add(this Vector3 _vector, float? _x = null, float? _y = null, float? _z = null) {
            return new Vector3(_vector.x + (_x ?? 0), _vector.y + (_y ?? 0), _vector.z + (_y ?? 0));
        }
        #endregion
        
        #region Dictionary
        public static void Add<TKey, TValue>(this Dictionary<TKey, TValue> _dictionary, KeyValuePair<TKey, TValue> _keyValuePair) {
            _dictionary.Add(_keyValuePair.Key, _keyValuePair.Value);
        }

        public static void AddMany<TKey, TValue>(this Dictionary<TKey, TValue> _dictionary, Dictionary<TKey, TValue> _newDictionary) {
            if (_newDictionary.IsNullOrEmpty()) return;
            
            foreach (KeyValuePair<TKey, TValue> _item in _newDictionary) {
                if (!_dictionary.ContainsKey(_item.Key)) _dictionary.Add(_item.Key, _item.Value);
            }
        }
        #endregion

        #region IEnumerables
        public static void AddMany<T>(this List<T> _group, T _element, int _quantity) {
            if (_group.IsNull()) {
                Debug.LogError("Se debe inicializar la lista de elementos");
                return;
            }

            if (_quantity <= 0) {
                Debug.LogError("Se debe de añadir una cantidad de objetos superior a 0");
                return;
            }
            
            for (int i = 0; i < _quantity; i++) {
                _group.Add(_element);
            }
        }

        public static bool IsNull<T>(this IEnumerable<T> _group) => _group == null;
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> _group) => _group == null || _group.Count() == 0;
        public static bool IsOutOfBounds<T>(this IEnumerable<T> _group, int _index) => _index < 0 || _index >= _group.Count();

        public static T GetMostCommon<T>(this IEnumerable<T> _group) {
            // Diccionario para contar las ocurrencias de cada elemento
            Dictionary<T, int> _counts = new();

            // Recorrer la colección y contar las ocurrencias de cada elemento
            foreach (T _element in _group) {
                if (_counts.ContainsKey(_element)) _counts[_element]++;
                else _counts[_element] = 1;
            }

            // Devolver el elemento que más se repite
            return _counts.OrderByDescending(c => c.Value).First().Key;
        }

        public static TResult GetMostCommon<TSource, TResult>(this IEnumerable<TSource> _group, Func<TSource, TResult> _selector) {
            // Diccionario para contar las ocurrencias de cada valor devuelto por el selector
            Dictionary<TResult, int> _counts = new();

            // Recorrer la colección y contar las ocurrencias de cada resultado del selector
            foreach (TSource _element in _group) {
                TResult _result = _selector(_element);

                if (_counts.ContainsKey(_result)) _counts[_result]++;
                else _counts[_result] = 1;
            }

            // Devolver el resultado (valor) que más se repite
            return _counts.OrderByDescending(c => c.Value).First().Key;
        }

        public static T SelectRandom<T>(this IEnumerable<T> _group, Func<T, bool> _selector) {
            if (_group.IsNullOrEmpty()) throw new InvalidOperationException("La secuencia no contiene elementos.");

            // Usar el índice aleatorio para seleccionar un elemento
            System.Random _random = new();
            int _count = _group.Count();
            int _index = _random.Next(0, _count);
            
            return _group.Where(_selector).ElementAt(_index);
        }
        #endregion
        
        #region Enums
        /// <summary> Devuelve 'true' si el enum contiene más de un valor </summary>
        public static bool MoreThanOneFlag<T>(this T _value) where T : Enum {
            return CustomUtils.CountBits(Convert.ToInt32(_value)) > 1;
        }

        public static T[] GetActiveFlags<T>(this T _value) where T : Enum {
            // Extraer todos los flags activados en una lista
            List<T> _activeFlags = new();

            foreach (T _flag in Enum.GetValues(typeof(T))) {
                if (_value.HasFlag(_flag) && _flag.ToString() != "None") _activeFlags.Add(_flag);
            }

            return _activeFlags.ToArray();
        }
        #endregion

        #region Scroll Rect
        /// <summary> Show up a scroll element </summary>
        public static void BringChildToView(this ScrollRect _scroll, RectTransform _child, float _offset = 1f) {
            /*
            Vector2 _viewportLocalPosition = _scroll.viewport.localPosition;
            Vector2 _childLocalPosition = _child.localPosition;

            _scroll.content.localPosition = new(
                0 - (_viewportLocalPosition.x + _childLocalPosition.x),
                0 - (_viewportLocalPosition.y + _childLocalPosition.y)
            );
            */

            Rect _rect = ((RectTransform)_scroll.transform).rect;
            float _itemPositionY = _scroll.transform.InverseTransformPoint(_child.position).y;
            float _halfItemHeight = _child.rect.height / 2f;

            var _lowerBound = _rect.yMax + _offset;
            var _upperBound = _rect.yMax - _offset;

            var _itemLowerBound = _itemPositionY - _halfItemHeight;
            var _itemUpperBound = _itemPositionY + _halfItemHeight;

            if (_itemLowerBound < _lowerBound) _scroll.content.anchoredPosition += new Vector2(0, _lowerBound - _itemLowerBound);
            else if (_itemUpperBound > _upperBound) _scroll.content.anchoredPosition += new Vector2(0, -(_itemUpperBound - _upperBound));
        }

        /// <summary> Show up a scroll element </summary>
        public static void BringChildToView(this ScrollRect _scroll, int _index, float _offset = 1) {
            _scroll.BringChildToView(_scroll.content.GetChild(_index).transform as RectTransform, _offset);
        }

        /// <summary> Check if scroll has childs </summary>
        public static bool HasChilds(this ScrollRect _scroll) => _scroll.content.childCount > 0;

        /// <summary> Elimina los childs del content </summary>
        public static void Clear(this ScrollRect _scroll) => _scroll.content.ClearChilds();

        public static void ScrollToLeft(this ScrollRect _scrollRect) => _scrollRect.normalizedPosition = new Vector2(1, 0);
        public static void ScrollToRight(this ScrollRect _scrollRect) => _scrollRect.normalizedPosition = new Vector2(0, 0);

        public static void ScrollToTop(this ScrollRect _scrollRect) => _scrollRect.normalizedPosition = new Vector2(0, 1);
        public static void ScrollToBottom(this ScrollRect _scrollRect) => _scrollRect.normalizedPosition = new Vector2(0, 0);
        #endregion
        
        #region Strings
        public static bool IsNullOrEmpty(this string _value) => _value == null || _value == "";
        public static bool EmailValidation(this string _value) {
            if (!_value.IsNullOrEmpty()) return Regex.IsMatch(_value, MatchEmailPattern);
            else return false;
        }
        #endregion

        #region Serialization
        /// <summary>
        /// Deserializa un objeto (normalmente un valor de Firebase) y devuelve su valor
        /// </summary>
        public static T DeserializeAndReturn<T>(this object _object) {
            if (_object is null) {
                throw new ArgumentNullException(nameof(_object), $"Object is null");
            }

            try {
                string _json = JsonConvert.SerializeObject(_object);
                return JsonConvert.DeserializeObject<T>(_json);

            } catch (Exception _exception) {
                Debug.LogError($"Error al deserializar: {_exception.Message}");
                return default;
            }
        }

        /// <summary>
        /// Deserializa un objeto (normalmente un valor de Firebase) y alimenta las variables de la clase correspondiente
        /// </summary>
        public static void DeserializeAndPopulate<T>(this object _object, T _target) {
            if (_object is null) {
                throw new ArgumentNullException(nameof(_object), $"'{nameof(_target)}' Object is null");
            }

            try {
                string _json = JsonConvert.SerializeObject(_object);
                JsonConvert.PopulateObject(_json, _target);

            } catch (Exception _exception) {
                Debug.LogError($"Error al deserializar: {_exception.Message}");
            }
        }

        /// <summary> Serializa un objeto y lo transforma en un array de bytes </summary>
        public static byte[] SerializeToBytes(this object _object) {
            BinaryFormatter _binaryFormatter = new();
            using MemoryStream _memoryStream = new();

            _binaryFormatter.Serialize(_memoryStream, _object);
            return _memoryStream.ToArray();
        }

        /// <summary> Deserializa un array de bytes y lo transforma en un objeto </summary>
        public static object DeserializeBytes(this byte[] _bytes) {
            BinaryFormatter _binaryFormatter = new();
            using MemoryStream _memoryStream = new();

            _memoryStream.Write(_bytes, 0, _bytes.Length);
            _memoryStream.Seek(0, SeekOrigin.Begin);

            return _binaryFormatter.Deserialize(_memoryStream);
        }
        #endregion

        #region Textures
        public static Texture ToTexture(this Sprite _sprite) {
            if (_sprite.rect.width == _sprite.texture.width) return _sprite.texture;
            else {
                Texture2D _texture2d = new Texture2D((int)_sprite.rect.width, (int)_sprite.rect.height);
                Color[] _colors = _sprite.texture.GetPixels(
                    (int)_sprite.textureRect.x,
                    (int)_sprite.textureRect.y,
                    (int)_sprite.textureRect.width,
                    (int)_sprite.textureRect.height
                );

                _texture2d.SetPixels(_colors);
                _texture2d.Apply();

                return _texture2d;
            }
        }

        public static byte[] ToPNG(this RenderTexture _texture) {
            Texture2D _texture2d = new(_texture.width, _texture.height, TextureFormat.RGBA32, false);
            RenderTexture _oldTexture = RenderTexture.active;
            
            RenderTexture.active = _texture;
            _texture2d.ReadPixels(new Rect(0, 0, _texture.width, _texture.height), 0, 0);
            _texture2d.Apply();
            RenderTexture.active = _oldTexture;

            byte[] _bytes = _texture2d.EncodeToPNG();
            GameObject.Destroy(_texture2d);

            return _bytes;
        }

        public static Texture2D ToTexture2D(this RenderTexture _texture) {
            Texture2D _texture2d = new(_texture.width, _texture.height, TextureFormat.RGB24, false);
            RenderTexture _oldTexture = RenderTexture.active;
            
            RenderTexture.active = _texture;
            _texture2d.ReadPixels(new Rect(0, 0, _texture.width, _texture.height), 0, 0);
            _texture2d.Apply();
            RenderTexture.active = _oldTexture;

            return _texture2d;
        }

        public static RenderTexture ToRenderTexture(this Texture2D _texture2d) {
            RenderTexture _renderTexture = new(_texture2d.width, _texture2d.height, 0) { enableRandomWrite = true };
            RenderTexture _oldTexture = RenderTexture.active;

            RenderTexture.active = _renderTexture;
            Graphics.Blit(_texture2d, _renderTexture);
            RenderTexture.active = _oldTexture;

            return _renderTexture;
        }
        #endregion
        
        #region Others
        /// <summary> Devuelve 'true' en el caso de que el action posea el método que se le comparta como parámetro </summary>
        public static bool IsRegistered(this Action _action, Delegate _handler) {
            foreach (Delegate _existingHandler in _action.GetInvocationList()) {
                if (_existingHandler == _handler) return true;
            }

            return false;
        }

        public static bool HasChilds(this Transform _transform) => _transform.childCount > 0;
        public static void ClearChilds(this Transform _transform) {
            if (_transform.childCount == 0) return;
            
            foreach (Transform _child in _transform) {
                UnityEngine.Object.Destroy(_child.gameObject);
            }
        }

        public static void SetInteractable(this CanvasGroup _canvasGroup, bool _state) {
            _canvasGroup.interactable = _state;
            _canvasGroup.blocksRaycasts = _state;
        }

        public static void SetActive(this CanvasGroup _canvasGroup, bool _state) {
            _canvasGroup.alpha = _state ? 1 : 0;
            _canvasGroup.interactable = _state;
            _canvasGroup.blocksRaycasts = _state;
        }

        public static string DisplayName(this Resolution _resolution) => $"{_resolution.width}x{_resolution.height} " + "(" + _resolution.refreshRateRatio.value.ToString("0") + ")";
        
        public static T ThrowIfNull<T>(this T _argument, string _argumentName) {
            if (_argument == null) throw new ArgumentNullException(_argumentName);
            return _argument;
        }

        public static bool TryFindIndex<TElement>(this TElement[] _arr, Predicate<TElement> _match, out int _index) {
            _arr.ThrowIfNull(nameof(_arr));
            _index = Array.FindIndex(_arr, _match);

            return _index >= 0;
        }

        public static bool TryFindIndex<TElement>(this TElement[] _arr, TElement _match, out int _index) {
            _arr.ThrowIfNull(nameof(_arr));

            for (int i = 0; i < _arr.Length; ++i) {
                if (_arr[i].Equals(_match)) {
                    _index = i;
                    return true;
                }
            }

            _index = -1;
            return false;
        }

        public static bool TryFindIndex<TElement>(this IReadOnlyList<TElement> _l, Predicate<TElement> _match, out int _index) {
            _l.ThrowIfNull(nameof(_l));

            for (int i = 0; i < _l.Count; ++i) {
                if (_match(_l[i])) {
                    _index = i;
                    return true;
                }
            }

            _index = -1;
            return false;
        }

        public static bool TryFindIndex<TElement>(this IReadOnlyList<TElement> _l, TElement _match, out int _index) {
            _l.ThrowIfNull(nameof(_l));

            for (int i = 0; i < _l.Count; ++i) {
                if (_l[i].Equals(_match)) {
                    _index = i;
                    return true;
                }
            }

            _index = -1;
            return false;
        }
        #endregion
    }
}
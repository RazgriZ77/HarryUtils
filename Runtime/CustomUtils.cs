using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace HarryUtils {
    public static class CustomUtils {
        #region Tasks
        public static Task<bool> WaitOneFrameAsync(this MonoBehaviour _monoBehaviour) {
            var _tcs = new TaskCompletionSource<bool>();
            _monoBehaviour.StartCoroutine(WaitOneFrame(_tcs));
            
            return _tcs.Task;
        }

        static IEnumerator WaitOneFrame(TaskCompletionSource<bool> _tcs) {
            yield return new WaitForEndOfFrame();
            _tcs.TrySetResult(true);
        }
        #endregion
        
        #region GPS
        public static bool CheckGPSAvailability() {
            if (!Input.location.isEnabledByUser) {
                return false;
            }

            return SystemInfo.supportsLocationService;
        }
        #endregion
        
        public static string TrimEmail(string _email) {
            return Regex.Replace(_email, @"[\.\$#\[\]/]", "_");
        }
        
        public static Color ColorFromHex(string _hex) {
            if (ColorUtility.TryParseHtmlString(_hex, out Color _color)) return _color;
            else return Color.white;
        }

        public static Color ColorFromHex(string _hex, float _alpha) {
            if (ColorUtility.TryParseHtmlString(_hex, out Color _color)) return new Color(_color.r, _color.g, _color.b, _alpha);
            else return Color.white;
        }
        
        public static float Distance(Vector2 _a, Vector2 _b) =>  (_b - _a).sqrMagnitude;
        
        public static void CanvasGroupState(ref CanvasGroup _canvasGroup, bool _state) {
            _canvasGroup.interactable = _state;
            _canvasGroup.blocksRaycasts = _state;
        }

        /// <summary> Devuelve una posición desde el punto de origen de un objeto sumándole un offset </summary>
        public static Vector3 PositionPlusOffset(Transform _transform, Vector2 _offset)
            => _transform.position + (_transform.right * _offset.x) + (_transform.up * _offset.y);

        public static Vector2 DirectionFromAngle(float _eulerY, float _angle) {
            _angle += _eulerY;
            return new Vector2(Mathf.Sin(_angle * Mathf.Deg2Rad), Mathf.Cos(_angle * Mathf.Deg2Rad));
        }

        public static bool CheckBetweenValues(float _value, float _min, float _max)
            => _value >= _min && _value <= _max;

        public static float ConvertBetweenScales(float _oldValue, float _firstMin, float _firstMax, float _secondMin, float _secondMax)
            => ((_oldValue - _firstMin) / (_firstMax - _firstMin) * (_secondMax - _secondMin)) + _secondMin;

        /// <summary> Sube un elemento en una lista </summary>
        public static void RaiseListElement<T>(int _order, List<T> _list, T _element) {
            int _index = _order - 1;
            if (_index < 0) _index = 0;

            _list.RemoveAt(_order);
            _list.Insert(_index, _element);
        }

        /// <summary> Baja un elemento en una lista </summary>
        public static void LowerListElement<T>(int _order, List<T> _list, T _element) {
            int _index = _order + 1;
            if (_index > _list.Count - 1) _index = _list.Count - 1;

            _list.RemoveAt(_order);
            _list.Insert(_index, _element);
        }

        /// <summary> Devuelve el valor que se le pasa como parámetro en positivo o negativo </summary>
        public static float GetRandomNegative(float _value) => (Random.Range(0,2) * 2 - 1) * _value;

        public static int CountBits(int _number) {
            int _count = 0;

            while (_number != 0) {
                _count += (_number & 1);
                _number >>= 1;
            }

            return _count;
        }
    }
}
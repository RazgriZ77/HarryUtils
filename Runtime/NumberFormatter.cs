using System;
using UnityEngine;

namespace HarryUtils {
    public static class NumberFormatter {
        // ==================== VARIABLES ===================
        #region Private Variables
        private static readonly string[] Abbreviation = new string[20] {
            "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "Oc", "No", "Dc",
            "Udc", "Ddc", "Tdc", "Qadc", "Qidc", "Sxdc", "Spdc", "Ocdc", "Nmdc", "Vg"
        };

        private static readonly string[] EnglishShortNames = new string[20] {
            "Million", "Billion", "Trillion", "Quadrillion", "Quintillion",
            "Sextillion", "Septillion", "Octillion", "Nonillion", "Decillion",
            "Undecillion", "Duodecillion", "Tredecillion", "Quattuordecillion", "Quindecillion",
            "Sexdecillion", "Septendecillion", "Octodecillion", "Novemdecillion", "Vigintillion"
        };

        private static readonly string[] SpanishShortNames = new string[20] {
            "Millones", "Billones", "Trillones", "Cuadrillones", "Quintillones",
            "Sixtillones", "Septillones", "Octillones", "Nonillones", "Decillones",
            "Undecillones", "Duodecillones", "Tredecillones", "Cuatrodecillones", "Quindecillones",
            "Seisdecillones", "Septidecillones", "Octodecillones", "Nonadecillones", "Vigintillones"
        };
        #endregion
        
        // ==================== METHODS ====================
        public static string ShortNotation(double _value) {
            int _zeroCount = (int)Math.Log10(_value);

            int _prefixIndex = _zeroCount / 3;
            _prefixIndex -= 2;

            if (_zeroCount < 6) return _value.ToString("0");
            else _prefixIndex = Mathf.Clamp(_prefixIndex, 0, Abbreviation.Length - 1);

            return (_value / Math.Pow(10, (_prefixIndex + 2) * 3)).ToString("0.00") + Abbreviation[_prefixIndex];
        }
    }
}
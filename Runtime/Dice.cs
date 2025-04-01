using System;

namespace HarryUtils {
    public class Dice {
        public enum Type { D4 = 4, D6 = 6, D8 = 8, D10 = 10, D12 = 12, D20 = 20 };
        public enum SortOrder { Ascending, Descending };

        // ==================== VARIABLES ===================
        #region Private Variables
        private const int MinimumDieValue = 1;
        #endregion

        // ==================== METHODS ====================
        public static int Roll(Type _dieType) {
            return GetRandomNumber(MinimumDieValue, (int)_dieType);
        }

        public static int Roll(Type _dieType, Unity.Mathematics.Random _random) {
            return GetRandomNumber(MinimumDieValue, (int)_dieType, _random);
        }

        public static int[] Roll(Type _dieType, int _numberOfDice, Unity.Mathematics.Random _random) {
            int[] _results = new int[_numberOfDice];

            for (int i = 0; i < _numberOfDice; i++) {
                _results[i] = Roll(_dieType, _random);
            }

            return _results;
        }

        public static int[] Roll(Type _dieType, int _numberOfDice, SortOrder _sortOrder, Unity.Mathematics.Random _random) {
            int[] results = Roll(_dieType, _numberOfDice, _random);
            results = Sort(results, _sortOrder);

            return results;
        }

        // =================================================
        
        private static int GetRandomNumber(int _min, int _max) => UnityEngine.Random.Range(_min, _max + 1);
        private static int GetRandomNumber(int _min, int _max, Unity.Mathematics.Random _random) => _random.NextInt(_min, _max + 1);

        private static int[] Sort(int[] _results, SortOrder _sortOrder) {
            switch (_sortOrder) {
                case SortOrder.Ascending: Array.Sort(_results, new Comparison<int>((i1, i2) => i1.CompareTo(i2)));
                break;

                case SortOrder.Descending: Array.Sort(_results, new Comparison<int>((i1, i2) => -i1.CompareTo(i2)));
                break;
            }

            return _results;
        }
    }
}
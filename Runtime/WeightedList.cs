using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace HarryUtils {
    /// <summary>
    /// This implements an algorithm for sampling from a discrete probability distribution via a generic list
    /// with extremely fast O(1) get operations and small (close to minimally small) O(n) space complexity and
    /// O(n) CRUD complexity. In other words, you can add any item of type T to a List with an integer weight,
    /// and get a random item from the list with probability ( weight / sum-weights ).
    /// </summary>
    public class WeightedList<T> : IEnumerable<T> {
        public enum WeightErrorHandlingType {
            SetWeightToOne, // Default
            ThrowExceptionOnAdd, // Throw exception for adding non-positive weight.
        }

        /// <summary>
        /// A single item for a list with matching T. Create one or more WeightedListItems, add to a Collection
        /// and Add() to the WeightedList for a single calculation pass.
        /// </summary>
        public readonly struct WeightedListItem<T1> {
            internal readonly T1 item;
            internal readonly int weight;

            public WeightedListItem(T1 _item, int _weight) {
                item = _item;
                weight = _weight;
            }
        }
        
        // ==================== VARIABLES ===================
        public WeightErrorHandlingType BadWeightErrorHandling { get; set; } = WeightErrorHandlingType.SetWeightToOne;

        public int TotalWeight => totalWeight;
        
        /// <summary> Minimum weight in the structure. 0 if Count == 0. </summary>
        public int MinWeight => minWeight;
        
        /// <summary> Maximum weight in the structure. 0 if Count == 0. </summary>
        public int MaxWeight => maxWeight;

        public T this[int index] => list[index];
        public int Count => list.Count;

        public uint RandomState => mathRandom.state;

        private readonly List<T> list = new();
        private readonly List<int> weights = new();
        private readonly List<int> probabilities = new();
        private readonly List<int> alias = new();

        private int totalWeight;
        private bool areAllProbabilitiesIdentical = false;
        private int minWeight;
        private int maxWeight;

        private Unity.Mathematics.Random mathRandom;

        // ==================== INICIO ====================

        /// <summary> Create a new WeightedList with an Unity.Mathematics.Random. </summary>
        public WeightedList(uint _seed, uint _state) {
            mathRandom = new(_seed) {
                state = _state
            };
        }

        /// <summary> Create a WeightedList with the provided items and a System.Random. </summary>
        public WeightedList(ICollection<WeightedListItem<T>> _listItems, uint _seed, uint _state) {
            mathRandom = new(_seed) {
                state = _state
            };
            
            foreach (WeightedListItem<T> _item in _listItems) {
                list.Add(_item.item);
                weights.Add(_item.weight);
            }

            Recalculate();
        }

        // ==================== METHODS ====================
        public T Next() {
            if (Count == 0) return default;

            int _nextInt = mathRandom.NextInt(Count);
            if (areAllProbabilitiesIdentical) return list[_nextInt];

            int _nextProbability = mathRandom.NextInt(totalWeight);
            return (_nextProbability < probabilities[_nextInt]) ? list[_nextInt] : list[alias[_nextInt]];
        }

        public void AddWeightToAll(int _weight) {
            if (_weight + minWeight <= 0 && BadWeightErrorHandling == WeightErrorHandlingType.ThrowExceptionOnAdd) throw new ArgumentException($"Subtracting {-1 * _weight} from all items would set weight to non-positive for at least one element.");

            for (int i = 0; i < Count; i++) {
                weights[i] = FixWeight(weights[i] + _weight);
            }

            Recalculate();
        }

        public void SubtractWeightFromAll(int _weight) => AddWeightToAll(_weight * -1);
        public void SetWeightOfAll(int _weight) {
            if (_weight <= 0 && BadWeightErrorHandling == WeightErrorHandlingType.ThrowExceptionOnAdd) throw new ArgumentException("Weight cannot be non-positive.");
            
            for (int i = 0; i < Count; i++) {
                weights[i] = FixWeight(_weight);
            }

            Recalculate();
        }

        public IReadOnlyList<T> Items => list.AsReadOnly();
        public IEnumerator<T> GetEnumerator() => list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();

        public void Add(T _item, int _weight) {
            list.Add(_item);
            weights.Add(FixWeight(_weight));

            Recalculate();
        }

        public void Add(ICollection<WeightedListItem<T>> _listItems) {
            foreach (WeightedListItem<T> _listItem in _listItems) {
                list.Add(_listItem.item);
                weights.Add(FixWeight(_listItem.weight));
            }

            Recalculate();
        }

        public void Clear() {
            list.Clear();
            weights.Clear();

            Recalculate();
        }

        public void Contains(T _item) => list.Contains(_item);
        public int IndexOf(T _item) => list.IndexOf(_item);
        public void Insert(int _index, T item, int _weight) {
            list.Insert(_index, item);
            weights.Insert(_index, FixWeight(_weight));

            Recalculate();
        }

        public void Remove(T _item) {
            int _index = IndexOf(_item);
            RemoveAt(_index);

            Recalculate();
        }

        public void RemoveAt(int _index) {
            list.RemoveAt(_index);
            weights.RemoveAt(_index);

            Recalculate();
        }

        public void SetWeight(T _item, int _newWeight) => SetWeightAtIndex(IndexOf(_item), FixWeight(_newWeight));
        public int GetWeightOf(T _item) => GetWeightAtIndex(IndexOf(_item));

        public void SetWeightAtIndex(int _index, int _newWeight) {
            weights[_index] = FixWeight(_newWeight);
            Recalculate();
        }

        public int GetWeightAtIndex(int _index) => weights[_index];

        public override string ToString() {
            StringBuilder _sb = new();

            _sb.Append("WeightedList<");
            _sb.Append(typeof(T).Name);
            _sb.Append(">: TotalWeight:");
            _sb.Append(TotalWeight);
            _sb.Append(", Min:");
            _sb.Append(minWeight);
            _sb.Append(", Max:");
            _sb.Append(maxWeight);
            _sb.Append(", Count:");
            _sb.Append(Count);
            _sb.Append(", {");

            for (int i = 0; i < list.Count; i++) {
                _sb.Append(list[i].ToString());
                _sb.Append(":");
                _sb.Append(weights[i].ToString());
                
                if (i < list.Count - 1) _sb.Append(", ");
            }

            _sb.Append("}");

            return _sb.ToString();
        }

        /// <summary> https://www.keithschwarz.com/darts-dice-coins/ </summary>
        private void Recalculate() {
            totalWeight = 0;
            areAllProbabilitiesIdentical = false;
            minWeight = 0;
            maxWeight = 0;

            bool _isFirst = true;

            alias.Clear(); // STEP 1
            probabilities.Clear(); // STEP 1

            List<int> _scaledProbabilityNumerator = new(Count);
            List<int> _small = new(Count); // STEP 2
            List<int> _large = new(Count); // STEP 2

            foreach (int _weight in weights) {
                if (_isFirst) {
                    minWeight = maxWeight = _weight;
                    _isFirst = false;
                }

                minWeight = (_weight < minWeight) ? _weight : minWeight;
                maxWeight = (maxWeight < _weight) ? _weight : maxWeight;
                totalWeight += _weight;

                _scaledProbabilityNumerator.Add(_weight * Count); // STEP 3 
                alias.Add(0);
                probabilities.Add(0);
            }

            // Degenerate case, all probabilities are equal.
            if (minWeight == maxWeight) {
                areAllProbabilitiesIdentical = true;
                return;
            }

            // STEP 4
            for (int i = 0; i < Count; i++) {
                if (_scaledProbabilityNumerator[i] < totalWeight) _small.Add(i);
                else _large.Add(i);
            }

            // STEP 5
            while (_small.Count > 0 && _large.Count > 0) {
                int _l = _small[^1]; // 5.1
                _small.RemoveAt(_small.Count - 1);

                int _g = _large[^1]; // 5.2
                _large.RemoveAt(_large.Count - 1);
                probabilities[_l] = _scaledProbabilityNumerator[_l]; // 5.3
                alias[_l] = _g; // 5.4

                int _tmp = _scaledProbabilityNumerator[_g] + _scaledProbabilityNumerator[_l] - totalWeight; // 5.5, even though using ints for this algorithm is stable
                _scaledProbabilityNumerator[_g] = _tmp;

                if (_tmp < totalWeight) _small.Add(_g); // 5.6 the large is now in the small pile
                else _large.Add(_g); // 5.7 add the large back to the large pile
            }

            // STEP 6
            while (_large.Count > 0) {
                int _g = _large[^1]; // 6.1

                _large.RemoveAt(_large.Count - 1);
                probabilities[_g] = totalWeight; //6.1
            }

            // STEP 7 - Can't happen for this implementation but left in source to match Keith Schwarz's algorithm
            #pragma warning disable S125 // Sections of code should not be commented out
            //while (small.Count > 0) {
            //    int l = small[^1]; // 7.1
            //    small.RemoveAt(small.Count - 1);
            //    _probabilities[l] = _totalWeight;
            //}
            #pragma warning restore S125 // Sections of code should not be commented out
        }

        // Adjust bad weights silently.
        internal static int FixWeightSetToOne(int _weight) => (_weight <= 0) ? 1 : _weight;

        // Throw an exception when adding a bad weight.
        internal static int FixWeightExceptionOnAdd(int _weight) => (_weight <= 0) ? throw new ArgumentException("Weight cannot be non-positive") : _weight;
        private int FixWeight(int _weight) => (BadWeightErrorHandling == WeightErrorHandlingType.ThrowExceptionOnAdd) ? FixWeightExceptionOnAdd(_weight) : FixWeightSetToOne(_weight);
    }
}
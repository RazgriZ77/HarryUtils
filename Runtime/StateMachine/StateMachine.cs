using System;
using System.Collections.Generic;
using UnityEngine;

namespace HarryUtils.StateMachineTool {
    public class StateMachine<TKey> {
        public interface IState {
            void OnEnter() { }
            void OnExit() { }
        }

        // ==================== VARIABLES ===================
        #region Public Variables
        public Action<TKey> onChangeState;

        public IState CurrentState => currentState;
        public TKey CurrentStateKey => currentStateKey;
        #endregion

        #region Private Variables
        private Dictionary<TKey, IState> states;

        private IState currentState;
        private TKey currentStateKey;
        #endregion

        // ==================== INICIO ====================
        public StateMachine() {
            states = new();
        }

        public StateMachine(Dictionary<TKey, IState> _states) : this() {
            AddStates(_states);
        }

        // ==================== METHODS ====================
        public void AddState(TKey _key, IState _state) {
            if (states.ContainsKey(_key)) Debug.LogWarning($"No se puede añadir el estado '{_key}' porque ya existe un estado con dicha key");
            else states.Add(_key, _state);
        }

        public StateMachine<TKey> AddStates<TValue>(Dictionary<TKey, TValue> _states) {
            foreach (KeyValuePair<TKey, TValue> _state in _states) {
                AddState(_state.Key, _state.Value as IState);
            }

            return this;
        }

        public void ChangeState(TKey _key) {
            if (!states.ContainsKey(_key)) Debug.LogError($"No se puede cambiar al estado '{_key}' porque no existe");
            else if (currentStateKey.Equals(_key)) return;
            else {
                currentState?.OnExit();

                currentState = states[_key];
                currentStateKey = _key;

                currentState.OnEnter();
                onChangeState?.Invoke(currentStateKey);
            }
        }
    }
}
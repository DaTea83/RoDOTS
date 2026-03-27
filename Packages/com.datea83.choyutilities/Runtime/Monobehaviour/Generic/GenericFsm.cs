using System;
using System.Collections.Generic;
using UnityEngine;

namespace EugeneC.Utilities {

    public abstract class GenericFsm<T, TEnum> : MonoBehaviour
        where T : GenericFsm<T, TEnum>
        where TEnum : Enum {

        public State currentState;
        [SerializeField] private State previousState;

        public Animator characterAnimator;
        private readonly Dictionary<TEnum, State> _allStates = new();

        private void Awake() {
            characterAnimator ??= GetComponentInChildren<Animator>();

            foreach (var state in GetComponentsInChildren<State>()) {
                _allStates.Add(state.StateEnum, state);
                state.Init((T)this);
            }

            currentState?.OnEnter();
        }

        protected virtual void Update() {
            currentState?.Run();
        }

        public void ChangeState(TEnum newStateID) {
            var newState = _allStates[newStateID];

            if (newState == currentState) return;
            if (currentState is not null && !currentState.CanTransitionTo(newStateID)) return;
            currentState?.OnExit();
            previousState = currentState;
            currentState = newState;
            currentState.OnEnter();
        }

        public void BacktoPreviousState() {
            if (previousState is not null) ChangeState(previousState.StateEnum);
        }

        public abstract class State : MonoBehaviour {

            protected T StateMachine;
            public abstract TEnum StateEnum { get; }

            protected T Parent {
                get {
                    StateMachine ??= GetComponentInParent<T>();

                    return StateMachine;
                }
            }

            public virtual void Init(T stateMachine) {
                StateMachine = stateMachine;
            }

            public virtual void OnEnter() { }

            public virtual void OnExit() { }

            public virtual void Run() { }

            public virtual bool CanTransitionTo(TEnum newStateID) {
                return true;
            }

        }

    }

}
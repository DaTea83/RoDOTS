#if ENABLE_INPUT_SYSTEM
using System;
using EugeneC.ECS;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EugeneC.Utilities {

    [DisallowMultipleComponent]
    public sealed class MouseGrabBridge : MonoBehaviour {

        [SerializeField] private InputActionAsset inputAction;
        [SerializeField] private InputActionReference mouseGrabAction;
        [SerializeField] private InputActionReference mousePositionAction;

        private float _currentInput;

        private Entity _entity;
        private EntityManager _entityManager;
        private float _previousInput;

        private async void Start() {
            try {
                if (inputAction is null) return;
                await Awaitable.EndOfFrameAsync();
                _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

                _entity = _entityManager
                    .CreateEntityQuery(typeof(MouseInputISingleton)).GetSingletonEntity();
            }
            catch (Exception e) {
                Debug.LogError(e);
            }
        }

        private void OnEnable() {
            if (inputAction is null) return;

            foreach (var map in inputAction.actionMaps)
            foreach (var a in map) {
                a.Enable();
#if UNITY_EDITOR
                Debug.Log("Enabled action: " + a.name);
#endif
            }

            mousePositionAction.action.performed += OnMouseMoved;
        }

        private void OnDisable() {
            if (inputAction is null) return;

            mousePositionAction.action.performed -= OnMouseMoved;

            foreach (var map in inputAction.actionMaps)
            foreach (var a in map) {
                a.Disable();
#if UNITY_EDITOR
                Debug.Log("Disabled action: " + a.name);
#endif
            }
        }

        private void OnMouseMoved(InputAction.CallbackContext obj) {
            _previousInput = _currentInput;

            if (_entity == Entity.Null) return;
            var isPressed = mouseGrabAction.action.ReadValue<float>();
            var pos = obj.ReadValue<Vector2>();

            var input = new MouseInputISingleton {
                CurrentInput = _currentInput = isPressed,
                PreviousInput = _previousInput,
                Position = pos
            };
            _entityManager.SetComponentData(_entity, input);
        }

    }

}
#endif
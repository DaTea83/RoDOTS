#if ENABLE_INPUT_SYSTEM
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using Object = UnityEngine.Object;

namespace EugeneC.Utilities {

    public class MultiInputSystem {

        private readonly string _controlScheme;
        private InputActionMap _actionMap;
        private IControlBinder _binder;
        public InputActionAsset Asset;
        public InputDevice Device;

        public InputUser User;

        public MultiInputSystem(InputDevice device, InputActionAsset asset, EControlSchemeEnum controltype) {
            User = InputUser.PerformPairingWithDevice(device);
            Device = device;
            Asset = Object.Instantiate(asset);
            _controlScheme = controltype.GetControlType();
        }

        public event Action<InputDevice, InputDeviceChange> OnBindObject;
        public event Action<InputDevice, InputDeviceChange> OnUnbindObject;

        public void EnableInput() {
            _actionMap.Enable();
        }

        public void DisableInput() {
            _actionMap.Disable();
        }

        public void BindObject<T>(T bindobject)
            where T : IControlBinder {
            if (_binder != null)
                UnbindObject();

            _binder = bindobject;
            _binder.Registry = this;

            var actionmapname =
                UtilityMethods.InterfaceToStringName(bindobject.InputInterface, "Actions", string.Empty);
            Debug.Log($"Finding action map name of {actionmapname}");
            _actionMap = Asset.FindActionMap(actionmapname);

            if (_actionMap == null) {
                Debug.LogError($"InputActionMap '{actionmapname}' not found in the InputActionAsset.");

                return;
            }

            User.AssociateActionsWithUser(_actionMap);
            User.ActivateControlScheme(_controlScheme);

            UtilityMethods.BindPlayerAction(_binder, _actionMap);
            EnableInput();

            OnBindObject?.Invoke(Device, InputDeviceChange.Added);
            _binder.OnBind();
        }

        public void UnbindObject() {
            _binder.Registry = null;
            _binder = null;

            foreach (var action in _actionMap)
                action.Reset();

            OnUnbindObject?.Invoke(Device, InputDeviceChange.Removed);
            DisableInput();
        }

    }

    public interface IControlBinder {

        public Type InputInterface { get; }
        MultiInputSystem Registry { get; set; }
        void OnBind();

    }

}
#endif
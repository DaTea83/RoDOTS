#if ENABLE_INPUT_SYSTEM
using System.Collections.Generic;
using EugeneC.Singleton;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EugeneC.Utilities {

// For objects that already existed
    public class InputManager : GenericSingleton<InputManager> {

        [SerializeField] private InputActionAsset actionAsset;
        [SerializeField] private int playerLimitCount = 3;
        [SerializeField] private bool allowNewJoin = true;
        [SerializeField] private bool allowKeyboard;

        private readonly Dictionary<InputDevice, MultiInputSystem> _deviceRegistry = new();
        private readonly Dictionary<MultiInputSystem, IControlBinder> _playerRegistry = new();

        public bool GetAllowKeyboard() {
            return allowKeyboard;
        }

        public bool RegisterPlayer(IControlBinder playerObject, InputDevice device) {
            if (!allowNewJoin) {
                Debug.LogWarning("New players are not allowed to join at this time.");

                return false;
            }

            if (_playerRegistry.Count > playerLimitCount) {
                Debug.LogWarning("Player limit reached. Cannot register new player.");

                return false;
            }

            if (_deviceRegistry.ContainsKey(device)) {
                Debug.LogWarning($"{device.displayName} is already registered.");

                return false;
            }

            var eControlScheme = UtilityMethods.GetDeviceType(device);
            var registry = new MultiInputSystem(device, actionAsset, eControlScheme);

            _deviceRegistry.Add(device, registry);
            _playerRegistry.Add(registry, playerObject);

            registry.BindObject(playerObject);
            Debug.Log($"Current player count: {_playerRegistry.Count}");

            return true;
        }

        public void UnregisterPlayer(IControlBinder playerObject) {
            MultiInputSystem registry = null;

            foreach (var entry in _playerRegistry) {
                if (entry.Value != playerObject) continue;
                registry = entry.Key;

                break;
            }

            if (registry != null) {
                registry.UnbindObject();
                _playerRegistry.Remove(registry);
                _deviceRegistry.Remove(registry.Device);
                Debug.Log($"Player with device {registry.Device} has been unregistered.");
            }
            else {
                Debug.LogWarning("Attempted to unregister a player that is not registered.");
            }
        }

        public void UnregisterAll() {
            foreach (var entry in _playerRegistry)
                entry.Key.UnbindObject();

            _playerRegistry.Clear();
            _deviceRegistry.Clear();

            Debug.Log("All players have been unregistered.");
        }

    }

}
#endif
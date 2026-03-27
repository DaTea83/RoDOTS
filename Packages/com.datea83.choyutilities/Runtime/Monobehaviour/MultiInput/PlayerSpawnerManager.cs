using System;
using System.Collections.Generic;
using EugeneC.Obsolete;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace EugeneC.Utilities {

// Combination of input manager and spawn manager

#pragma warning disable CS0612
    public class PlayerSpawnerManager : GenericSpawnManager<PlayerSpawnerManager.PlayerTypeEnum>
#pragma warning restore CS0612
    {

        public enum PlayerTypeEnum : byte {

            PlayerType1

        }

        [SerializeField] private InputActionAsset actionAsset;
        [SerializeField] private PlayerTypeEnum playerType;
        [SerializeField] private Transform defaultSpawnLocation;
        [SerializeField] private int playerLimitCount = 3;
        [SerializeField] private bool allowNewJoin = true;
        [SerializeField] private bool allowKeyboard;

        private readonly Dictionary<InputDevice, MultiInputSystem> _deviceRegistry = new();
        private readonly Dictionary<MultiInputSystem, IControlBinder> _playerRegistry = new();

        private void OnEnable() {
            InputSystem.onDeviceChange += OnDeviceChange;
            SceneManager.sceneLoaded += OnSceneLoaded;

            PlayerSpawnController.Instance.SubOnAllowNewPlayers(OnAllowNewJoinChange);
            PlayerSpawnController.Instance.SubOnAbleControls(AllInputControl);
            PlayerSpawnController.Instance.SubOnResetGame(ResetGame);

            CheckForExistingDevices();
        }

        protected override void OnDisable() {
            base.OnDisable();
            InputSystem.onDeviceChange -= OnDeviceChange;
            SceneManager.sceneLoaded -= OnSceneLoaded;

            PlayerSpawnController.Instance.UnsubOnAllowNewPlayers(OnAllowNewJoinChange);
            PlayerSpawnController.Instance.UnsubOnAbleControls(AllInputControl);
            PlayerSpawnController.Instance.UnsubOnResetGame(ResetGame);
        }

        private void OnAllowNewJoinChange(bool change) {
            allowNewJoin = change;
        }

        private void OnDeviceChange(InputDevice device, InputDeviceChange change) {
            if (change == InputDeviceChange.Added) {
                Debug.Log($"Device added: {device.displayName}");
                TryRegisterDevice(device);
            }
            else if (change == InputDeviceChange.Removed) {
                Debug.Log($"Device removed: {device.displayName}");
                UnregisterDevice(device);
            }
        }

        private void CheckForExistingDevices() {
            foreach (var device in InputSystem.devices) TryRegisterDevice(device);
        }

        private void TryRegisterDevice(InputDevice device) {
            if (!allowNewJoin) return;
            if (device is not Gamepad && (device is not Keyboard || !allowKeyboard)) return;
            if (_deviceRegistry.ContainsKey(device)) return;

            if (_playerRegistry.Count < playerLimitCount) {
                var playerObj = SpawnObject(playerType, defaultSpawnLocation.position,
                    Quaternion.identity);

                if (playerObj is null) return;
                var controlBinder = playerObj.GetComponent<IControlBinder>();

                if (controlBinder != null)
                    RegisterPlayer(controlBinder, device);
                else
                    Debug.LogError(
                        "Spawned object does not have a component that implements IControlBinder.");
            }
            else {
                Debug.Log("Player limit reached. Cannot register new player.");
            }
        }

        private void RegisterPlayer(IControlBinder playerBinder, InputDevice device) {
            var controlScheme = UtilityMethods.GetDeviceType(device);
            var registry = new MultiInputSystem(device, actionAsset, controlScheme);

            _deviceRegistry.Add(device, registry);
            _playerRegistry.Add(registry, playerBinder);

            registry.BindObject(playerBinder);
        }

        private void UnregisterDevice(InputDevice device) {
            if (!_deviceRegistry.TryGetValue(device, out var registry)) return;
            if (!_playerRegistry.TryGetValue(registry, out var playerBinder)) return;
            registry.UnbindObject();

            var binderBehaviour = playerBinder as MonoBehaviour;

            if (binderBehaviour is not null) {
                DespawnObject(binderBehaviour.gameObject);
                _playerRegistry.Remove(registry);
                _deviceRegistry.Remove(device);

                Debug.Log(
                    $"Unregistered player {playerBinder.GetType().Name} and removed device {device.displayName}");
            }
            else {
                Debug.LogError($"{playerBinder.GetType().Name} is not a MonoBehaviour. Cannot despawn object.");
            }
        }

        private void UnregisterAll() {
            foreach (var registry in _playerRegistry.Keys)
                registry.UnbindObject();

            foreach (var playerBinder in _playerRegistry.Values) {
                var binderBehaviour = playerBinder as MonoBehaviour;

                if (binderBehaviour is not null)
                    DespawnObject(binderBehaviour.gameObject);
            }

            _playerRegistry.Clear();
            _deviceRegistry.Clear();

            Debug.Log("All players have been unregistered.");
        }

        private void ResetGame(object sender, EventArgs e) {
            UnregisterAll();
            CheckForExistingDevices();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            Debug.Log($"Scene loaded: {scene.name}");
            ReinstantiatePlayers();
        }

        private void ReinstantiatePlayers() {
            var registries = new List<MultiInputSystem>(_playerRegistry.Keys);
            _playerRegistry.Clear();

            foreach (var registry in registries) {
                var playerObj = SpawnObject(PlayerTypeEnum.PlayerType1, Vector3.zero, Quaternion.identity);

                if (playerObj is not null) {
                    var controlBinder = playerObj.GetComponent<IControlBinder>();

                    if (controlBinder != null) {
                        registry.BindObject(controlBinder);
                        _playerRegistry.Add(registry, controlBinder);

                        Debug.Log(
                            $"Reinstantiated player {controlBinder.GetType().Name} and bound to device {registry.Device.displayName}");
                    }
                    else {
                        Debug.LogError("Spawned object does not have a component that implements IControlBinder.");
                    }
                }
                else {
                    Debug.LogError("Failed to spawn player object.");
                }
            }
        }

        private void AllInputControl(bool able) {
            foreach (var players in spawnedObjects) {
                var controlBinder = players.GetComponent<IControlBinder>();

                if (controlBinder == null) continue;

                if (able)
                    controlBinder.Registry.EnableInput();
                else
                    controlBinder.Registry.DisableInput();
            }
        }

    }

}
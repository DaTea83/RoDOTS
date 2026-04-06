using System;
using System.Collections.Generic;
using EugeneC.Mono;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

namespace EugeneC.Singleton {
    
    public abstract class GenericLegacyUIManager<TEnum, TMono> : GenericPoolingManager<TEnum, UiHelper, TMono>
        where TEnum : struct, Enum
        where TMono : MonoBehaviour {

        [SerializeField] protected Canvas canvasRef;

        private List<UiHelper> _openedUi;
        protected RectTransform CanvasPos => canvasRef.transform as RectTransform;

        public bool IsTransitioning { get; private set; }

        protected override async void Start() {
            try {
                await Awaitable.NextFrameAsync(Token);

                if (canvasRef is null) {
                    TryGetComponent(out canvasRef);
                    if (canvasRef is null)
                        throw new Exception(
                            "Canvas Reference is not set, please set it in the inspector or add it to the scene"
                        );
                }
                Pools = new ObjectPool<UiHelper>[poolAttributes.poolPrefabs.Length];
                RuntimePools = new RuntimePoolSerialize[poolAttributes.poolPrefabs.Length];

                for (var i = 0; i < poolAttributes.poolPrefabs.Length; i++) {
                    if (poolAttributes.poolPrefabs[i].prefab is null) continue;

                    //Set the length to 1 as there should be only one instance of that type of UI at a time
                    RuntimePools[i].spawn = new UiHelper[1];

                    var id = i;

                    // Initialize the pool
                    Pools[i] = InitPool(() => {
                        var spawn = Instantiate(poolAttributes.poolPrefabs[id].prefab, CanvasPos);
                        return spawn;
                    });
                }

                if (InitializeOnStart)
                    ForcedNewInstance();
            }
            catch (Exception e) {
                Debug.LogError(e);
            }
        }

        protected virtual void OnEnable() {
            _openedUi = ListPool<UiHelper>.Get();
        }

        protected override async void OnDisable() {
            try {
                await CloseAll();
                ListPool<UiHelper>.Release(_openedUi);
                base.OnDisable();
            }
            catch (Exception e) {
                print(e);
            }
        }

        public event Action OnOpenUi;
        public event Action OnCloseUi;

        protected virtual void ForcedNewInstance() {
            for (var i = 0; i < poolAttributes.poolPrefabs.Length; i++) {
                var spawnUi = Pools[i].Get();
                spawnUi.gameObject.transform.SetSiblingIndex(i);
                spawnUi.OnSpawn();
                RuntimePools[i].spawn[0] = spawnUi;
                spawnUi.gameObject.SetActive(false);
            }
        }

        public virtual async Awaitable<(UiHelper, bool)> Open(TEnum id) {
            var index = GetPoolIndex(id);

            if (index == -1) return (null, false);

            IsTransitioning = true;
            var newUi = RuntimePools[index].spawn[0];
            newUi.gameObject.SetActive(true);
            _openedUi.Add(newUi);

            OnOpenUi?.Invoke();
            var t = newUi.OnStartOpen();
            await Awaitable.WaitForSecondsAsync(math.abs(t), Token);
            newUi.OnEndOpen();
            IsTransitioning = false;

            return (newUi, true);
        }

        public virtual async Awaitable<(UiHelper, bool)> Close(TEnum id, float time) {
            var index = GetPoolIndex(id);

            if (index == -1) return (null, false);

            IsTransitioning = true;
            var newUi = RuntimePools[index].spawn[0];

            OnCloseUi?.Invoke();
            var t = newUi.OnStartClose();
            await Awaitable.WaitForSecondsAsync(math.abs(t), Token);
            newUi.OnEndClose();

            await Awaitable.NextFrameAsync();
            newUi.gameObject.SetActive(false);
            _openedUi.Remove(newUi);
            IsTransitioning = false;

            return (newUi, true);
        }

        public virtual async Awaitable<bool> CloseAll(bool forcedAbort = false) {
            IsTransitioning = true;
            var i = 0f;

            foreach (var ui in _openedUi) {
                OnCloseUi?.Invoke();
                var t = ui.OnStartClose();
                //Get the highest value and delay the said value
                i = i < t ? t : i;
            }

            if (forcedAbort) i = 0f;
            await Awaitable.WaitForSecondsAsync(i, Token);

            foreach (var ui in _openedUi) {
                ui.OnEndClose();
                if (!forcedAbort)
                    await Awaitable.NextFrameAsync(Token);
                ui.gameObject.SetActive(false);
            }

            IsTransitioning = false;
            _openedUi.Clear();

            return true;
        }

        public virtual async Awaitable<bool> Replace(TEnum id) {
            var c = await CloseAll();

            if (!c) return false;
            await Open(id);

            return true;
        }

    }

}
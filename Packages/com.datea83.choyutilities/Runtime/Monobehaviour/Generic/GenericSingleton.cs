using System;
using System.Threading;
using Unity.Entities;
using UnityEngine;

namespace EugeneC.Singleton {

    public abstract class GenericSingleton<T> : MonoBehaviour
        where T : MonoBehaviour {

        public static T Instance { get; private set; }

        protected virtual void Awake() {
            InitSingleton();
            GetWorld();
        }

        protected virtual void OnDisable() {
            CancelTask();
        }

        protected virtual void OnDestroy() {
            UnInitSingleton();
        }

        protected virtual void InitSingleton() {
            if (Instance is not null && Instance != this) {
                Destroy(gameObject);

                return;
            }

            Instance = (T)(MonoBehaviour)this;
        }

        protected virtual void UnInitSingleton() {
            if (Instance == this)
                Instance = null;
        }

        #region ECS

        private const ushort MAX_FRAME = 200;
        protected World World { get; private set; }

        private async void GetWorld() {
            try {
                var frame = 0;

                while (frame < MAX_FRAME) {
                    World = World.DefaultGameObjectInjectionWorld;

                    if (World is not null) break;
                    frame++;
                    Debug.Log($"{gameObject.name} is waiting for World at frame at {frame}");
                    await Awaitable.NextFrameAsync(Token);
                }

                if (World is null)
                    Debug.LogError("World is null, did you forget to add a subscene to the scene?");
            }
            catch (Exception e) {
                Debug.LogError(e);
            }
        }

        protected async Awaitable<TComponent> GetSingletonEntity<TComponent>()
            where TComponent : unmanaged, IComponentData {
            var query = World.EntityManager.CreateEntityQuery(
                ComponentType.ReadOnly<TComponent>());

            TComponent singleton = default;
            var validSingleton = false;
            var frame = 0;

            while (frame < MAX_FRAME) {
                validSingleton = query.TryGetSingleton(out singleton);

                if (validSingleton) break;
                Debug.Log($"{gameObject.name} is waiting for Singleton at frame at {frame}");
                frame++;
                await Awaitable.NextFrameAsync(Token);
            }

            return !validSingleton ? throw new Exception("Singleton not found") : singleton;
        }

        protected async Awaitable<DynamicBuffer<TBuffer>> GetSingletonBuffer<TBuffer>()
            where TBuffer : unmanaged, IBufferElementData {
            var query = World.EntityManager.CreateEntityQuery(
                ComponentType.ReadOnly<TBuffer>());

            DynamicBuffer<TBuffer> buffer = default;
            var validSingleton = false;
            var frame = 0;

            while (frame < MAX_FRAME) {
                validSingleton = query.TryGetSingletonBuffer(out buffer);

                if (validSingleton) break;
                Debug.Log($"{gameObject.name} is waiting for buffer at frame at {frame}");
                frame++;
                await Awaitable.NextFrameAsync(Token);
            }

            return !validSingleton ? throw new Exception("Buffer not found") : buffer;
        }

        #endregion

        #region AsyncCancellation

        private CancellationTokenSource _cts = new();
        protected CancellationToken Token => _cts.Token;
        protected event Action OnCancelTask;

        protected void CancelTask() {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
            OnCancelTask?.Invoke();
        }

        #endregion

    }

}
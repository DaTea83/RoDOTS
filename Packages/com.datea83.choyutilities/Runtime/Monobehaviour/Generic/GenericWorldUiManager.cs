using System;
using System.Collections.Generic;
using EugeneC.ECS;
using EugeneC.Mono;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Pool;
using BoxCollider = Unity.Physics.BoxCollider;

namespace EugeneC.Singleton {

    //TODO
    public abstract class GenericWorldUiManager<TEnum, TMono> : GenericPoolingManager<TEnum, UiHelper, TMono>
        where TEnum : Enum
        where TMono : MonoBehaviour {

        [SerializeField] protected Canvas canvasRef;
        [SerializeField] protected bool spawnEcsCollider;

        [HideInInspector] public bool isTransitioning;

        private List<UiHelper> _openedUi;

        protected RectTransform CanvasPos => canvasRef.transform as RectTransform;

        protected override async void Start() {
            try {
                await Awaitable.NextFrameAsync(Token);

                if (canvasRef is null) throw new Exception("Canvas is not set");
                CreateArchetype();
                _uiHandleSystem = World.GetExistingSystemManaged<UiHandleSystemBase>();

                Pools = new ObjectPool<UiHelper>[poolPrefabs.Length];
                RuntimePools = new RuntimePoolSerialize[poolPrefabs.Length];

                for (byte i = 0; i < Pools.Length; i++) {
                    if (poolPrefabs[i].prefab is null) continue;

                    RuntimePools[i].spawn = new UiHelper[1];
                    var id = i;

                    Pools[i] = InitPool(() => {
                        var spawn = Instantiate(poolPrefabs[id].prefab, CanvasPos);
                        spawn.OnSpawn();
                        spawn.SetId(id);
                        spawn.SetAndSubSystem(_uiHandleSystem);
                        CreateEntities(spawn.uiTransforms, id);
                        RuntimePools[id].spawn[0] = spawn;
                        spawn.gameObject.SetActive(false);

                        return spawn;
                    });

                    var spawnUi = Pools[i].Get();
                    RuntimePools[i].spawn[0] = spawnUi;
                }
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

        public virtual async Awaitable<(UiHelper, bool)> Open(TEnum id) {
            var index = GetPoolIndex(id);

            if (index == -1) return (null, false);

            isTransitioning = true;
            var newUi = RuntimePools[index].spawn[0];
            newUi.gameObject.SetActive(true);
            _openedUi.Add(newUi);

            OnOpenUi?.Invoke();
            var t = newUi.OnStartOpen();
            await Awaitable.WaitForSecondsAsync(math.abs(t), Token);
            newUi.OnEndOpen();
            isTransitioning = false;

            return (newUi, true);
        }

        public virtual async Awaitable<(UiHelper, bool)> Close(TEnum id, float time) {
            var index = GetPoolIndex(id);

            if (index == -1) return (null, false);

            isTransitioning = true;
            var newUi = RuntimePools[index].spawn[0];

            OnCloseUi?.Invoke();
            var t = newUi.OnStartClose();
            await Awaitable.WaitForSecondsAsync(math.abs(t), Token);
            newUi.OnEndClose();

            await Awaitable.NextFrameAsync();
            newUi.gameObject.SetActive(false);
            _openedUi.Remove(newUi);
            isTransitioning = false;

            return (newUi, true);
        }

        public virtual async Awaitable<bool> CloseAll() {
            isTransitioning = true;
            var i = 0f;

            foreach (var ui in _openedUi) {
                OnCloseUi?.Invoke();
                var t = ui.OnStartClose();
                //Get the highest value and delay the said value
                i = i < t ? t : i;
            }

            await Awaitable.WaitForSecondsAsync(i, Token);

            foreach (var ui in _openedUi) {
                ui.OnEndClose();
                await Awaitable.NextFrameAsync(Token);
                ui.gameObject.SetActive(false);
            }

            isTransitioning = false;
            _openedUi.Clear();

            return true;
        }

        public virtual async Awaitable<bool> Replace(TEnum id) {
            var c = await CloseAll();

            if (!c) return false;
            await Open(id);

            return true;
        }

        #region ECS

        private EntityArchetype _archetype;
        private UiHandleSystemBase _uiHandleSystem;

        private void CreateArchetype() {
            if (!spawnEcsCollider) return;

            _archetype = World.EntityManager.CreateArchetype(
                typeof(LocalTransform),
                typeof(PhysicsMass),
                typeof(PhysicsCollider),
                typeof(PhysicsVelocity),
                typeof(PhysicsGravityFactor),
                typeof(EntityTransformIData),
                typeof(UIData),
                typeof(UIBuffer));
        }

        private void CreateEntities(RectTransform[] uiTransforms, byte parentId) {
            if (!spawnEcsCollider) return;
            if (uiTransforms is null || uiTransforms.Length == 0) return;

            var manager = World.EntityManager;

            var gravity = new PhysicsGravityFactor {
                Value = 0
            };

            var mass = new PhysicsMass {
                InverseMass = 0,
                InverseInertia = float3.zero
            };

            for (var i = 0; i < uiTransforms.Length; i++) {
                var entity = manager.CreateEntity(_archetype);

                var rect = uiTransforms[i].rect;
                var lossyScale = uiTransforms[i].lossyScale;

                var size = new float3(
                    rect.width * lossyScale.x,
                    rect.height * lossyScale.y,
                    0.1f
                );

                using var box = BoxCollider.Create(new BoxGeometry {
                    Center = float3.zero,
                    Orientation = quaternion.identity,
                    Size = size,
                    BevelRadius = 0f
                });

                var col = new PhysicsCollider {
                    Value = box
                };

                var lt = new LocalTransform {
                    Position = uiTransforms[i].position,
                    Rotation = uiTransforms[i].rotation
                };

                var follow = new EntityTransformIData {
                    Transform = uiTransforms[i],
                    Offset = 0,
                    SmoothFollowSpeed = 0
                };

                var ui = new UIData {
                    ParentId = parentId,
                    OwnId = (byte)i
                };

                manager.SetComponentData(entity, gravity);
                manager.SetComponentData(entity, mass);
                manager.SetComponentData(entity, col);
                manager.SetComponentData(entity, lt);
                manager.SetComponentData(entity, follow);
                manager.SetComponentData(entity, ui);
            }
        }

        #endregion

    }

}
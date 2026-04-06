using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace EugeneC.Singleton {

    public abstract class GenericPoolingManager<TEnum, TObj, TMono> : GenericSingleton<TMono>
        where TEnum : struct, Enum
        where TObj : Component
        where TMono : MonoBehaviour {

        public abstract class PoolingAttributes : ScriptableObject {
            
            public InitialPoolSerialize[] poolPrefabs;
            [Serializable]
            public struct InitialPoolSerialize {

                public TObj prefab;
                public TEnum id;

            }
        }

        [Serializable]
        public struct RuntimePoolSerialize {

            public TObj[] spawn;
            public int currentIndex;
            public int previousIndex;

        }
        
        [SerializeField] protected PoolingAttributes poolAttributes;
        [SerializeField] protected byte poolCount = 32;
        protected List<int> PauseIndexes;

        protected ObjectPool<TObj>[] Pools;
        protected RuntimePoolSerialize[] RuntimePools;

        /// <summary>
        ///     Determines if you want to spawn all prefabs as child on start
        /// </summary>
        protected abstract bool InitializeOnStart { get; }

        /// <summary>
        ///     Turn off collection checking reduce gc pressure, but not thread safe
        /// </summary>
        protected abstract bool CollectionCheck { get; }

        protected virtual async void Start() {
            try {
                await Awaitable.NextFrameAsync(Token);
                Pools = new ObjectPool<TObj>[poolAttributes.poolPrefabs.Length];
                RuntimePools = new RuntimePoolSerialize[poolAttributes.poolPrefabs.Length];

                for (var i = 0; i < Pools.Length; i++) {
                    if (poolAttributes.poolPrefabs[i].prefab is null) continue;

                    RuntimePools[i].spawn = new TObj[poolCount];
                    Pools[i] = InitPool(poolAttributes.poolPrefabs[i].prefab);

                    if (!InitializeOnStart) continue;

                    for (var j = 0; j < poolCount; j++) {
                        var spawnObj = Pools[i].Get();
                        spawnObj.gameObject.transform.SetSiblingIndex(j);
                        RuntimePools[i].spawn[j] = spawnObj;
                    }
                }
            }
            catch (Exception e) {
                Debug.LogError(e);
            }
        }

        protected override void OnDestroy() {
            foreach (var pool in Pools) {
                pool?.Clear();
                pool?.Dispose();
            }

            base.OnDestroy();
        }

        protected virtual int GetPoolIndex(TEnum id) {
            if (!Enum.IsDefined(typeof(TEnum), id)) return -1;

            return Array.FindIndex(poolAttributes.poolPrefabs, i => EqualityComparer<TEnum>.Default.Equals(i.id, id));
        }

        protected virtual ObjectPool<TObj> InitPool(Component prefab) {
            return InitPool(() => {
                _ = Instantiate(prefab.gameObject, transform)
                    .TryGetComponent<TObj>(out var component)
                    ? component
                    : throw new Exception("Prefab must have a component of type " + typeof(TObj));

                return component;
            });
        }

        public virtual ObjectPool<TObj> InitPool(Func<TObj> createFunc) {
            var pool = new ObjectPool<TObj>(
                createFunc,
                obj => obj.gameObject.SetActive(true),
                obj => obj.gameObject.SetActive(false),
                Destroy,
                CollectionCheck,
                poolAttributes.poolPrefabs.Length,
                poolCount
            );

            return pool;
        }

    }

}
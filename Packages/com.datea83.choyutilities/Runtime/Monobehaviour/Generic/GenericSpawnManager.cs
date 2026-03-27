using System;
using System.Collections.Generic;
using EugeneC.Utilities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

namespace EugeneC.Singleton {

    public abstract class GenericSpawnManager<TEnum, TMono> : GenericPoolingManager<TEnum, Component, TMono>
        where TEnum : Enum
        where TMono : MonoBehaviour {

        private List<Component> _spawnedObjects;

        protected virtual void OnEnable() {
            _spawnedObjects = ListPool<Component>.Get();
        }

        protected override void OnDisable() {
            ListPool<Component>.Release(_spawnedObjects);
            base.OnDisable();
        }

        public virtual Component Spawn(TEnum id, float3 location = default, quaternion rotation = default) {
            var index = GetPoolIndex(id);

            if (index == -1) return null;

            var spawned = Pools[index].Get();
            spawned.transform.position = location;
            spawned.transform.rotation = rotation;
            _spawnedObjects.Add(spawned);

            return spawned;
        }

        public virtual Component SpawnInRandomSphere(TEnum id, float radius, float3 location,
            quaternion rotation = default) {
            var dir = this.RandomValue3() * radius;

            return Spawn(id, location + dir, rotation);
        }

        public virtual Component SpawnInRandomCircle(TEnum id, ETwoAxis axis, float radius, float3 location,
            quaternion rotation = default) {
            var value = this.RandomValue2() * radius;

            var dir = axis switch {
                ETwoAxis.XY => new float3(value.x, value.y, 0),
                ETwoAxis.XZ => new float3(value.x, 0, value.y),
                ETwoAxis.YZ => new float3(0, value.x, value.y),
                _ => throw new ArgumentOutOfRangeException(nameof(axis), axis, null)
            };

            return Spawn(id, location + dir, rotation);
        }

        public virtual void Despawn(Component component, TEnum id) {
            var index = GetPoolIndex(id);

            if (index == -1) return;

            var success = _spawnedObjects.Remove(component);

            if (!success) throw new Exception("spawn not found in spawned objects");
            Pools[index].Release(component);
        }

        public virtual void DespawnAll() {
            ListPool<Component>.Release(_spawnedObjects);

            foreach (var p in Pools) {
                p?.Dispose();
                p?.Clear();
            }

            _spawnedObjects = ListPool<Component>.Get();
        }

    }

}
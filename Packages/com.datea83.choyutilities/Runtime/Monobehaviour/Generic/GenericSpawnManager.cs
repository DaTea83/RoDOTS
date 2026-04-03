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

        private List<(Component obj, TEnum id)> _spawnedObjects;

        protected virtual void OnEnable() {
            _spawnedObjects = ListPool<(Component, TEnum)>.Get();
        }

        protected override void OnDisable() {
            DespawnAll();
            ListPool<(Component, TEnum)>.Release(_spawnedObjects);
            base.OnDisable();
        }

        public virtual Component Spawn(TEnum id, float3 location = default, quaternion rotation = default) {
            var index = GetPoolIndex(id);

            if (index == -1) return null;

            var spawned = Pools[index].Get();
            spawned.transform.position = location;
            spawned.transform.rotation = rotation;
            _spawnedObjects.Add((spawned, id));

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

            var i = _spawnedObjects.FindIndex(t => t.obj == component);

            if (i == -1) throw new Exception("spawn not found in spawned objects");
            _spawnedObjects.RemoveAt(i);
            Pools[index].Release(component);
        }

        public virtual void DespawnAll() {
            foreach (var (obj, id) in _spawnedObjects) {
                var index = GetPoolIndex(id);
                if (index != -1) Pools[index].Release(obj);
            }

            _spawnedObjects.Clear();
        }

    }

}
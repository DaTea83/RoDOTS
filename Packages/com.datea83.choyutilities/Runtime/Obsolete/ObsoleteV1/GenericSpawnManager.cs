using System;
using System.Collections.Generic;
using EugeneC.Singleton;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EugeneC.Obsolete {

    //TODO: Make a new version, mark this as deprecated 
    [Obsolete]
    public abstract class GenericSpawnManager<T> : GenericSingleton<GenericSpawnManager<T>>
        where T : Enum {

        public SpawnSerialize<T>[] serializedOb;

        public List<GameObject> spawnedObjects = new();
        private readonly Dictionary<T, SpawnSerialize<T>> _spawnDictionary = new();

        protected virtual void Start() {
            foreach (var spawnPrefab in serializedOb)
                _spawnDictionary[spawnPrefab.spawnId] = spawnPrefab;
        }

        public GameObject SpawnObject(T id, Vector3 pos, Quaternion rot) {
            GameObject newObj = null;

            if (!_spawnDictionary.TryGetValue(id, out var spawnPrefab)) return null;
            var key = Random.Range(0, spawnPrefab.prefab.Length);
            var c = spawnPrefab.prefab[key];
            newObj = Instantiate(c, pos, rot);
            spawnedObjects.Add(newObj);

            return newObj;
        }

        public void DespawnObject(GameObject despawnob) {
            if (!spawnedObjects.Contains(despawnob)) return;
            spawnedObjects.Remove(despawnob);
            Destroy(despawnob);
        }

        public void DespawnEverything() {
            foreach (var ob in spawnedObjects.ToArray())
                DespawnObject(ob);
        }

    }

    [Obsolete]
    [Serializable]
    public struct SpawnSerialize<T>
        where T : Enum {

        public T spawnId;
        public GameObject[] prefab;

    }

}
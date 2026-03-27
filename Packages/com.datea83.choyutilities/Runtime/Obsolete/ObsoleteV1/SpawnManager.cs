using System;
using System.Collections.Generic;
using EugeneC.Singleton;
using UnityEngine;

namespace EugeneC.Obsolete {

    [Obsolete]
    public class SpawnManager : GenericSingleton<SpawnManager> {

        public SpawnObject[] spawnObject;
        public List<GameObject> SpawnedObjects = new();
        private readonly Dictionary<SpawnEnum, SpawnObject> SpawnDictionary = new();

        protected override void Awake() {
            base.Awake();
            foreach (var SpawnPrefab in spawnObject) SpawnDictionary[SpawnPrefab.SpawnId] = SpawnPrefab;
        }

        public GameObject Spawning(SpawnEnum spawnid, Vector3 pos, Quaternion rot) {
            GameObject newobject = null;

            if (SpawnDictionary.TryGetValue(spawnid, out var SpawnPrefab)) {
                newobject = Instantiate(SpawnPrefab.Prefab, pos, rot);
                SpawnedObjects.Add(newobject);
            }

            return newobject;
        }

        public void DeSpawning(GameObject SpawnPrefab) {
            SpawnedObjects.Remove(SpawnPrefab);
            Destroy(SpawnPrefab);
        }

        public void DeSpawnAll() {
            foreach (var objects in SpawnedObjects.ToArray()) DeSpawning(objects);
        }

    }

    [Serializable]
    public class SpawnObject {

        public SpawnEnum SpawnId;
        public GameObject Prefab;

    }

    public enum SpawnEnum { }

}
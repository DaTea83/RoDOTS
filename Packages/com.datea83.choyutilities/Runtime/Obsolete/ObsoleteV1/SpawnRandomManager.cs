using System;
using System.Collections.Generic;
using EugeneC.Singleton;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EugeneC.Obsolete {

    [Obsolete]
    //Combined method of AudioManager and SpawnManager
    public class SpawnRandomManager : GenericSingleton<SpawnRandomManager> {

        public SpawnTerrain[] terrainPrefab;
        public List<GameObject> spawnedObstacles = new();
        private readonly Dictionary<TerrainType, SpawnTerrain> _terrainDictionary = new();

        protected override void Awake() {
            base.Awake();
            foreach (var terrain in terrainPrefab) _terrainDictionary[terrain.TerrainId] = terrain;
        }

        public GameObject ProceduralSpawn(TerrainType terrainID, Vector3 loc, Quaternion rot) {
            GameObject newObject = null;

            if (_terrainDictionary.TryGetValue(terrainID, out var terrain)) {
                var key = Random.Range(0, terrain.Prefab.Length);
                var c = terrain.Prefab[key];
                newObject = Instantiate(c, loc, rot);
                spawnedObstacles.Add(newObject);
            }

            return newObject;
        }

        public void RemoveThisPrefab(GameObject objects) {
            spawnedObstacles.Remove(objects);
            Destroy(objects);
        }

        public void RemoveAllTerrain() {
            foreach (var objects in spawnedObstacles.ToArray()) RemoveThisPrefab(objects);
        }

    }

    [Obsolete]
    [Serializable]
    public class SpawnTerrain {

        public TerrainType TerrainId;
        public GameObject[] Prefab;

    }

    [Obsolete]
    public enum TerrainType {

        Area1,
        Area2,
        Area3,
        Area4,
        Area5,

        FarView1 = 20,
        FarView2,
        FarView3,
        FarView4,
        FarView5,

        ObstacleSet1 = 40,
        ObstacleSet2,
        ObstacleSet3,
        ObstacleSet4,
        ObstacleSet5,

        Cloud1 = 60,
        Cloud2,
        Cloud3,
        Cloud4,
        Cloud5,

        DestroyCube1 = 80,
        DestroyCube2,
        DestroyCube3,
        DestroyCube4,
        DestroyCube5

    }

}
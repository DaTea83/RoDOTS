using EugeneC.Singleton;
using UnityEngine;

namespace RoDOTS.runtime {
    
    public class SpawnController : GenericSpawnManager<SpawnController.ESpawnType, SpawnController> {
        
        public enum ESpawnType {
            Robot1,
            Robot2,
        }

        protected override bool initializeOnStart => false;
        protected override bool collectionCheck => false;
    }
}
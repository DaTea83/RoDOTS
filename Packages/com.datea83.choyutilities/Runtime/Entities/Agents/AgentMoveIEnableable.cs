using Unity.Entities;

namespace EugeneC.ECS {

    public struct InitializeAgentIData : IComponentData {

        public Entity Spawn;
    }

    public struct AgentMoveIEnableable : IComponentData, IEnableableComponent {

        public Entity CurrentNode;
        public float CurrentRestTime;
    }
    
    public struct AgentMoveICleanupTag : ICleanupComponentData{ }

    public struct ConnectedNodeIBuffer : IBufferElementData {
        
        public Entity ConnectedNode;
        
    }

    public struct SpawnNodeIBuffer : IBufferElementData {

        public Entity Prefab;
    }
    
    public struct SpawnNodeIEnableable : IComponentData, IEnableableComponent {
        
        public bool SpawnOnce;
        public float DefaultSpawnDelay;
        public float CurrentSpawnDelay;
    }
    
    public struct AgentISingleton : IComponentData {
        
        public ushort TotalSpawnCount;
        public ushort CurrentSpawnCount;
        public ushort SpawnLimit;
    }
}
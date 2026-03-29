using Unity.Entities;

namespace EugeneC.ECS {

    public struct InitializeAgentIData : IComponentData {

        public Entity Spawn;
    }

    public struct AgentMoveIEnableable : IComponentData, IEnableableComponent {

        public Entity CurrentNode;
        public float CurrentRestTime;
    }

    public struct AgentMoveNodeIBuffer : IBufferElementData {
        
        public Entity ConnectedNode;
        
    }

    public struct AgentSpawnIBuffer : IBufferElementData {

        public Entity Prefab;
    }
    
    public struct AgentSpawnNodeIData : IComponentData {
        
        public bool SpawnOnce;
        public float DefaultSpawnDelay;
        public float CurrentSpawnDelay;
    }
    
    public struct AgentSpawnISingleton : IComponentData {
        
        public ushort TotalSpawnCount;
        public ushort CurrentSpawnCount;
        public ushort SpawnLimit;
    }
}
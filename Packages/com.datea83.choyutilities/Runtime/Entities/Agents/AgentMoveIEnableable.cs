using Unity.Entities;

namespace EugeneC.ECS {

    public struct InitializeAgentIData : IComponentData {

        public Entity Spawn;
        public float ExistTime;
    }

    public struct AgentMoveIEnableable : IComponentData, IEnableableComponent {

        public Entity CurrentNode;
        public float Speed;
        public float DefaultRestTime;
        public float CurrentRestTime;
        public float DotProduct;
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
        public float ExistTime;
    }

    public struct AgentMoveSystemISingleton : IComponentData {
        
        public float RotationSpeed;
        public float MinSpeed;
        public float MaxSpeed;
        public float MinRestTime;
        public float MaxRestTime;
        public bool HasRestTime;

    }
    
    public struct AgentSpawnISingleton : IComponentData {
        
        public ushort TotalSpawnCount;
        public ushort CurrentSpawnCount;
        public ushort SpawnLimit;
    }
}
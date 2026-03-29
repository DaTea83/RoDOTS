using EugeneC.ECS;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace RoDOTS.runtime
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(DestroyAuthoring))]
    public class RobotAuthoring : MonoBehaviour
    {
        [SerializeField] private byte teamId;
        
        private class Baker : Baker<RobotAuthoring> {
            public override void Bake(RobotAuthoring authoring) {
                var e = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(e, new RobotIData {
                    TeamId = authoring.teamId
                });
            }
        }
    }
    
    public struct RobotIData : IComponentData {
        public Entity ClosestEnemy;
        public float3 CurrentPosition;
        public float3 PreviousPosition;
        public ushort DefaultHealth;
        public ushort CurrentHealth;
        public byte TeamId;
        public bool EnemyInRange;
        public bool EnemyInSight;
    }
}

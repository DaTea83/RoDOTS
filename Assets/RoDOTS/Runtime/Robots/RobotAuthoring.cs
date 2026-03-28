using EugeneC.ECS;
using Unity.Entities;
using UnityEngine;

namespace RoDOTS.runtime
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(DestroyAuthoring))]
    public class RobotAuthoring : MonoBehaviour
    {
        private class Baker : Baker<RobotAuthoring> {
            public override void Bake(RobotAuthoring authoring) {
                
            }
        }
    }
}

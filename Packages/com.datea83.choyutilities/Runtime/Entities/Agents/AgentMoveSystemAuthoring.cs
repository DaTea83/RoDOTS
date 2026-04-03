using Unity.Entities;
using UnityEngine;

namespace EugeneC.ECS {

    [DisallowMultipleComponent]
    public class AgentMoveSystemAuthoring : MonoBehaviour {

        [SerializeField] private ushort spawnLimit;

        private class Baker : Baker<AgentMoveSystemAuthoring> {

            public override void Bake(AgentMoveSystemAuthoring authoring) {
                var e = GetEntity(TransformUsageFlags.None);

                AddComponent(e, new AgentISingleton {
                    SpawnLimit = authoring.spawnLimit
                });
            }

        }

    }

}
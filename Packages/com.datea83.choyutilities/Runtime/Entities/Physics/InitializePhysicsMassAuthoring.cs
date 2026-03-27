using Unity.Entities;
using UnityEngine;

namespace EugeneC.ECS {

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public class InitializePhysicsMassAuthoring : MonoBehaviour {

        private class InitializePhysicsMassAuthoringBaker : Baker<InitializePhysicsMassAuthoring> {

            public override void Bake(InitializePhysicsMassAuthoring authoring) {
                var e = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<InitializePhysicsMassDataITag>(e);
            }

        }

    }

}
using Unity.Entities;
using UnityEngine;

namespace EugeneC.ECS {

	/// <summary>
	/// If the entity is destroyed, destroy the animator
	/// </summary>
	[UpdateInGroup(typeof(Eu_DestroySystemGroup))]
	[UpdateAfter(typeof(DestroyEntityISystem))]
	public partial struct CleanupAnimatorISystem : ISystem {
        
		public void OnUpdate(ref SystemState state) {
			var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
        
			foreach (var (cleanup, entity) 
			         in SystemAPI.Query<RefRO<AnimatorICleanup>>()
				         .WithNone<ObjTransformIData>()
				         .WithEntityAccess()) {
            
				if (cleanup.ValueRO.Animator.Value is not null) {
					Object.Destroy(cleanup.ValueRO.Animator.Value.gameObject);
				}
				ecb.RemoveComponent<AnimatorICleanup>(entity);
			}
        
			ecb.Playback(state.EntityManager);
		}
	}

}
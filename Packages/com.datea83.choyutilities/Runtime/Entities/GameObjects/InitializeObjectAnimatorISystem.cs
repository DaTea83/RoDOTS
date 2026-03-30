using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace EugeneC.ECS {

	/// <summary>
	/// Add required components to the entity
	/// </summary>
	[UpdateInGroup(typeof(Eu_InitializationSystemGroup))]
	public partial struct InitializeObjectAnimatorISystem : ISystem {
        
		public void OnUpdate(ref SystemState state) {
			var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            
			foreach (var (obj, entity) in 
			         SystemAPI.Query<RefRO<GameObjectIData>>()
				         .WithAll<InitializeAnimatorITag>().WithEntityAccess()) {
                 
				var instance = Object.Instantiate(obj.ValueRO.Prefab.Value.gameObject);
				ecb.AddComponent(entity, new ObjTransformIData {
					Transform = instance.transform,
					SmoothFollowSpeed = 0,
					Offset = float3.zero
				});
				ecb.AddComponent(entity, new AnimatorICleanup {
					Animator = new UnityObjectRef<Animator> {
						Value = instance.GetComponent<Animator>()
					}
				});
				ecb.AddComponent(entity, new AnimatorIData {
					Animator = instance.GetComponent<Animator>(),
				});
                
				ecb.RemoveComponent<GameObjectIData>(entity);
				ecb.RemoveComponent<InitializeAnimatorITag>(entity);
			}
			ecb.Playback(state.EntityManager);
		}
	}

}
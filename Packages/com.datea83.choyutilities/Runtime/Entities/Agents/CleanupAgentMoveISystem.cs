using Unity.Entities;

namespace EugeneC.ECS {

	/// <summary>
	/// Notify singleton that the entity is destroyed
	/// </summary>
	[UpdateInGroup(typeof(Eu_DestroySystemGroup))]
	[UpdateAfter(typeof(DestroyEntityISystem))]
	public partial struct CleanupAgentMoveISystem : ISystem {

		public void OnCreate(ref SystemState state) {
			state.RequireForUpdate<AgentISingleton>();
		}

		public void OnUpdate(ref SystemState state) {
			var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
			var singleton = SystemAPI.GetSingleton<AgentISingleton>();

			foreach (var (cleanup, entity) in 
			         SystemAPI.Query<RefRO<AgentMoveICleanupTag>>()
				         .WithNone<AgentMoveIEnableable>().WithEntityAccess()) {
		        
				singleton.CurrentSpawnCount--;
				ecb.RemoveComponent<AgentMoveICleanupTag>(entity);
			}
	        
			SystemAPI.SetSingleton(singleton);
			ecb.Playback(state.EntityManager);
		}

	}

}
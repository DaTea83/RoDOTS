using Unity.Entities;
using UnityEngine;

namespace EugeneC.ECS {

	/// <summary>
	/// A substitute for event calling
	/// </summary>
	[DisallowMultipleComponent]
	public class GroupTagAuthoring : MonoBehaviour {

		[SerializeField] private byte idTag = byte.MaxValue;
		
		private class Baker : Baker<GroupTagAuthoring> {

			public override void Bake(GroupTagAuthoring authoring) {
				var e = GetEntity(TransformUsageFlags.None);
				AddComponent(e, new GroupTagIData { Id = authoring.idTag });
			}

		}

	}

	public struct GroupTagIData : IComponentData {

		public byte Id;
	}
}
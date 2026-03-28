using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace EugeneC.ECS {

    [DisallowMultipleComponent]
    public sealed class ObjFollowerAuthoring : MonoBehaviour {

        public Transform target;
        public float3 targetOffset;
        [Range(0f, 30f)] public float smoothFollowSpeed;

        internal class Baker : Baker<ObjFollowerAuthoring> {

            public override void Bake(ObjFollowerAuthoring authoring) {
                DependsOn(authoring.target);
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new ObjTransformIData {
                    Transform = new UnityObjectRef<Transform> {
                        Value = authoring.target
                    },
                    Offset = authoring.targetOffset,
                    SmoothFollowSpeed = authoring.smoothFollowSpeed
                });
            }

        }

    }

    /// <summary>
    /// GameObject follow entity transform
    /// </summary>
    public struct ObjTransformIData : IComponentData {

        public UnityObjectRef<Transform> Transform;
        public float3 Offset;
        public float SmoothFollowSpeed;
    }

    [UpdateInGroup(typeof(Eu_PostTransformSystemGroup), OrderFirst = true)]
    public partial struct ObjFollowerISystem : ISystem {

        public void OnUpdate(ref SystemState state) {
            var dt = SystemAPI.Time.DeltaTime;

            foreach (var (ltw, objTransformRef)
                     in SystemAPI.Query<RefRO<LocalToWorld>, RefRW<ObjTransformIData>>()) {
                var targetPos = ltw.ValueRO.Position + objTransformRef.ValueRO.Offset;
                var factor = objTransformRef.ValueRO.SmoothFollowSpeed > 0
                    ? objTransformRef.ValueRO.SmoothFollowSpeed * dt
                    : 1;
                var obj = objTransformRef.ValueRW.Transform.Value;

                obj.position = math.lerp(obj.position, targetPos, factor);
                obj.rotation = math.slerp(obj.rotation, ltw.ValueRO.Rotation, factor);
            }
        }

    }
}
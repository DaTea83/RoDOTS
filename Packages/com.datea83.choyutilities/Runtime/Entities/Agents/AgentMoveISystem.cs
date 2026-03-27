using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace EugeneC.ECS {

    [BurstCompile]
    [UpdateInGroup(typeof(Eu_PreTransformSystemGroup))]
    public partial struct AgentMoveISystem : ISystem {

        private const float DistanceThreshold = 0.1f;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<AgentMoveSystemISingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            
            new AgentMoveIJob {
                
                AgentMoveSingleton = SystemAPI.GetSingleton<AgentMoveSystemISingleton>(),
                NodeLookup = SystemAPI.GetBufferLookup<AgentMoveNodeIBuffer>(true),
                LtwLookup = SystemAPI.GetComponentLookup<LocalToWorld>(true),
                Time = SystemAPI.Time.DeltaTime,
                Threshold = DistanceThreshold,
                
            }.ScheduleParallel();
        }

        [BurstCompile]
        public partial struct AgentMoveIJob : IJobEntity {

            [ReadOnly] public AgentMoveSystemISingleton AgentMoveSingleton;
            [ReadOnly] public BufferLookup<AgentMoveNodeIBuffer> NodeLookup;
            [ReadOnly] public ComponentLookup<LocalToWorld> LtwLookup;
            public float Time;
            public float Threshold;
            
            [BurstCompile]
            private void Execute(ref AgentMoveIEnableable move, ref RandomIData random, 
                ref LocalTransform lt) {

                var target = LtwLookup[move.CurrentNode];

                if (math.distance(lt.Position, target.Position) > Threshold) {
                    var direction = math.normalize(target.Position - lt.Position);
                    lt.Position += direction * move.Speed * Time;
                }
                else {
                    if (AgentMoveSingleton.HasRestTime) {
                        move.CurrentRestTime += Time;
                        if(move.CurrentRestTime < move.DefaultRestTime) return;
                        move.CurrentRestTime = 0;
                    }

                    var node = NodeLookup[move.CurrentNode];
                    if (node.Length == 0) return;
                    //The random system initializes earlier than the spawn system, so for first frame there's no value for random component
                    if (random.Value.state == 0) return;
                    var index = random.Value.NextInt(0, node.Length);
                    var nextNode = node[index].ConnectedNode;

                    move.CurrentNode = nextNode;
                    var nodeLtw = LtwLookup[nextNode];
                    lt.Rotation = quaternion.LookRotation(nodeLtw.Position, lt.Up());
                }
            }
        }
    }

}
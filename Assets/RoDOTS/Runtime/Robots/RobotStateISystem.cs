using EugeneC.ECS;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace RoDOTS.runtime {
    
    [UpdateInGroup(typeof(Eu_PostTransformSystemGroup))]
    public partial struct RobotStateISystem : ISystem {
        
        private int _movement;
        private int _isDead;
        private int _isReload;
        private int _doAttack;
        
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<AgentSpawnISingleton>();
            
            _movement = Animator.StringToHash("F_Movement");
            _isDead = Animator.StringToHash("IsDead");
            _isReload = Animator.StringToHash("IsReload");
            _doAttack = Animator.StringToHash("DoAttack");
        }

        public void OnUpdate(ref SystemState state) {

            foreach (var (robot, move, stats,
                         anim, ltw) 
                     in SystemAPI.Query<RefRW<RobotIData>,RefRO<AgentMoveIEnableable>, RefRO<AgentStatsIData>
                         ,RefRW<AnimatorIData>, RefRO<LocalToWorld>>()) {
                
                robot.ValueRW.PreviousPosition = robot.ValueRW.CurrentPosition;
                robot.ValueRW.CurrentPosition = ltw.ValueRO.Position;
                
                var delta = robot.ValueRW.CurrentPosition - robot.ValueRW.PreviousPosition;
                var length = math.length(delta);
                var animator = anim.ValueRW.Animator.Value;
                
                animator.SetFloat(_movement, stats.ValueRO.MoveSpeed * length);
            }
        }
    }
}
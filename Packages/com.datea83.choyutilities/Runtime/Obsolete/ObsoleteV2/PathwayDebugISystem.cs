using System;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace EugeneC.ObsoleteV2 {

    [Obsolete]
    [WorldSystemFilter(WorldSystemFilterFlags.Editor)]
    public partial struct PathwayDebugISystem : ISystem {

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var path
                     in SystemAPI.Query<RefRO<AgentPathwaysIData>>()) {
                if (!path.ValueRO.Pathways.IsCreated) continue;

                ref var pathways = ref path.ValueRO.Pathways.Value;

                for (var i = 0; i < pathways.Length; i++)
                for (var j = 0; j < pathways[i].Path.Length; j++)
                for (var k = 0; k < pathways[i].Path[j].Position.Length - 1; k++)
                    Debug.DrawLine(pathways[i].Path[j].Position[k], pathways[i].Path[j].Position[k + 1],
                        Color.red);
            }
        }

    }

}
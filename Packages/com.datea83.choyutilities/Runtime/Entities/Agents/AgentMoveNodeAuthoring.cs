using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EugeneC.ECS {

    [DisallowMultipleComponent]
    public class AgentMoveNodeAuthoring : MonoBehaviour {

        [SerializeField] private AgentMoveNodeAuthoring[] connections;
        
#if UNITY_EDITOR        
        [Header("Gizmos")]
        [SerializeField] private Color connectionColor = Color.yellow;
        [SerializeField][Min(0.1f)] private float connectionWidth = 20f;
        [SerializeField] private Color nodeColor = Color.white;
        [SerializeField] private float nodeRadius = 1f;

        private void OnDrawGizmos() {

            Handles.color = nodeColor;
            Handles.SphereHandleCap(0, transform.position, Quaternion.identity, nodeRadius, EventType.Repaint);
            
            if (connections is null || connections.Length == 0) return;
            Handles.color = connectionColor;
            foreach (var c in connections) {
                if (c is null) continue;
                float3 target = c.transform.position;
                Handles.DrawAAPolyLine(connectionWidth, transform.position, target);
                
                // Draw Arrow
                var dir = math.normalize(target - (float3)transform.position);
                // Draw near endpoint
                var close = math.lerp(transform.position, target, 0.9f);
                var right = math.mul(math.mul(quaternion.LookRotation(dir, math.up()), quaternion.Euler(0, math.radians(150), 0)), math.forward());
                var left = math.mul(math.mul(quaternion.LookRotation(dir, math.up()), quaternion.Euler(0, math.radians(-150), 0)), math.forward());

                Handles.DrawAAPolyLine(connectionWidth * 1.2f, close, close + right);
                Handles.DrawAAPolyLine(connectionWidth * 1.2f, close, close + left);
            }
        }
#endif

        private class Baker : Baker<AgentMoveNodeAuthoring> {

            public override void Bake(AgentMoveNodeAuthoring authoring) {
                var e = GetEntity(TransformUsageFlags.Renderable);
                var buffer = AddBuffer<AgentMoveNodeIBuffer>(e);

                foreach (var c in authoring.connections) {
                    DependsOn(c);
                    buffer.Add(new AgentMoveNodeIBuffer { ConnectedNode = GetEntity(c, TransformUsageFlags.Renderable) });
                }
            }
        }
    }

}
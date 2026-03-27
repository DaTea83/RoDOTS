using System;
using Unity.Mathematics;
using UnityEngine;

namespace EugeneC.ObsoleteV2 {

    [Obsolete]
    [DisallowMultipleComponent]
    public sealed class AgentPoint : MonoBehaviour {

        public EAgentSpeed agentSpeed;
        public float overrideSpeed;
        public float pointThreshold = 0.2f;

        public float3 Position => transform.position;

        private void OnDrawGizmos() {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, pointThreshold);
        }

    }

    [Serializable]
    [Obsolete]
    public struct PointSerializable {

        public Color color;
        public EBakingLineType bakingLineType;
        public AgentPoint[] agentPoints;

    }

    public enum EBakingLineType : byte {

        Straight,
        Curved

    }

    public enum EAgentSpeed : byte {

        Legacy,
        Override

    }

}
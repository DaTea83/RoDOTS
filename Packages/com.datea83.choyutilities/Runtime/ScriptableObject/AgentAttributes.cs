using UnityEngine;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming

namespace EugeneC.Utilities {

    [CreateAssetMenu(fileName = "AgentAttributes", menuName = "Choy Utilities/Agents", order = 0)]
    public class AgentAttributes : ScriptableObject {

        [Min(0.01f)] public float MoveSpeed = 10f;
        [Min(0.01f)] public float RotationSpeed = 3f;
        public bool HasRestTime = true;
        [Min(0.01f)] public float RestTime = 1f;

        [Tooltip("0 and below means it doesn't despawn")] [Min(0f)]
        public float ExistTime;

    }

}
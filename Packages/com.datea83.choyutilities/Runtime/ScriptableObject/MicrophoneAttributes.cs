using UnityEngine;

namespace EugeneC.Utilities {

    [CreateAssetMenu(fileName = "Microphone", menuName = "Choy Utilities/Microphone")]
    public class MicrophoneAttributes : ScriptableObject {

        [Range(0, 300)] public int recMaxLength = 60;
        public bool loop = true;
        [Range(0, 48000)] public int frequency = 16000;

    }

}
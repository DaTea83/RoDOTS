using System.Collections.Generic;
using UnityEngine;

namespace EugeneC.Utilities {

    [CreateAssetMenu(fileName = "DialogueAttributes", menuName = "Choy Utilities/DialogueAttributes")]
    public class DialogueAttributes : ScriptableObject {

        [Min(0.05f)] public float pauseDurationEachLine = 1f;
        [Min(0.01f)] public float timePerCharacter = 0.01f;
        [TextArea] public List<string> dialogue;

    }

}
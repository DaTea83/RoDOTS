using UnityEngine;
using UnityEngine.Localization;

namespace EugeneC.Utility {

    [CreateAssetMenu(fileName = "NewBasicScriptable", menuName = "Choy Utilities/Basic")]
    public class BasicAttributes : ScriptableObject {

        public LocalizedString localizedName;
        public Sprite picture;

    }

}
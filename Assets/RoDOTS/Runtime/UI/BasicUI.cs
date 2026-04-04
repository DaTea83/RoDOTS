using EugeneC.Mono;
using UnityEngine;
using UnityEngine.UI;

namespace RoDOTS.runtime {
    
    public class BasicUI : UiHelper {
        
        [SerializeField] private Button settingsButton;
        
        public override void OnSpawn() {
            
        }
        
        public override float OnStartOpen() {
            return transitionTime;
        }
        
        public override void OnEndOpen() {
            
        }
        
        public override float OnStartClose() {
            return transitionTime;
        }
        
        public override void OnEndClose() {
            
        }
    }
}
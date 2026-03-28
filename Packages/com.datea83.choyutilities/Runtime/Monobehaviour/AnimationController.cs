using System.Collections.Generic;
using EugeneC.Singleton;
using UnityEngine;

namespace EugeneC.ECS {

    public class AnimationController : GenericSingleton<AnimationController> {

        [SerializeField] private string[] animationIds;
        public Dictionary<string, int> AnimationIDMap { get; private set; }

        private void OnEnable() {
            AnimationIDMap = new Dictionary<string, int>();
            
            foreach (var id in animationIds) {
                var hash = Animator.StringToHash(id);
                AnimationIDMap.Add(id, hash);
            }
        }
    }

}
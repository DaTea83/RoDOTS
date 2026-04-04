using EugeneC.Singleton;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace RoDOTS.runtime {
    
    public class AudioManager : GenericAudioManager<AudioManager.EAudioType, AudioManager> {

        public enum EAudioType : byte {
            
        }

        protected override bool InitializeOnStart => true;
        protected override bool CollectionCheck => false;
        
        protected override void Awake() {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }
        
        protected void OnEnable() {
            
            SceneManager.sceneLoaded += OnNewScene();
            SceneManager.sceneUnloaded += OnExitScene();
        }

        protected override void OnDisable() {
            SceneManager.sceneLoaded -= OnNewScene();
            SceneManager.sceneUnloaded -= OnExitScene();
            base.OnDisable();
        }

        private UnityAction<Scene, LoadSceneMode> OnNewScene() {
            GetWorld();
            return null;
        }
        
        private UnityAction<Scene> OnExitScene() {
            PauseAllClips();
            return null;
        }
    }
}
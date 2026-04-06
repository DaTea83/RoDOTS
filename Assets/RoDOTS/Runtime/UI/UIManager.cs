using EugeneC.Singleton;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace RoDOTS.runtime {
    public class UIManager : GenericLegacyUIManager<UIManager.EUiType, UIManager> {

        public enum EUiType : byte {
            Start0,
            Mono1,
            Pooling2,
            Object3,
            Authoring4,
            GpuAni5,
            
            Basic = 100,
            Settings
        }

        protected override bool InitializeOnStart => true;
        protected override bool CollectionCheck => false;

        protected override void Awake() {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

        protected override async void Start() {
            try {
                base.Start();
                await Awaitable.NextFrameAsync(Token);
                await Replace(EUiType.Basic);
            }
            catch (System.Exception e) {
                print(e);
            }
        }

        protected override void OnEnable() {
            base.OnEnable();
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
            _ = Open(EUiType.Basic);
            return null;
        }
        
        private UnityAction<Scene> OnExitScene() {
            _ = CloseAll(true);
            return null;
        }
    }
}
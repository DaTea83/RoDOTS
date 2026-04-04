using TMPro;
using UnityEngine;
using UnityEngine.Localization;

namespace EugeneC.Utilities {

    [AddComponentMenu("Eugene/FPS Counter")]
    [RequireComponent(typeof(TMP_Text))]
    public sealed class MonoFpsCounter : MonoBehaviour {

        [SerializeField] private TMP_Text displayText;
        [SerializeField] private LocalizedString fpsString;

        private float _frames;
        private float _time;

        private float FPS => _frames / _time;

        private void OnValidate() {
            displayText ??= GetComponent<TMP_Text>();
        }

        private void Start() {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 500;
            
            InvokeRepeating(nameof(UpdateText), .1f, .5f);
        }

        private void Update() {
            _time += Time.unscaledDeltaTime;
            _frames++;

            if (_time < 0.5f) return;
            _frames = 0;
            _time = 0;
        }

        private void UpdateText() {
            if (displayText is null) return;
            displayText.text = fpsString.GetLocalizedString($"{FPS:F1}");
        }

    }

}
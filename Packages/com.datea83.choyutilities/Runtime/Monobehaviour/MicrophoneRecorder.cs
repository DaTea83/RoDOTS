using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace EugeneC.Utilities {

    [AddComponentMenu("Eugene/Microphone Recorder")]
    public sealed class MicrophoneRecorder : MonoBehaviour {

        public MicrophoneAttributes settings;
        public float chunksLengthSec = 0.5f;
        public bool replay;

        public bool useVad = true;
        public float vadUpdateRateSec = 0.1f;
        public float vadContextSec = 30f;
        public float vadLastSec = 1.25f;
        public float vadThd = 1.0f;
        public float vadFreqThd = 100.0f;
        [CanBeNull] public Image vadIndicatorImage;

        public bool vadStop;
        public bool dropVadPart = true;
        public float vadStopTime = 3f;

        [CanBeNull] public TMP_Dropdown microphoneDropdown;
        public string microphoneDefaultLabel = "Default microphone";
        private int _chunksLength;
        private AudioClip _clip;
        private int _lastChunkPos;

        private int _lastVadPos;
        private int _latestMicPos;
        private float _length;
        private bool _madeLoopLap;

        private string _selectedMicDevice;
        private float? _vadStopBegin;

        public string SelectedMicDevice {
            get => _selectedMicDevice;
            set {
                if (value != null && !AvailableMicDevices.Contains(value))
                    throw new ArgumentException("Microphone device not found");
                _selectedMicDevice = value;
            }
        }

        public int ClipSamples => _clip.samples * _clip.channels;

        public string RecordStartMicDevice { get; private set; }
        public bool IsRecording { get; private set; }
        public bool IsVoiceDetected { get; private set; }

        public IEnumerable<string> AvailableMicDevices => Microphone.devices;

        private void Start() {
            if (microphoneDropdown is null) return;

            microphoneDropdown.options = AvailableMicDevices
                .Prepend(microphoneDefaultLabel)
                .Select(text => new TMP_Dropdown.OptionData(text))
                .ToList();

            microphoneDropdown.value = microphoneDropdown.options
                .FindIndex(op => op.text == microphoneDefaultLabel);

            microphoneDropdown.onValueChanged.AddListener(i => {
                if (microphoneDropdown is null) return;
                var opt = microphoneDropdown.options[i];
                SelectedMicDevice = opt.text == microphoneDefaultLabel ? null : opt.text;
            });
        }

        private void Update() {
            if (!IsRecording) return;

            //micPos, aka sample frames (sample rate * duration), increases during recording
            //when it reaches the end of the buffer, it starts from the beginning again
            //hence the condition is true
            var micPos = Microphone.GetPosition(RecordStartMicDevice);

            if (micPos < _latestMicPos) {
                //Announce that we made a full clip loop
                _madeLoopLap = true;

                if (!settings.loop) {
#if UNITY_EDITOR
                    Debug.Log($"Stopping recording, mic pos returned back to {micPos}");
#endif
                    StopRecording();

                    return;
                }

#if UNITY_EDITOR
                Debug.Log("Mic made a new loop lap, continue recording.");
#endif
            }

            _latestMicPos = micPos;

            UpdateChunks(micPos);
            UpdateVad(micPos);
        }

        public event Action<bool> OnVadChanged;
        public event Action<AudioChunk> OnChunkReady;
        public event Action<AudioChunk> OnRecordStop;

        private void UpdateChunks(int micPos) {
            if (OnChunkReady is null) return;

            // check if chunks length is valid
            if (_chunksLength <= 0) return;

            // get current chunk length
            var chunk = GetMicPosDist(_lastChunkPos, micPos);

            // send new chunks while there has valid size
            while (chunk > _chunksLength) {
                var origData = new float[_chunksLength];
                _clip.GetData(origData, _lastChunkPos);

                var chunkStruct = new AudioChunk {
                    Data = origData,
                    Frequency = _clip.frequency,
                    Channels = _clip.channels,
                    Length = chunksLengthSec,
                    IsVoiceDetected = IsVoiceDetected
                };
                OnChunkReady(chunkStruct);

                _lastChunkPos = (_lastChunkPos + _chunksLength) % ClipSamples;
                chunk = GetMicPosDist(_lastChunkPos, micPos);
            }
        }

        private void UpdateVad(int micPos) {
            if (!useVad) return;

            // get current recorded clip length
            var samplesCount = GetMicBufferLength(micPos);

            if (samplesCount <= 0) return;

            // check if it's time to update
            var vadUpdateRateSamples = vadUpdateRateSec * _clip.frequency;
            var dt = GetMicPosDist(_lastVadPos, micPos);

            if (dt < vadUpdateRateSamples) return;
            _lastVadPos = samplesCount;

            // try to get sample for voice detection
            var data = GetMicBufferLast(micPos, vadContextSec);
            var vad = MicrophoneCollections.SimpleVad(data, _clip.frequency, vadLastSec, vadThd, vadFreqThd);

            // raise event if vad has changed
            if (vad != IsVoiceDetected) {
                _vadStopBegin = !vad ? Time.realtimeSinceStartup : null;
                IsVoiceDetected = vad;
                OnVadChanged?.Invoke(vad);
            }

            // update vad indicator
            if (vadIndicatorImage) {
                var color = vad ? Color.green : Color.red;
                vadIndicatorImage.color = color;
            }

            UpdateVadStop();
        }

        private float[] GetMicBufferLast(int mPos, float lastSec) {
            var len = GetMicBufferLength(mPos);

            if (len == 0)
                return Array.Empty<float>();

            var lastSamples = (int)(_clip.frequency * lastSec);
            var dataLength = math.min(lastSamples, len);
            var offset = mPos - dataLength;
            if (offset < 0) offset = len + offset;

            var d = new float[dataLength];
            _clip.GetData(d, offset);

            return d;
        }

        private void UpdateVadStop() {
            if (!vadStop || _vadStopBegin == null) return;
            var passedTime = Time.realtimeSinceStartup - _vadStopBegin;

            if (!(passedTime > vadStopTime)) return;
            var dropTime = dropVadPart ? vadStopTime : 0f;
            StopRecording(dropTime);
        }

        public void StartRecording() {
            if (IsRecording) return;

            RecordStartMicDevice = SelectedMicDevice;
            _clip = Microphone.Start(RecordStartMicDevice, settings.loop, settings.recMaxLength, settings.frequency);
            IsRecording = true;

            _latestMicPos = 0;
            _madeLoopLap = false;
            _lastChunkPos = 0;
            _lastVadPos = 0;
            _vadStopBegin = null;
            _chunksLength = (int)(_clip.frequency * _clip.channels * chunksLengthSec);
        }

        public void StopRecording(float dropTimeSec = 0f) {
            if (!IsRecording)
                return;

            // get all data from mic audio clip
            var data = GetMicBuffer(dropTimeSec);

            var finalAudio = new AudioChunk {
                Data = data,
                Channels = _clip.channels,
                Frequency = _clip.frequency,
                IsVoiceDetected = IsVoiceDetected,
                Length = (float)data.Length / (_clip.frequency * _clip.channels)
            };

            // play echo sound
            if (replay) {
                var echoClip = AudioClip.Create("echo", data.Length,
                    _clip.channels, _clip.frequency, false);
                echoClip.SetData(data, 0);
                PlayClip(echoClip);
            }

            // stop mic audio recording
            Microphone.End(RecordStartMicDevice);
            IsRecording = false;
            Destroy(_clip);
#if UNITY_EDITOR
            Debug.Log("Stopped microphone recording. Final audio length " +
                      $"{finalAudio.Length} ({finalAudio.Data.Length} samples)");
#endif
            // update VAD, no speech with disabled mic
            if (IsVoiceDetected) {
                IsVoiceDetected = false;
                OnVadChanged?.Invoke(false);
            }

            // finally, fire event
            OnRecordStop?.Invoke(finalAudio);
        }

        //Can optimize
        private static void PlayClip(AudioClip clip) {
            var go = new GameObject("One shot audio") {
                transform = {
                    position = float3.zero
                }
            };
            var source = go.AddComponent<AudioSource>();
            source.clip = clip;
            source.spatialBlend = 1.0f;
            source.volume = 1;
            source.Play();
            Destroy(go, clip.length);
        }

        private float[] GetMicBuffer(float dropSec = 0f) {
            var micPos = Microphone.GetPosition(RecordStartMicDevice);
            var len = GetMicBufferLength(micPos);

            if (len == 0) return Array.Empty<float>();

            // drop last samples from length if necessary
            var dropTimeSamples = (int)(_clip.frequency * dropSec);
            len = math.max(0, len - dropTimeSamples);

            // get last len samples from recorded audio
            // offset used to get audio from previous circular buffer lap
            var d = new float[len];
            var offset = _madeLoopLap ? micPos : 0;
            _clip.GetData(d, offset);

            return d;
        }

        /// <summary>
        ///     Get mic buffer length that was actually recorded.
        /// </summary>
        private int GetMicBufferLength(int micPos) {
            // looks like we just started recording and stopped it immediately
            // nothing was actually recorded
            if (micPos == 0 && !_madeLoopLap) return 0;

            // get length of the mic buffer that we want to return
            // this need to account circular loop buffer
            var len = _madeLoopLap ? ClipSamples : micPos;

            return len;
        }

        /// <summary>
        ///     Calculate distance between two mic positions.
        ///     It takes circular buffer into account.
        /// </summary>
        private int GetMicPosDist(int prevPos, int newPos) {
            if (newPos >= prevPos)
                return newPos - prevPos;

            // circular buffer case
            return ClipSamples - prevPos + newPos;
        }

    }

}
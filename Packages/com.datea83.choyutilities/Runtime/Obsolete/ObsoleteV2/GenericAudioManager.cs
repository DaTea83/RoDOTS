using System;
using System.Collections.Generic;
using EugeneC.Singleton;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;

namespace EugeneC.ObsoleteV2 {

#if UNITY_2023_2_OR_NEWER

    // For audio randomization use the Unity new Random Audio Container, hence why there's a version requirement
    [Obsolete]
    public abstract class GenericAudioManager<TEnum, TMono> : GenericSingleton<TMono>
        where TEnum : Enum
        where TMono : MonoBehaviour {

        public enum EAudioPriority : byte {

            Highest = 0,
            UltraHigh = 1 << 0,
            VeryHigh = 1 << 1,
            High = 1 << 2,
            AboveAverage = 1 << 3,
            Average = 1 << 4,
            BelowAverage = 1 << 5,
            Low = 1 << 6,
            VeryLow = 1 << 7,
            Lowest = byte.MaxValue

        }

        public enum EMixerType : byte {

            Sfx = 1,
            Narration = 1 << 1,
            Music = 1 << 2

        }

        [SerializeField] protected AudioResourceSerialize[] audioResource;
        [SerializeField] protected AudioSource audioSourcePrefab;
        [SerializeField] protected byte poolCount = 32;
        [SerializeField] protected bool loop;
        [SerializeField] protected EAudioPriority priority = EAudioPriority.High;

        [Header("Audio Mixer Groups")] [SerializeField]
        protected AudioMixerSerialize masterAudioMixerGroup = new() {
            mixerName = "Master"
        };

        [SerializeField] protected AudioMixerSerialize sfxMixerGroup, narrationMixerGroup, musicMixerGroup;

        protected AudioSource[] AudioSources;
        protected int CurrentIndex;
        protected List<int> PauseIndexes;
        protected int PreviousIndex;

        protected virtual async void Start() {
            try {
                await Awaitable.NextFrameAsync(Token);

                if (audioSourcePrefab is null) throw new Exception("Audio Source Prefab is not set");

                if (masterAudioMixerGroup.mixerGroup is null)
                    throw new Exception("Master Audio Mixer Group is not set");

                AudioSources = new AudioSource[poolCount];

                for (var i = 0; i < poolCount; i++) {
                    var spawnAudio = Instantiate(audioSourcePrefab, transform);
                    spawnAudio.loop = loop;
                    spawnAudio.outputAudioMixerGroup = masterAudioMixerGroup.mixerGroup;
                    spawnAudio.priority = (int)priority;
                    AudioSources[i] = spawnAudio;
                }
            }
            catch (Exception e) {
                Debug.LogException(e);
            }
        }

        public virtual float PlayClipAtPos(TEnum id, float3 pos, byte audioPriority = (byte)EAudioPriority.Average) {
            if (!Enum.IsDefined(typeof(TEnum), id)) return 0f;

            var resourceIndex = Array.FindIndex(audioResource,
                r => EqualityComparer<TEnum>.Default.Equals(r.id, id));

            var resource = audioResource[resourceIndex].audio;

            return PlayClipAtPos(resource, pos, masterAudioMixerGroup.mixerGroup, audioPriority);
        }

        public virtual float PlayClipAtPos(AudioResource resource,
            float3 pos,
            AudioMixerGroup channel = null,
            byte audioPriority = (byte)EAudioPriority.Average) {
            var currentSource = AudioSources[CurrentIndex];

            currentSource.transform.localPosition = pos;
            currentSource.outputAudioMixerGroup = channel ?? masterAudioMixerGroup.mixerGroup;
            currentSource.resource = resource;
            currentSource.priority = audioPriority;
            currentSource.Play();

            var lengthSeconds = currentSource.clip?.length ?? 0f;

            PreviousIndex = CurrentIndex;
            CurrentIndex++;
            CurrentIndex %= AudioSources.Length;

            return lengthSeconds;
        }

        public virtual float PlayClipAtPos(TEnum id,
            float3 pos,
            EMixerType mixerType,
            byte audioPriority = (byte)EAudioPriority.Average) {
            if (!Enum.IsDefined(typeof(TEnum), id)) return 0f;

            var resourceIndex = Array.FindIndex(audioResource,
                r => EqualityComparer<TEnum>.Default.Equals(r.id, id));

            var resource = audioResource[resourceIndex].audio;

            return PlayClipAtPos(resource, pos, mixerType, audioPriority);
        }

        public virtual float PlayClipAtPos(AudioResource resource,
            float3 pos,
            EMixerType mixerType,
            byte audioPriority = (byte)EAudioPriority.Average) {
            var channel = mixerType switch {
                EMixerType.Sfx => sfxMixerGroup.mixerGroup ?? masterAudioMixerGroup.mixerGroup,
                EMixerType.Narration => narrationMixerGroup.mixerGroup ?? masterAudioMixerGroup.mixerGroup,
                EMixerType.Music => musicMixerGroup.mixerGroup ?? masterAudioMixerGroup.mixerGroup,
                _ => throw new ArgumentOutOfRangeException(nameof(mixerType), mixerType, null)
            };

            var currentSource = AudioSources[CurrentIndex];

            currentSource.transform.localPosition = pos;
            currentSource.outputAudioMixerGroup = channel;
            currentSource.resource = resource;
            currentSource.priority = audioPriority;
            currentSource.Play();

            var lengthSeconds = currentSource.clip?.length ?? 0f;

            PreviousIndex = CurrentIndex;
            CurrentIndex++;
            CurrentIndex %= AudioSources.Length;

            return lengthSeconds;
        }


        //TODO
        public virtual async Awaitable<float> PlayFocusClip(TEnum id,
            float3 pos,
            EMixerType mixerType,
            byte audioPriority = (byte)EAudioPriority.Average) {
            var curve = mixerType switch {
                EMixerType.Sfx => sfxMixerGroup.volumeCurve ?? masterAudioMixerGroup.volumeCurve,
                EMixerType.Narration => narrationMixerGroup.volumeCurve ?? masterAudioMixerGroup.volumeCurve,
                EMixerType.Music => musicMixerGroup.volumeCurve ?? masterAudioMixerGroup.volumeCurve,
                _ => throw new ArgumentOutOfRangeException(nameof(mixerType), mixerType, null)
            };

            if (!Enum.IsDefined(typeof(TEnum), id)) return 0f;

            var resourceIndex = Array.FindIndex(audioResource,
                r => EqualityComparer<TEnum>.Default.Equals(r.id, id));

            var resource = audioResource[resourceIndex].audio;
            var delay = PlayClipAtPos(resource, pos, mixerType, audioPriority);

            var time = 0f;
            while (time < delay) time += Time.deltaTime;

            await Awaitable.WaitForSecondsAsync(delay, Token);

            return delay;
        }

        public virtual float PlayClip(TEnum id, byte audioPriority = (byte)EAudioPriority.Average) {
            return PlayClipAtPos(id, float3.zero, audioPriority);
        }

        public virtual float PlayClip(AudioResource resource, byte audioPriority = (byte)EAudioPriority.Average) {
            return PlayClipAtPos(resource, float3.zero, masterAudioMixerGroup.mixerGroup, audioPriority);
        }

        public virtual bool StopClip(int idx = -1) {
            idx = idx == -1 ? PreviousIndex : idx;
            var source = AudioSources[idx];

            if (!source.isPlaying) return false;
            source.Stop();

            return true;
        }

        public virtual bool PauseAllClips(bool isStop = false) {
            PauseIndexes = new List<int>();

            for (var i = 0; i < AudioSources.Length; i++) {
                var currentSource = AudioSources[i];

                if (!currentSource.isPlaying) continue;

                if (!isStop)
                    currentSource.Pause();
                else
                    currentSource.Stop();
                PauseIndexes.Add(i);
            }

            return PauseIndexes.Count == AudioSources.Length;
        }

        public virtual bool ResumeClips() {
            if (PauseIndexes is null) return false;

            foreach (var index in PauseIndexes)
                AudioSources[index].Play();

            PauseIndexes.Clear();

            return true;
        }

        [Serializable]
        public struct AudioResourceSerialize {

            public AudioResource audio;
            public TEnum id;

        }

        [Serializable]
        public struct AudioMixerSerialize {

            public AudioMixerGroup mixerGroup;
            public AnimationCurve volumeCurve;
            public string mixerName;
            [Range(-80f, 20f)] public float defaultVolume, focusedVolume, unfocusedVolume;

        }

    }

#endif

}
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

namespace EugeneC.Singleton {

    public abstract class GenericAudioManager<TEnum, TMono> : GenericSingleton<TMono>
        where TEnum : struct, Enum
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

        public abstract class PoolingAttributes : ScriptableObject {
            
            public AudioResourceSerialize[] audioResource;
            [Serializable]
            public struct AudioResourceSerialize {

                public AudioResource audio;
                public TEnum id;

            }
        }

        [Serializable]
        public struct AudioMixerSerialize {

            public AudioMixer mixer;
            public AnimationCurve volumeCurve;
            public string mixerName;
            [Range(-80f, 20f)] public float defaultVolume, focusedVolume, unfocusedVolume;

        }

        [SerializeField] protected PoolingAttributes poolAttributes;
        [SerializeField] protected AudioSource audioSourcePrefab;
        [SerializeField] protected byte poolCount = 32;
        [SerializeField] protected bool loop;
        [SerializeField] protected EAudioPriority priority = EAudioPriority.High;

        [Header("Audio Mixer Groups")] [SerializeField]
        protected AudioMixerSerialize masterAudioMixerGroup = new() { mixerName = "Master" };

        [SerializeField] protected AudioMixerSerialize sfxMixerGroup, narrationMixerGroup, musicMixerGroup;

        protected ObjectPool<AudioSource> Pool;
        protected AudioSource[] AudioSources;
        protected List<int> PauseIndexes;
        
        private byte _previousIndex;
        private byte _currentIndex;
        
        protected virtual async void Start() {
            try {
                await Awaitable.NextFrameAsync(Token);

                if (audioSourcePrefab is null) throw new Exception("Audio Source Prefab is not set");
                AudioSources = new AudioSource[poolCount];

                Pool = new ObjectPool<AudioSource>(
                    () => Instantiate(audioSourcePrefab, transform),
                    obj => obj.gameObject.SetActive(true),
                    obj => obj.gameObject.SetActive(false),
                    Destroy,
                    false,
                    poolCount,
                    poolCount << 1);
                
                for (var i = 0; i < poolCount; i++) {
                    var spawnAudio = Pool.Get();
                    spawnAudio.gameObject.transform.SetSiblingIndex(i);
                    spawnAudio.loop = loop;
                    spawnAudio.outputAudioMixerGroup = masterAudioMixerGroup.mixer.outputAudioMixerGroup;
                    spawnAudio.priority = (int)priority;
                    AudioSources[i] = spawnAudio;
                }

                masterAudioMixerGroup.mixer.SetFloat(masterAudioMixerGroup.mixerName,
                    masterAudioMixerGroup.defaultVolume);
                sfxMixerGroup.mixer?.SetFloat(sfxMixerGroup.mixerName, sfxMixerGroup.defaultVolume);
                narrationMixerGroup.mixer?.SetFloat(narrationMixerGroup.mixerName, narrationMixerGroup.defaultVolume);
                musicMixerGroup.mixer?.SetFloat(musicMixerGroup.mixerName, musicMixerGroup.defaultVolume);
            }
            catch (Exception e) {
                Debug.LogError(e);
            }
        }

        protected virtual void OnEnable() {
            PauseIndexes = ListPool<int>.Get();
        }

        protected override void OnDisable() {
            ListPool<int>.Release(PauseIndexes);
            base.OnDisable();
        }

        protected virtual int GetPoolIndex(TEnum id) {
            if (!Enum.IsDefined(typeof(TEnum), id)) return -1;

            return Array.FindIndex(poolAttributes.audioResource, i => EqualityComparer<TEnum>.Default.Equals(i.id, id));
        }

        public virtual float PlayClipAtPos(TEnum id, float3 pos, byte audioPriority = (byte)EAudioPriority.Average) {
            var index = GetPoolIndex(id);

            if (index == -1) throw new Exception("Audio Resource not found");

            var resource = poolAttributes.audioResource[index].audio;

            return PlayClipAtPos(resource, pos, masterAudioMixerGroup.mixer.outputAudioMixerGroup, audioPriority);
        }

        public virtual float PlayClipAtPos(AudioResource resource, float3 pos, AudioMixerGroup channel = null,
            byte audioPriority = (byte)EAudioPriority.Average) {
            var currentSource = AudioSources[_currentIndex];

            currentSource.transform.localPosition = pos;
            currentSource.outputAudioMixerGroup = channel;
            currentSource.resource = resource;
            currentSource.priority = audioPriority;
            currentSource.Play();

            var lengthSeconds = currentSource.clip?.length ?? 0f;

            _previousIndex = _currentIndex;
            _currentIndex++;
            _currentIndex %= (byte)AudioSources.Length;

            return lengthSeconds;
        }

        public virtual float PlayClipAtPos(TEnum id, float3 pos, EMixerType mixerType,
            byte audioPriority = (byte)EAudioPriority.Average) {
            var index = GetPoolIndex(id);

            if (index == -1) throw new Exception("Audio Resource not found");

            var resource = poolAttributes.audioResource[index].audio;

            return PlayClipAtPos(resource, pos, mixerType, audioPriority);
        }

        private AudioMixerSerialize GetMixerType(EMixerType mixerType) {
            return mixerType switch {
                EMixerType.Sfx => sfxMixerGroup,
                EMixerType.Narration => narrationMixerGroup,
                EMixerType.Music => musicMixerGroup,
                _ => masterAudioMixerGroup
            };
        }

        public virtual float PlayClipAtPos(AudioResource resource, float3 pos, EMixerType mixerType,
            byte audioPriority = (byte)EAudioPriority.Average) {
            var channel = GetMixerType(mixerType).mixer;
            var currentSource = AudioSources[_currentIndex];

            currentSource.transform.localPosition = pos;
            currentSource.outputAudioMixerGroup = channel.outputAudioMixerGroup;
            currentSource.resource = resource;
            currentSource.priority = audioPriority;
            currentSource.Play();

            var lengthSeconds = currentSource.clip?.length ?? 0f;

            _previousIndex = _currentIndex;
            _currentIndex++;
            _currentIndex %= (byte)AudioSources.Length;

            return lengthSeconds;
        }

        public virtual async Awaitable<float> PlayFocusClip(TEnum id, float3 pos, EMixerType mixerType,
            byte audioPriority = (byte)EAudioPriority.Average) {
            var index = GetPoolIndex(id);

            if (index == -1) return 0f;

            var resource = poolAttributes.audioResource[index].audio;
            var delay = PlayClipAtPos(resource, pos, mixerType, audioPriority);
            var curve = GetMixerType(mixerType).volumeCurve;
            var c0 = GetMixerType(mixerType);

            var time = 0f;

            while (time < delay) {
                time += Time.deltaTime;
                c0.mixer?.SetFloat(c0.mixerName, curve.Evaluate(time / delay) * c0.focusedVolume);
                await Awaitable.NextFrameAsync(Token);
            }

            c0.mixer?.SetFloat(c0.mixerName, c0.defaultVolume);

            return delay;
        }

        public virtual float PlayClip(TEnum id, byte audioPriority = (byte)EAudioPriority.Average) {
            return PlayClipAtPos(id, float3.zero, audioPriority);
        }

        public virtual float PlayClip(AudioResource resource, byte audioPriority = (byte)EAudioPriority.Average) {
            return PlayClipAtPos(resource, float3.zero, masterAudioMixerGroup.mixer.outputAudioMixerGroup,
                audioPriority);
        }

        public virtual async Awaitable<float> PlayClip(TEnum id, float3 pos, EMixerType mixerType,
            byte audioPriority = (byte)EAudioPriority.Average) {
            return await PlayFocusClip(id, pos, mixerType, audioPriority);
        }

        public virtual bool StopClip(int idx = -1) {
            idx = idx == -1 ? _previousIndex : idx;
            var source = AudioSources[idx];

            if (!source.isPlaying) return false;
            source.Stop();

            return true;
        }

        public virtual bool PauseAllClips(bool isStop = false) {
            PauseIndexes = ListPool<int>.Get();

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

            ListPool<int>.Release(PauseIndexes);

            return true;
        }

    }

}
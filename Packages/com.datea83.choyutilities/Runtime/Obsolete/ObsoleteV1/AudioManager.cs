using System;
using System.Collections.Generic;
using EugeneC.Singleton;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace EugeneC.Obsolete {

    [Obsolete]
    public class AudioManager : GenericSingleton<AudioManager> {

        public AudioSource SourcePrefab;
        public AudioSfxOld[] Audiosfx;

        private readonly Dictionary<AudioType, AudioSfxOld> AudioDictionary = new();

        protected override void Awake() {
            base.Awake();
            foreach (var sfx in Audiosfx) AudioDictionary[sfx.AudioID] = sfx;
        }

        public void PlaySourceClip(AudioType AudioId, AudioMixerGroup Channel, Vector3 position) {
            if (AudioDictionary.TryGetValue(AudioId, out var sfx)) {
                var source = Instantiate(SourcePrefab, position, Quaternion.identity);
                var key = Random.Range(0, sfx.AudioClips.Length);
                var clip = sfx.AudioClips[key];
                source.outputAudioMixerGroup = Channel;
                source.PlayOneShot(clip);
                Destroy(source.gameObject, clip.length);
            }
        }

        public void PlayCLip(AudioType AudioId, AudioSource Source) {
            if (AudioDictionary.TryGetValue(AudioId, out var sfx)) {
                var key = Random.Range(0, sfx.AudioClips.Length);
                var c = sfx.AudioClips[key];
                Source.PlayOneShot(c);
            }
        }

        public void PlayLoopClip(AudioType AudioId, AudioSource Source) {
            Source.loop = true;
            PlayCLip(AudioId, Source);
        }

        public void PlayIgnorePauseClip(AudioType AudioId, AudioSource Source) {
            Source.ignoreListenerPause = true;
            PlayCLip(AudioId, Source);
        }

        public void PlayOverrideClip(AudioType AudioId, AudioSource Source) {
            Source.loop = false;
            Source.Stop();
            PlayIgnorePauseClip(AudioId, Source);
        }

    }

    [Serializable]
    public class AudioSfxOld {

        public AudioType AudioID;
        public AudioClip[] AudioClips;

    }

    public enum AudioType {

        Bugeat,
        BugWalk,
        BugDead

    }

}
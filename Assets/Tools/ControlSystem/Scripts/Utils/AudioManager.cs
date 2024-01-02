/*
 * @Author: wangyun
 * @CreateTime: 2023-02-20 00:57:45 313
 * @LastEditor: wangyun
 * @EditTime: 2023-02-20 00:57:45 323
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Control {
	public class AudioManager : MonoBehaviour {
		private static AudioManager s_Instance;
		public static AudioManager Instance {
			get {
				if (!s_Instance) {
					s_Instance = new GameObject("[" + nameof(AudioManager) + "]").AddComponent<AudioManager>();
				}
				return s_Instance;
			}
		}

		private float m_Volume = 1;
		public float Volume {
			get => m_Volume;
			set {
				float prevVolume = m_Volume;
				if (!Mathf.Approximately(value, prevVolume)) {
					m_Volume = value;
					foreach (var playingSource in m_PlayingSources) {
						playingSource.volume = playingSource.volume * value / prevVolume;
					}
				}
			}
		}

		private readonly List<AudioSource> m_PlayingSources = new List<AudioSource>();
		private readonly Queue<AudioSource> m_SourcePool = new Queue<AudioSource>();

		private float prevTimeScale = 1;
		private void Update() {
			float timeScale = Time.timeScale;
			if (!Mathf.Approximately(timeScale, prevTimeScale)) {
				prevTimeScale = timeScale;
				foreach (var playingSource in m_PlayingSources) {
					playingSource.pitch = timeScale;
				}
			}
		}

		private void OnDestroy() {
			StopAllCoroutines();
			foreach (var playingSource in m_PlayingSources) {
				ReleaseAudioSource(playingSource);
			}
		}

		public void Play(AudioClip clip, float volumeScale = 1) {
			if (clip) {
				StartCoroutine(IEPlay(clip, volumeScale));
			}
		}
		private IEnumerator IEPlay(AudioClip clip, float volumeScale) {
			AudioSource source = GetAudioSource();
			source.clip = clip;
			source.volume = Volume * volumeScale;
			source.Play();
			m_PlayingSources.Add(source);
			yield return new WaitForSeconds(clip.length);
			m_PlayingSources.Remove(source);
			ReleaseAudioSource(source);
		}
		
		private AudioSource GetAudioSource() {
			AudioSource source = m_SourcePool.Count > 0 ? m_SourcePool.Dequeue() : gameObject.AddComponent<AudioSource>();
			source.loop = false;
			source.pitch = Time.timeScale;
			return source;
		}
		private void ReleaseAudioSource(AudioSource source) {
			source.Stop();
			source.clip = null;
			source.volume = 1;
			source.loop = false;
			source.pitch = 1;
			m_SourcePool.Enqueue(source);
		}
	}
}

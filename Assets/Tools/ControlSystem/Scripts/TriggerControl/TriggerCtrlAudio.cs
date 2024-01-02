/*
 * @Author: wangyun
 * @CreateTime: 2022-04-26 13:14:08 509
 * @LastEditor: wangyun
 * @EditTime: 2022-04-26 13:14:08 503
 */

using System;
using System.Collections;
using UnityEngine;

namespace Control {
	public enum TriggerCtrlAudioOperation {
		PLAY = 0,
		STOP = 1,
		PAUSE = 2,
		RESUME = 3
	}
	
	[RequireComponent(typeof(AudioSource))]
	public class TriggerCtrlAudio : TriggerCtrlTrigger {
		public TriggerCtrlAudioOperation operation = TriggerCtrlAudioOperation.PLAY;
		public float fadeDuration;
		
		protected override void DoTrigger() {
			switch (operation) {
				case TriggerCtrlAudioOperation.PLAY: {
					AudioSource source = GetComponent<AudioSource>();
					if (fadeDuration > 0) {
						float volume = source.volume;
						source.volume = 0;
						source.Play();
						StartCoroutine(IEFadeVolume(volume));
					} else {
						source.Play();
					}
					break;
				}
				case TriggerCtrlAudioOperation.STOP: {
					AudioSource source = GetComponent<AudioSource>();
					if (fadeDuration > 0) {
						float volume = source.volume;
						StartCoroutine(IEFadeVolume(0, () => {
							source.Stop();
							source.volume = volume;
						}));
					} else {
						source.Stop();
					}
					break;
				}
				case TriggerCtrlAudioOperation.PAUSE: {
					AudioSource source = GetComponent<AudioSource>();
					if (fadeDuration > 0) {
						float volume = source.volume;
						StartCoroutine(IEFadeVolume(0, () => {
							source.Pause();
							source.volume = volume;
						}));
					} else {
						source.Pause();
					}
					break;
				}
				case TriggerCtrlAudioOperation.RESUME: {
					AudioSource source = GetComponent<AudioSource>();
					if (fadeDuration > 0) {
						float volume = source.volume;
						source.volume = 0;
						source.UnPause();
						StartCoroutine(IEFadeVolume(volume));
					} else {
						source.UnPause();
					}
					break;
				}
			}
		}

		private IEnumerator IEFadeVolume(float endVolume, Action callback = null) {
			AudioSource source = GetComponent<AudioSource>();
			float beginVolume = source.volume;
			float startTime = Time.time;
			float time = 0;
			while (time < fadeDuration) {
				source.volume = Mathf.Lerp(beginVolume, endVolume, time / fadeDuration);
				yield return null;
				time = Time.time - startTime;
			}
			source.volume = endVolume;
			callback?.Invoke();
		}
	}
}
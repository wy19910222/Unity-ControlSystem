/*
 * @Author: wangyun
 * @CreateTime: 2022-12-15 13:04:22 454
 * @LastEditor: wangyun
 * @EditTime: 2023-03-05 16:38:35 625
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Control {
	public partial class BaseProcessStep {
		private void DoStepAudioOneShot() {
			if (obj is AudioClip clip) {
				float volumeScale = GetFArgument(0);
				AudioManager.Instance.Play(clip, volumeScale);
			}
		}
		private void DoStepAudioSourceCtrl(MonoBehaviour executor) {
			if (obj is AudioSource source) {
				IEnumerator IEFadeVolume(AudioSource _source, float duration, float endVolume, System.Action callback = null) {
					float beginVolume = _source.volume;
					float startTime = Time.time;
					float _time = 0;
					while (_time < duration) {
						_source.volume = Mathf.Lerp(beginVolume, endVolume, _time / duration);
						yield return null;
						_time = Time.time - startTime;
					}
					_source.volume = endVolume;
					callback?.Invoke();
				}
				
				int ctrlType = GetIArgument(0);
				float fadeDuration = GetFArgument(0);
				switch (ctrlType) {
					case 0:
						if (fadeDuration > 0) {
							float volume = source.volume;
							source.volume = 0;
							source.Play();
							executor.StartCoroutine(IEFadeVolume(source, fadeDuration, volume));
						} else {
							source.Play();
						}
						break;
					case 1:
						if (source.isPlaying && fadeDuration > 0) {
							float volume = source.volume;
							executor.StartCoroutine(IEFadeVolume(source, fadeDuration, 0, () => {
								source.Stop();
								source.volume = volume;
							}));
						} else {
							source.Stop();
						}
						break;
					case 2:
						if (source.isPlaying && fadeDuration > 0) {
							float volume = source.volume;
							executor.StartCoroutine(IEFadeVolume(source, fadeDuration, 0, () => {
								source.Pause();
								source.volume = volume;
							}));
						} else {
							source.Pause();
						}
						break;
					case 3:
						if (!source.isPlaying && fadeDuration > 0) {
							float volume = source.volume;
							source.volume = 0;
							source.UnPause();
							executor.StartCoroutine(IEFadeVolume(source, fadeDuration, volume));
						} else {
							source.UnPause();
						}
						break;
					case 4:
						if (source.isPlaying) {
							if (fadeDuration > 0) {
								float volume = source.volume;
								executor.StartCoroutine(IEFadeVolume(source, fadeDuration, 0, () => {
									source.Pause();
									source.volume = volume;
								}));
							} else {
								source.Pause();
							}
						} else {
							if (fadeDuration > 0) {
								float volume = source.volume;
								source.volume = 0;
								source.UnPause();
								executor.StartCoroutine(IEFadeVolume(source, fadeDuration, volume));
							} else {
								source.UnPause();
							}
						}
						break;
				}
			}
		}
		
		private List<int> m_PrevAudiosIndexList;
		private bool m_AudiosShuffled;
		private void DoStepAudiosPlay() {
			int totalCount = objArguments.Count;
			switch (totalCount) {
				case 0:
					break;
				case 1: {
					if (objArguments[0] is AudioClip clip) {
						AudioManager.Instance.Play(clip);
					}
					break;
				}
				default: {
					List<AudioClip> audioList = new List<AudioClip>();
					if (m_PrevAudiosIndexList == null) {
						m_PrevAudiosIndexList = new List<int>();
					}
					int prevIndexCount = m_PrevExecutorsIndexList.Count;
					int audioCount = GetIArgument(0);
					int playType = GetIArgument(1);
					switch (playType) {
						case 0:
							int nextIndex = prevIndexCount > 0 ? m_PrevAudiosIndexList[prevIndexCount - 1] + 1 : 0;
							int shuffleType = GetIArgument(2);
							if (shuffleType > 0) {
								void Shuffle() {
									for (int i = objArguments.Count - 1; i > 0; --i) {
										int j = Random.Range(0, i + 1);
										if (j != i) {
											(objArguments[i], objArguments[j]) = (objArguments[j], objArguments[i]);
										}
									}
								}
								switch (shuffleType) {
									case 1:
										if (!m_AudiosShuffled) {
											m_AudiosShuffled = true;
											Shuffle();
										}
										break;
									case 2:
										// 如果上一次正好执行完列表或已经跨过列表末尾从头执行，则这次执行前要洗牌
										if (nextIndex >= totalCount || nextIndex < audioCount) {
											m_AudiosShuffled = true;
											Shuffle();
										}
										break;
								}
							}
							m_PrevAudiosIndexList.Clear();
							for (int i = 0, count = Mathf.Min(audioCount, totalCount); i < count; ++i) {
								int index = nextIndex + i;
								if (index >= totalCount) {
									index -= totalCount;
								}
								m_PrevAudiosIndexList.Add(index);
								audioList.Add(objArguments[index] as AudioClip);
							}
							break;
						case 1:
							// 如果数量不够，则无法做到不重复，只能是无限制，否则，看参数
							int randomType = prevIndexCount + audioCount > totalCount ? 0 : GetIArgument(2);
							List<int> indexList = new List<int>();
							for (int i = 0; i < totalCount; ++i) {
								if (randomType == 0 || !m_PrevAudiosIndexList.Contains(i)) {
									indexList.Add(i);
								}
							}
							m_PrevAudiosIndexList.Clear();
							for (int i = 0, count = Mathf.Min(audioCount, indexList.Count); i < count; ++i) {
								int indexIndex = Random.Range(0, indexList.Count);
								int index = indexList[indexIndex];
								indexList.RemoveAt(indexIndex);
								m_PrevAudiosIndexList.Add(index);
								audioList.Add(objArguments[index] as AudioClip);
							}
							break;
					}
					foreach (var clip in audioList) {
						if (clip) {
							AudioManager.Instance.Play(clip);
						}
					}
					break;
				}
			}
		}
	}
}
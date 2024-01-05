/*
 * @Author: wangyun
 * @CreateTime: 2022-12-15 13:03:47 789
 * @LastEditor: wangyun
 * @EditTime: 2023-03-05 16:37:02 432
 */

using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Control {
	public partial class ProcessStepBase {
		private List<int> m_PrevTriggersIndexList;
		private bool m_TriggersShuffled;
		private void DoStepTrigger() {
			int totalCount = objArguments.Count;
			switch (totalCount) {
				case 0:
					break;
				case 1: {
					if (objArguments[0] is BaseTriggerCtrl trigger) {
						trigger.Trigger();
					}
					break;
				}
				default: {
					List<BaseTriggerCtrl> triggerList = new List<BaseTriggerCtrl>();
					if (m_PrevTriggersIndexList == null) {
						m_PrevTriggersIndexList = new List<int>();
					}
					int triggerCount = GetIArgument(0);
					int triggerType = GetIArgument(1);
					switch (triggerType) {
						case 0:
							int nextIndex = m_PrevTriggersIndexList.Count > 0 ? m_PrevTriggersIndexList[m_PrevTriggersIndexList.Count - 1] + 1 : 0;
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
										if (!m_TriggersShuffled) {
											m_TriggersShuffled = true;
											Shuffle();
										}
										break;
									case 2:
										if (nextIndex >= totalCount || nextIndex < triggerCount) {
											m_TriggersShuffled = true;
											Shuffle();
										}
										break;
								}
							}
							m_PrevTriggersIndexList.Clear();
							for (int i = 0, count = Mathf.Min(triggerCount, totalCount); i < count; ++i) {
								int index = nextIndex + i;
								if (index >= totalCount) {
									index -= totalCount;
								}
								m_PrevTriggersIndexList.Add(index);
								triggerList.Add(objArguments[index] as BaseTriggerCtrl);
							}
							break;
						case 1:
							int randomType = GetIArgument(2);
							List<int> indexList = new List<int>();
							for (int i = 0, prevIndexCount = m_PrevTriggersIndexList.Count; i < totalCount; ++i) {
								if (randomType == 0 || prevIndexCount + triggerCount > totalCount || !m_PrevTriggersIndexList.Contains(i)) {
									indexList.Add(i);
								}
							}
							m_PrevTriggersIndexList.Clear();
							for (int i = 0, count = Mathf.Min(triggerCount, indexList.Count); i < count; ++i) {
								int indexIndex = Random.Range(0, indexList.Count);
								int index = indexList[indexIndex];
								indexList.RemoveAt(indexIndex);
								m_PrevTriggersIndexList.Add(index);
								triggerList.Add(objArguments[index] as BaseTriggerCtrl);
							}
							break;
					}
					foreach (var trigger in triggerList) {
						if (trigger) {
							trigger.Trigger();
						}
					}
					break;
				}
			}
		}

		private void DoStepStateController() {
			if (obj is StateController cState) {
				bool isRelative = GetBArgument(0);
				int stateCount = cState.StateCount;
				if (isRelative) {
					int offset = GetIArgument(0);
					int index = cState.Index + offset;
					bool loop = GetBArgument(1);
					if (loop) {
						while (index < 0) {
							index += stateCount;
						}
						while (index >= stateCount) {
							index -= stateCount;
						}
					}
					cState.Index = index;
				} else {
					bool recordIndex = GetBArgument(1);
					bool random = GetBArgument(2);
					bool noRepeat = GetBArgument(3);
					if (random) {
						if (iArguments.Contains(-1)) {
							if (stateCount > 0) {
								if (noRepeat) {
									int index = Random.Range(0, stateCount - 1);
									if (index >= cState.Index) {
										index += 1;
									}
									cState.Index = index;
								} else {
									cState.Index = Random.Range(0, stateCount);
								}
							}
						} else {
							if (recordIndex) {
								List<int> indexes = iArguments.FindAll(index => index >= 0 && index < stateCount);
								if (noRepeat) {
									indexes.Remove(cState.Index);
								}
								int count = indexes.Count;
								if (count > 0) {
									cState.Index = indexes[Random.Range(0, count)];
								}
							} else {
								Dictionary<int, int> uidIndexDict = new Dictionary<int, int>();
								for (int i = 0; i < stateCount; ++i) {
									State state = cState.states[i];
									if (!noRepeat || i != cState.Index) {
										uidIndexDict.Add(state.uid, i);
									}
								}
								List<int> uids = iArguments.FindAll(uid => uidIndexDict.ContainsKey(uid));
								int count = uids.Count;
								if (count > 0) {
									cState.Index = uidIndexDict[uids[Random.Range(0, count)]];
								}
							}

						}
					} else {
						if (recordIndex) {
							int index = GetIArgument(0);
							cState.Index = index;
						} else {
							int uid = GetIArgument(0);
							cState.Index = cState.states.FindIndex(state => state.uid == uid);
						}
					}
				}
			}
		}

		private Tweener m_ProgressTweener;
		private void DoStepProgressController() {
			if (obj is ProgressController cProgress) {
				float progress;
				bool isRelative = GetBArgument(0);
				if (isRelative) {
					float offset = GetFArgument(0);
					progress = cProgress.Progress + offset;
					bool loop = GetBArgument(1);
					if (loop) {
						while (progress >= 1) {
							progress -= 1;
						}
					}
				} else {
					bool random = GetBArgument(1);
					if (random) {
						float min = GetFArgument(0);
						float max = GetFArgument(1);
						progress = Random.Range(min, max);
					} else {
						progress = GetFArgument(0);
					}
				}
				if (m_ProgressTweener != null) {
					m_ProgressTweener.Kill();
					m_ProgressTweener = null;
				}
#if UNITY_EDITOR
				if (tween && Application.isPlaying) {
#else
				if (tween) {
#endif
					m_ProgressTweener = DOTween.To(
							() => cProgress.Progress,
							v => cProgress.Progress = v,
							progress,
							tweenDuration
					);
					if (tweenEase == Ease.INTERNAL_Custom) {
						m_ProgressTweener.SetEase(tweenEaseCurve);
					} else {
						m_ProgressTweener.SetEase(tweenEase);
					}
					m_ProgressTweener.SetDelay(tweenDelay).OnComplete(() => m_ProgressTweener = null);
				} else {
					cProgress.Progress = progress;
				}
			}
		}

		private void DoStepCustomEvent() {
			string eventName = GetSArgument(0);
			bool broadcast = GetBArgument(0);
			if (broadcast) {
				CustomEventListener.Emit(eventName);
			} else {
				GameObject target = GetObjArgument<GameObject>(0);
				if (target) {
					target.SendMessage("CustomTrigger", eventName);
				}
			}
		}
	}
}
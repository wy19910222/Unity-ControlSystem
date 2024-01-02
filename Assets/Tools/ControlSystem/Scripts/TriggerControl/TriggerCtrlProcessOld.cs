/*
 * @Author: wangyun
 * @CreateTime: 2022-06-28 22:23:45 172
 * @LastEditor: wangyun
 * @EditTime: 2022-07-27 18:02:54 844
 */

using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using DG.Tweening;
using DG.Tweening.Core;
using Sirenix.OdinInspector;

using URandom = UnityEngine.Random;
using UObject = UnityEngine.Object;

namespace Control {
	public enum ProcessStepType {
		// BaseTriggerCtrl obj
		TRIGGER = 0,
		// StateController obj, int index/indexMask, bool random
		STATE_CONTROLLER = 1,
		// ProgressController obj, float progress / bool random
		PROGRESS_CONTROLLER = 2,
		
		// ABSAnimationComponent obj, bool formHere
		DO_TWEEN_RESTART = 11,
		// DO_TWEEN_BACKWARDS = 12,
		// ABSAnimationComponent obj, float percent/time
		DO_TWEEN_GOTO = 13,
		// DO_TWEEN_FORWARDS = 14,
		// ABSAnimationComponent obj
		DO_TWEEN_PAUSE = 15,
		// ABSAnimationComponent obj
		DO_TWEEN_RESUME = 16,
		// ABSAnimationComponent obj
		DO_TWEEN_KILL = 17,
		
		// Animator obj, string paramName, bool/int/float value
		ANIMATOR = 21,
		// Animator obj, RuntimeAnimatorController controller
		ANIMATOR_CONTROLLER = 22,
		// ANIMATOR_AVATAR = 23,
		// Animator obj, bool applyRootMotion
		ANIMATOR_APPLY_ROOT_MOTION = 24,
		
		// AudioClip obj, float volumeScale
		AUDIO_ONE_SHOT = 31,
		// AudioSource source, float fadeDuration
		AUDIO_PLAY = 32,
		// AudioSource source, float fadeDuration
		AUDIO_STOP = 33,
		// AudioPAUSE = 34,
		// AudioResume = 35,
		
		// GameObject obj, bool value
		ACTIVE = 41,
		// Component obj, bool value
		ENABLED = 42,
		
		// Transform obj, Transform parent, bool resetPosRot
		INSTANTIATE = 51,
		// GameObject obj, bool onlyDestroyChildren
		DESTROY = 52,
		// Transform obj, Transform parent, bool resetPosRot
		PARENT = 53,

		// PlayableDirector obj
		PLAYABLE_PLAY = 61,
		// PlayableDirector obj
		PLAYABLE_STOP = 62,
		// PlayableDirector obj
		PLAYABLE_PAUSE = 63,
		// PlayableDirector obj
		PLAYABLE_RESUME = 64,
		// PlayableDirector obj, bool isPercent, float time/rate
		PLAYABLE_GOTO = 65,
		// PLAYABLE_EVALUATE = 66,
		// PLAYABLE_REBUILD_GRAPH = 67,
		
		// LOOK_AT,
		// CAMERA_ANCHOR_X,
		// CAMERA_ANCHOR_Y,
		
		// string eventName, bool broadcast, GameObject target
		CUSTOM_EVENT = 98,
		// UnityEvent obj
		UNITY_EVENT = 99,
	}
	
	[Serializable]
	public partial class ProcessStep {
		[HideInInspector]
		public string title;
		[HideInInspector]
		public float time;
		[HideInInspector]
		public float delayFrames;
		[HideInInspector]
		public ProcessStepType type;
		[HideInInspector]
		public UObject obj;
		[HideInInspector]
		public float fArgument;
		[HideInInspector]
		public string sArgument;
		[HideInInspector]
		public UObject objArgument;
		[HideInInspector]
		public UnityEvent unityEvent;
		
		public bool IsTriggered { get; set; }
	}
	
	[Obsolete("TriggerCtrlProcess has been deprecated. Use TriggerCtrlProcessNew instead")]
	[AddComponentMenu("")]
	public partial class TriggerCtrlProcessOld : TriggerCtrlTrigger {
		public bool singleProcess;
		public List<ProcessStep> steps = new List<ProcessStep>();

		private Coroutine m_Co;

		protected override void DoTrigger() {
			if (singleProcess) {
				StopProcess();
			}
			StartProcess();
		}

		public void StartProcess() {
			m_Co = StartCoroutine(IEProcess());
		}

		public void StopProcess() {
			if (m_Co != null) {
				StopCoroutine(m_Co);
				m_Co = null;
			}
		}

		private IEnumerator IEProcess() {
			if (steps.Count > 0) {
				List<ProcessStep> _steps = new List<ProcessStep>(steps);
				// 冒泡排序，同优先级保持原始顺序
				BubbleSort(_steps, (step1, step2) => {
					if (Mathf.Approximately(step1.time, step2.time)) {
						return step1.delayFrames - step2.delayFrames;
					}
					return step1.time - step2.time;
				});
				// 倒序，方便移除
				_steps.Reverse();
				
				Dictionary<ProcessStep, int> stepDelayedDict = new Dictionary<ProcessStep, int>();
				_steps.ForEach(step => stepDelayedDict.Add(step, 0));
				
				List<ProcessStep> waitForEndOfFrameSteps = new List<ProcessStep>();
				WaitForEndOfFrame endOfFrameYield = new WaitForEndOfFrame();
				
				int stepCount = _steps.Count;
				float startTime = Time.time;
				while (stepCount > 0) {
					float time = Time.time - startTime;
					for (int i = stepCount - 1; i >= 0; --i) {
						ProcessStep step = _steps[i];
						if (step.time <= time) {
							int delayed = stepDelayedDict[step];
							if (delayed >= step.delayFrames) {
								DoStep(step);
								_steps.RemoveAt(i);
								stepCount--;
							} else if (delayed + 1 > step.delayFrames) {
								waitForEndOfFrameSteps.Add(step);
								_steps.RemoveAt(i);
								stepCount--;
							} else {
								stepDelayedDict[step] = delayed + 1;
							}
						}
					}
					if (waitForEndOfFrameSteps.Count > 0) {
						yield return endOfFrameYield;
						foreach (ProcessStep step in waitForEndOfFrameSteps) {
							DoStep(step);
						}
						waitForEndOfFrameSteps.Clear();
					}
					if (stepCount <= 0) {
						break;
					}
					yield return null;
				}
				m_Co = null;
			}
		}

		private void DoStep(ProcessStep step) {
#if UNITY_EDITOR
			step.IsTriggered = true;
#endif
			switch (step.type) {
				case ProcessStepType.TRIGGER:
					if (step.obj is BaseTriggerCtrl trigger) {
						trigger.Trigger();
					}
					break;
				case ProcessStepType.STATE_CONTROLLER:
					if (step.obj is StateController cState) {
						bool random = step.sArgument == "1";
						if (random) {
							int indexMask = (int) step.fArgument;
							List<int> indexes = new List<int>();
							for (int i = 0, length = cState.StateCount; i < length; i++) {
								if ((indexMask & (long) 1 << i) != 0) {
									indexes.Add(i);
								}
							}
							cState.Index = indexes[URandom.Range(0, indexes.Count)];
						} else {
							cState.Index = (int) step.fArgument;
						}
					}
					break;
				case ProcessStepType.PROGRESS_CONTROLLER:
					if (step.obj is ProgressController cProgress) {
						bool random = step.sArgument == "1";
						cProgress.Progress = random ? URandom.value : step.fArgument;
					}
					break;
				case ProcessStepType.DO_TWEEN_RESTART:
					// fArgument > 0作为fromHere，sArgument == "1"作为includeDelay
					if (step.obj is ABSAnimationComponent tweenAnimRestart) {
						Tween tween = tweenAnimRestart.tween;
						if (tween != null) {
							if (step.fArgument > 0) {
								switch (tweenAnimRestart) {
									case DOTweenAnimation anim:
										if (anim.isRelative) {
											MethodInfo mi = typeof(DOTweenAnimation).GetMethod("ReEvaluateRelativeTween",
													BindingFlags.Instance | BindingFlags.NonPublic);
											mi?.Invoke(anim, Array.Empty<object>());
										}
										break;
									case DOTweenPath path:
										if (path.relative && !path.isLocal) {
											MethodInfo mi = typeof(DOTweenPath).GetMethod("ReEvaluateRelativeTween",
													BindingFlags.Instance | BindingFlags.NonPublic);
											mi?.Invoke(path, Array.Empty<object>());
										}
										break;
									default:
										// 只是借这个接口实现fromHere，目前不会走到这里
										tweenAnimRestart.DORestart(true);
										break;
								}
							}
							tween.Restart(step.sArgument == "1");
						}
					}
					break;
				case ProcessStepType.DO_TWEEN_GOTO:
					// fArgument作为进度，sArgument == "1"作为isPercent
					if (step.obj is ABSAnimationComponent tweenAnimGoto) {
						Tween tween = tweenAnimGoto.tween;
						if (tween != null) {
							bool isPercent = step.sArgument == "1" && tween.Loops() != -1;
							var value = isPercent ? tween.Duration() * Mathf.Clamp01(step.fArgument) : Mathf.Max(step.fArgument, 0);
							if (value > 0) {
								tween.Goto(value, tween.IsPlaying());
							} else {
								if (tween.IsPlaying()) {
									if (tween.position != 0) {
										tween.Goto(value, true);
									}
								} else {
									tween.Rewind();
								}
							}
						}
					}
					break;
				case ProcessStepType.DO_TWEEN_PAUSE:
					if (step.obj is ABSAnimationComponent tweenAnimPause) {
						tweenAnimPause.tween?.Pause();
					}
					break;
				case ProcessStepType.DO_TWEEN_RESUME:
					if (step.obj is ABSAnimationComponent tweenAnimResume) {
						tweenAnimResume.tween?.Play();
					}
					break;
				case ProcessStepType.DO_TWEEN_KILL:
					if (step.obj is ABSAnimationComponent tweenAnimKill) {
						tweenAnimKill.tween?.Kill();
					}
					break;
				case ProcessStepType.ANIMATOR: {
					if (step.obj is Animator animator) {
						int parameterCount = animator.parameterCount;
						for (int i = 0; i < parameterCount; ++i) {
							AnimatorControllerParameter parameter = animator.GetParameter(i);
							if (parameter.name == step.sArgument) {
								switch (parameter.type) {
									case AnimatorControllerParameterType.Float:
										animator.SetFloat(step.sArgument, step.fArgument);
										break;
									case AnimatorControllerParameterType.Int:
										animator.SetInteger(step.sArgument, (int) step.fArgument);
										break;
									case AnimatorControllerParameterType.Bool:
										animator.SetBool(step.sArgument, step.fArgument > 0);
										break;
									case AnimatorControllerParameterType.Trigger:
										if (step.fArgument > 0) {
											animator.SetTrigger(step.sArgument);
										} else {
											animator.ResetTrigger(step.sArgument);
										}
										break;
								}
								break;
							}
						}
					}
					break;
				}
				case ProcessStepType.ANIMATOR_CONTROLLER: {
					if (step.obj is Animator animator) {
						animator.runtimeAnimatorController = step.objArgument as RuntimeAnimatorController;
					}
					break;
				}
				case ProcessStepType.ANIMATOR_APPLY_ROOT_MOTION: {
					if (step.obj is Animator animator) {
						animator.applyRootMotion = step.fArgument > 0;
					}
					break;
				}
				case ProcessStepType.AUDIO_ONE_SHOT:
					if (step.obj is AudioClip clip) {
						AudioManager.Instance.Play(clip, step.fArgument);
					}
					break;
				case ProcessStepType.AUDIO_PLAY:
					if (step.obj is AudioSource sourcePlay) {
						float duration = step.fArgument;
						if (duration > 0) {
							float volume = sourcePlay.volume;
							sourcePlay.volume = 0;
							sourcePlay.Play();
							StartCoroutine(IEFadeVolume(sourcePlay, duration, volume));
						} else {
							sourcePlay.Play();
						}
					}
					break;
				case ProcessStepType.AUDIO_STOP:
					if (step.obj is AudioSource sourceStop) {
						float duration = step.fArgument;
						if (duration > 0) {
							float volume = sourceStop.volume;
							StartCoroutine(IEFadeVolume(sourceStop, duration, 0, () => {
								sourceStop.Stop();
								sourceStop.volume = volume;
							}));
						} else {
							sourceStop.Stop();
						}
					}
					break;
				case ProcessStepType.ACTIVE: {
					if (step.obj is GameObject go) {
						go.SetActive(step.fArgument > 0);
					}
					break;
				}
				case ProcessStepType.ENABLED: {
					switch (step.obj) {
						case Behaviour bhv:
							bhv.enabled = step.fArgument > 0;
							break;
						case Renderer rdr:
							rdr.enabled = step.fArgument > 0;
							break;
						case Collider cld:
							cld.enabled = step.fArgument > 0;
							break;
						case LODGroup lodG:
							lodG.enabled = step.fArgument > 0;
							break;
						case Cloth cloth:
							cloth.enabled = step.fArgument > 0;
							break;
					}
					break;
				}
				case ProcessStepType.INSTANTIATE: {
					if (step.obj is Transform prefab) {
						bool pivotIsParent = step.sArgument == "1" || step.sArgument == "3";
						bool resetToPivot = step.sArgument == "2" || step.sArgument == "3";
						bool resetPosRot = step.fArgument > 0;
						Transform pivot = step.objArgument as Transform;
						Transform parent = pivotIsParent ? pivot : transform;
						if (parent) {
							Vector3 pos = resetPosRot ? resetToPivot && pivot ? pivot.position: transform.position : parent.TransformPoint(prefab.localPosition);
							Quaternion rot = resetPosRot ? resetToPivot && pivot ? pivot.rotation : transform.rotation : parent.rotation * prefab.localRotation;
							Transform trans = Instantiate(prefab, pos, rot, parent);
							trans.gameObject.SetActive(true);
						} else {
							Vector3 pos = resetPosRot ? resetToPivot ? Vector3.zero : transform.position : prefab.position;
							Quaternion rot = resetPosRot ? resetToPivot ? Quaternion.identity : transform.rotation : prefab.rotation;
							Transform trans = Instantiate(prefab, pos, rot);
							trans.gameObject.SetActive(true);
						}
					}
					break;
				}
				case ProcessStepType.DESTROY: {
					if (step.obj is GameObject goDestroy) {
						bool onlyDestroyChildren = step.fArgument > 0;
						if (onlyDestroyChildren) {
							Transform trans = goDestroy.transform;
							for (int index = trans.childCount - 1; index >= 0; --index) {
								Transform child = trans.GetChild(index);
#if UNITY_EDITOR
								if (Application.isPlaying) {
									Destroy(child.gameObject);
								} else {
									DestroyImmediate(child.gameObject);
								}
#else
								Destroy(child.gameObject);
#endif
							}
						} else {
#if UNITY_EDITOR
							if (Application.isPlaying) {
								Destroy(goDestroy);
							} else {
								DestroyImmediate(goDestroy);
							}
#else
							Destroy(goDestroy);
#endif
						}
					}
					break;
				}
				case ProcessStepType.PARENT: {
					if (step.obj is Transform trans) {
						trans.SetParent(step.objArgument as Transform);
						bool resetPosRot = step.fArgument > 0;
						if (resetPosRot) {
							trans.localPosition = Vector3.zero;
							trans.localRotation = Quaternion.identity;
						}
					}
					break;
				}
				case ProcessStepType.PLAYABLE_PLAY: {
					if (step.obj is PlayableDirector director) {
						director.Play();
					}
					break;
				}
				case ProcessStepType.PLAYABLE_STOP: {
					if (step.obj is PlayableDirector director) {
						director.Stop();
					}
					break;
				}
				case ProcessStepType.PLAYABLE_PAUSE: {
					if (step.obj is PlayableDirector director) {
						director.Pause();
					}
					break;
				}
				case ProcessStepType.PLAYABLE_RESUME: {
					if (step.obj is PlayableDirector director) {
						director.Resume();
					}
					break;
				}
				case ProcessStepType.PLAYABLE_GOTO: {
					if (step.obj is PlayableDirector director) {
						bool isPercent = step.sArgument == "1";
						var value = isPercent ? director.duration * Mathf.Clamp01(step.fArgument) : Mathf.Max(step.fArgument, 0);
						director.time = value;
					}
					break;
				}
				case ProcessStepType.CUSTOM_EVENT:
					if (step.fArgument > 0) {
						CustomEventListener.Emit(step.sArgument);
					} else {
						if (step.objArgument is GameObject go) {
							go.SendMessage("CustomTrigger", step.sArgument);
						}
					}
					break;
				case ProcessStepType.UNITY_EVENT:
					step.unityEvent?.Invoke();
					break;
			}
		}

		private IEnumerator IEFadeVolume(AudioSource source, float duration, float endVolume, Action callback = null) {
			float beginVolume = source.volume;
			float startTime = Time.time;
			float time = 0;
			while (time < duration) {
				source.volume = Mathf.Lerp(beginVolume, endVolume, time / duration);
				yield return null;
				time = Time.time - startTime;
			}
			source.volume = endVolume;
			callback?.Invoke();
		}

		/// <summary>
		/// 冒泡排序
		/// </summary>
		private static void BubbleSort<T>(IList<T> list, Func<T, T, float> comparison) {
			int stepCount = list.Count;
			if (stepCount > 1) {
				for (int i = 0, lastIndex = stepCount - 1, sortBorder = lastIndex; i < lastIndex; ++i) {
					bool isSortComplete = true;
					int lastSwapIndex = 0;
					for (int j = 0; j < sortBorder; ++j) {
						if (comparison(list[j], list[j + 1]) > 0) {
							(list[j], list[j + 1]) = (list[j + 1], list[j]);
							isSortComplete = false;
							lastSwapIndex = j;
						}
					}
					sortBorder = lastSwapIndex;
					if (isSortComplete) {
						break;
					}
				}
			}
		}
	}
}
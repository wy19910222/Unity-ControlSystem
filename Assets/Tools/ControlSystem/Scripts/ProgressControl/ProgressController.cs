/*
 * @Author: wangyun
 * @CreateTime: 2022-04-18 14:22:18 824
 * @LastEditor: wangyun
 * @EditTime: 2022-06-27 11:44:26 097
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Control {
	[Serializable]
	public class ProgressRelate {
		// ReSharper disable once RedundantDefaultMemberInitializer
		public float minProgress = 0;
		public float maxProgress = 1;
		public float delay;
		public bool single = true;
	}
	
	[Serializable]
	public class ProgressRelateProgress : ProgressRelate {
		public ProgressController controller;
		// ReSharper disable once RedundantDefaultMemberInitializer
		public float targetMinProgress = 0;
		public float targetMaxProgress = 1;
	}
	
	[Serializable]
	public class ProgressRelateState : ProgressRelate {
		public const int TARGET_NONE = -1;
		
		public StateController controller;
		public int targetIndex = TARGET_NONE;
		public int targetUID = TARGET_NONE;
	}
	
	[Serializable]
	public class ProgressRelateExecutor : ProgressRelate {
		public BaseExecutor executor;
		public bool canExecuteAgain;
		public bool executed;
	}

	public class ProgressController : MonoBehaviour {
		public string title;
		public float initialProgress;
		public List<ProgressRelateProgress> relations = new List<ProgressRelateProgress>();
		public List<ProgressRelateState> stateRelations = new List<ProgressRelateState>();
		public List<ProgressRelateExecutor> executorRelations = new List<ProgressRelateExecutor>();

		public bool lazyInit;
		public bool invalidateTween;
		public bool InvalidateTween {
			get => invalidateTween;
			set => invalidateTween = value;
		}

		public bool Initialized { get; private set; }

		public bool tween;
		public float tweenDelay;
		public float tweenDuration = 0.3F;
		public Ease tweenEase = Ease.OutQuad;
		public AnimationCurve tweenEaseCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

		public Action OnApplied {get; set;}
		public Action OnChanged {get; set;}

#if UNITY_EDITOR
		// ReSharper disable once NotAccessedField.Local
		[SerializeField]
		private int snapCount;
#endif

		private Tweener m_Tweener;
		private readonly Dictionary<object, Coroutine> m_DelayCallDic = new Dictionary<object, Coroutine>();

		private void Reset() {
			title = string.Empty;
			initialProgress = 0;
			relations.Clear();
			stateRelations.Clear();
			executorRelations.Clear();
			m_PrevProgress = 0;
			m_Progress = 0;
		}

		private void Awake() {
			if (!lazyInit) {
				if (!Initialized) {
					Init();
				}
			}
		}

		private void Init() {
			Initialized = true;
			m_PrevProgress = initialProgress;
			m_Progress = initialProgress;
			Apply();
			RelationApply();
			OnApplied?.Invoke();
		}

		[SerializeField]
		private float m_Progress;
		public float Progress {
			get => m_Progress;
			set {
				if (m_Tweener != null) {
					m_Tweener.Kill();
					m_Tweener = null;
				}
#if UNITY_EDITOR
				if (tween && Application.isPlaying) {
#else
				if (tween) {
#endif
					m_Tweener = DOTween.To(
							() => Progress,
							v => {
								if (SetProgress(v)) {
									OnChanged?.Invoke();
								}
							},
							value,
							tweenDuration
					);
					if (tweenEase == Ease.INTERNAL_Custom) {
						m_Tweener.SetEase(tweenEaseCurve);
					} else {
						m_Tweener.SetEase(tweenEase);
					}
					m_Tweener.SetDelay(tweenDelay).OnComplete(() => m_Tweener = null);
				} else {
					if (SetProgress(value)) {
						OnChanged?.Invoke();
					}
				}
			}
		}
		public bool SetProgress(float progress) {
			if (!Initialized) {
				Init();
			}
			progress = Mathf.Clamp(progress, 0, 1);
			if (Math.Abs(progress - m_Progress) > Mathf.Epsilon) {
				m_PrevProgress = m_Progress;
				m_Progress = progress;
				Apply();
				RelationApply();
				OnApplied?.Invoke();
				return true;
			}
			return false;
		}
		
		[SerializeField]
		private float m_PrevProgress;
		public float PrevProgress => m_PrevProgress;

		public BaseProgressCtrl[] Targets {
			get {
				var comps = GetComponentsInChildren<BaseProgressCtrl>(true);
				return Array.FindAll(comps, item => item.controller == this);
			}
		}

		/**
		 * 记录状态
		 */
		public void Capture() {
			foreach (var item in Targets) {
				item.Capture(m_Progress);
			}
		}

		/**
		 * 应用状态
		 */
		public void Apply() {
			foreach (var item in Targets) {
				item.Apply(m_Progress);
			}
		}

		/**
		 * 关联控制
		 */
		private void RelationApply() {
			foreach (var relation in relations) {
				if (relation.controller && relation.minProgress <= m_Progress && relation.maxProgress >= m_Progress) {
					var range = relation.maxProgress - relation.minProgress;
					var rate = range == 0 ? 0 : (m_Progress - relation.minProgress) / range;
					DelayCall(relation.delay, () => {
						relation.controller.Progress = relation.targetMinProgress + rate * (relation.targetMaxProgress - relation.targetMinProgress);
					}, relation);
				}
			}
			foreach (var relation in stateRelations) {
				if (relation.controller && relation.targetUID != ProgressRelateState.TARGET_NONE
						&& relation.minProgress <= m_Progress && relation.maxProgress >= m_Progress) {
					int index = relation.controller.states.FindIndex(state => state.uid == relation.targetUID);
					DelayCall(relation.delay, () => relation.controller.Index = index, relation);
				}
			}
			foreach (var relation in executorRelations) {
				if (relation.executor && relation.minProgress <= m_Progress && relation.maxProgress >= m_Progress) {
					if (relation.canExecuteAgain || !relation.executed) {
						relation.executed = true;
						DelayCall(relation.delay, () => relation.executor.Execute(), relation);
					}
				} else {
					if (relation.executed) {
						relation.executed = false;
					}
				}
			}
		}

		private void DelayCall(float delay, Action callback, ProgressRelate owner) {
			if (delay > 0) {
				if (m_DelayCallDic.TryGetValue(owner, out Coroutine co)) {
					if (owner.single) {
						StopCoroutine(co);
					}
					m_DelayCallDic.Remove(owner);
				}
				m_DelayCallDic.Add(owner, StartCoroutine(IEDelayCall(delay, () => {
					m_DelayCallDic.Remove(owner);
					callback?.Invoke();
				})));
			} else {
				callback?.Invoke();
			}
		}
		private static IEnumerator IEDelayCall(float delay, Action callback) {
			yield return new WaitForSeconds(delay);
			callback?.Invoke();
		}
	}
}
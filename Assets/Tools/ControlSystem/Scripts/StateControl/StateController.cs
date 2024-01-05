/*
 * @Author: wangyun
 * @CreateTime: 2022-03-30 16:21:41 627
 * @LastEditor: wangyun
 * @EditTime: 2022-06-27 11:44:03 523
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Control {
	[Serializable]
	public class State {
		public int uid;
		public string name;
		public string desc;

		public State(int uid, string name = "") {
			this.uid = uid;
			this.name = name;
		}
	}

	[Serializable]
	public abstract class StateRelate {
		public List<int> fromUIDs = new List<int> {-1};
		public List<int> toUIDs = new List<int> {-1};
		public float delay;
		public bool single = true;
	}

	[Serializable]
	public class StateRelateState : StateRelate {
		public const int TARGET_NONE = -1;
		public const int TARGET_SAME_INDEX = -2;
		public const int TARGET_SAME_NAME = -3;

		public StateController controller;
		public int targetUID = TARGET_NONE;
	}

	[Serializable]
	public class StateRelateProgress : StateRelate {
		public ProgressController controller;
		public float targetProgress;
	}

	[Serializable]
	public class StateRelateTrigger : StateRelate {
		public BaseTrigger trigger;
	}

	public class StateController : MonoBehaviour {
		protected int AutoIncID {
			get {
				int id = -1;
				foreach (var state in states) {
					if (state.uid > id) {
						id = state.uid;
					}
				}
				return id + 1;
			}
		}

		public string title;
		public int initialIndex;
		public List<State> states = new List<State>();
		public List<StateRelateState> relations = new List<StateRelateState>();
		public List<StateRelateProgress> progressRelations = new List<StateRelateProgress>();
		public List<StateRelateTrigger> triggerRelations = new List<StateRelateTrigger>();

		public bool lazyInit;
		public bool invalidateTween;
		public bool InvalidateTween {
			get => invalidateTween;
			set => invalidateTween = value;
		}

		public bool Initialized { get; protected set; }

		public Action OnApplied {get; set;}
		public Action OnChanged {get; set;}
		
		protected readonly Dictionary<object, Coroutine> m_DelayCallDic = new Dictionary<object, Coroutine>();

		protected virtual void Reset() {
			title = string.Empty;
			initialIndex = 0;
			states.Clear();
			relations.Clear();
			progressRelations.Clear();
			triggerRelations.Clear();
			m_PrevIndex = 0;
			m_Index = 0;
			AddState();
			AddState();
		}

		protected virtual void Awake() {
			if (!lazyInit) {
				if (!Initialized) {
					Init();
				}
			}
		}

		protected virtual void Init() {
			Initialized = true;
			m_PrevIndex = initialIndex;
			m_Index = initialIndex;
			Apply();
			RelationApply();
			OnApplied?.Invoke();
		}

		public int StateCount => states.Count;

		[SerializeField]
		protected int m_Index;
		public int Index {
			get => m_Index;
			set {
				if (SetIndex(value)) {
					OnChanged?.Invoke();
				}
			}
		}
		public bool SetIndex(int value) {
			if (!Initialized) {
				Init();
			}
			value = Mathf.Clamp(value, 0, StateCount - 1);
			if (value != m_Index) {
				m_PrevIndex = m_Index;
				m_Index = value;
				Apply();
				RelationApply();
				OnApplied?.Invoke();
				return true;
			}
			return false;
		}
		
		[SerializeField]
		protected int m_PrevIndex;
		public int PrevIndex => m_PrevIndex;

		public string State {
			get => m_Index >= 0 && m_Index < StateCount ? states[m_Index].name : null;
			set {
				if (SetState(value)) {
					OnChanged?.Invoke();
				}
			}
		}
		public string PrevState => m_PrevIndex >= 0 && m_PrevIndex < StateCount ? states[m_PrevIndex].name : null;

		public bool SetState(string value) {
			return SetIndex(states.FindIndex(state => state.name == value));
		}

		public BaseStateCtrl[] Targets {
			get {
				var comps = GetComponentsInChildren<BaseStateCtrl>(true);
				return Array.FindAll(comps, item => item.controller == this);
			}
		}

		public void AddState(string stateName = "") {
			states.Add(new State(AutoIncID, stateName));
		}

		public void RemoveState(int index) {
			states.RemoveAt(index);
		}

		/**
		 * 记录状态
		 */
		public void Capture() {
			if (m_Index >= 0 && m_Index < StateCount) {
				var uid = states[m_Index].uid;
				foreach (var item in Targets) {
					item.Capture(uid);
				}
			}
		}

		/**
		 * 应用状态
		 */
		public void Apply() {
			if (m_Index >= 0 && m_Index < StateCount) {
				var uid = states[m_Index].uid;
				foreach (var item in Targets) {
					item.Apply(uid);
				}
			}
		}

		/**
		 * 关联控制
		 */
		protected virtual void RelationApply() {
			foreach (var relation in relations) {
				if (relation.controller && relation.targetUID != StateRelateState.TARGET_NONE
						&& IsIndexSelected(relation.toUIDs, m_Index) && IsIndexSelected(relation.fromUIDs, m_PrevIndex)) {
					switch (relation.targetUID) {
						case StateRelateState.TARGET_SAME_INDEX: {
							int index = Index;
							DelayCall(relation.delay, () => relation.controller.Index = index, relation);
							break;
						}
						case StateRelateState.TARGET_SAME_NAME: {
							string state = State;
							DelayCall(relation.delay, () => relation.controller.State = state, relation);
							break;
						}
						default: {
							int index = relation.controller.states.FindIndex(state => state.uid == relation.targetUID);
							DelayCall(relation.delay, () => relation.controller.Index = index, relation);
							break;
						}
					}
				}
			}
			foreach (var relation in progressRelations) {
				if (relation.controller && IsIndexSelected(relation.toUIDs, m_Index) && IsIndexSelected(relation.fromUIDs, m_PrevIndex)) {
					DelayCall(relation.delay, () => relation.controller.Progress = relation.targetProgress, relation);
				}
			}
			foreach (var relation in triggerRelations) {
				if (relation.trigger && IsIndexSelected(relation.toUIDs, m_Index) && IsIndexSelected(relation.fromUIDs, m_PrevIndex)) {
					DelayCall(relation.delay, () => relation.trigger.Trigger(), relation);
				}
			}
		}

		private bool IsIndexSelected(ICollection<int> uids, int index) {
			return uids.Contains(-1) || index >= 0 && index < StateCount && uids.Contains(states[index].uid);
		}

		protected void DelayCall(float delay, Action callback, StateRelate owner) {
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
		protected static IEnumerator IEDelayCall(float delay, Action callback) {
			yield return new WaitForSeconds(delay);
			callback?.Invoke();
		}

		[ContextMenu("ArrangeStateID")]
		public void ArrangeStateID() {
#if UNITY_EDITOR
			UnityEditor.Undo.RegisterFullObjectHierarchyUndo(this, "ArrangeStateID");
#endif
			Dictionary<int, int> idMap = new Dictionary<int, int> {{-1, -1}};
			for (int i = 0, length = states.Count; i < length; ++i) {
				idMap.Add(states[i].uid, i);
				states[i].uid = i;
			}
			foreach (var relation in relations) {
				for (int i = 0, length = relation.fromUIDs.Count; i < length; ++i) {
					relation.fromUIDs[i] = idMap[relation.fromUIDs[i]];
				}
				for (int i = 0, length = relation.toUIDs.Count; i < length; ++i) {
					relation.toUIDs[i] = idMap[relation.toUIDs[i]];
				}
			}
			foreach (var relation in progressRelations) {
				for (int i = 0, length = relation.fromUIDs.Count; i < length; ++i) {
					relation.fromUIDs[i] = idMap[relation.fromUIDs[i]];
				}
				for (int i = 0, length = relation.toUIDs.Count; i < length; ++i) {
					relation.toUIDs[i] = idMap[relation.toUIDs[i]];
				}
			}
			foreach (var relation in triggerRelations) {
				for (int i = 0, length = relation.fromUIDs.Count; i < length; ++i) {
					relation.fromUIDs[i] = idMap[relation.fromUIDs[i]];
				}
				for (int i = 0, length = relation.toUIDs.Count; i < length; ++i) {
					relation.toUIDs[i] = idMap[relation.toUIDs[i]];
				}
			}
			foreach (var item in Targets) {
				item.ArrangeStateID(idMap);
			}
		}
	}
}
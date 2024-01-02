/*
 * @Author: wangyun
 * @CreateTime: 2022-04-19 16:21:41 627
 * @LastEditor: wangyun
 * @EditTime: 2022-04-19 16:21:41 627
 */

using System;
using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace Control {
	public abstract class BaseTriggerCtrl : MonoBehaviour {
#if UNITY_EDITOR
		[OnInspectorGUI("DrawTriggerButton", false)]
#endif
		public string title;
		public bool autoTrigger;
		public float triggerDelay;
		public bool validInactive;
		
		private Action m_OnTrigger;
#if UNITY_EDITOR
		protected virtual bool IsTriggered { get; set; }
#endif

		protected void Start() {
			if (autoTrigger) {
				Trigger();
			}
		}

		[ContextMenu("Trigger")]
		public void Trigger() {
			if (enabled && (validInactive || gameObject.activeInHierarchy)) {
#if UNITY_EDITOR
				IsTriggered = true;
				if (triggerDelay > 0 && Application.isPlaying) {
#else
				if (triggerDelay > 0) {
#endif
					StartCoroutine(IETrigger());
				} else {
					BeforeTrigger();
					DoTrigger();
					AfterTrigger();
					m_OnTrigger?.Invoke();
				}
			}
		}

		private IEnumerator IETrigger() {
			yield return new WaitForSeconds(triggerDelay);
			BeforeTrigger();
			DoTrigger();
			AfterTrigger();
			m_OnTrigger?.Invoke();
		}
		
		protected abstract void BeforeTrigger();
		
		protected abstract void DoTrigger();
		
		protected abstract void AfterTrigger();
		
		public void OnTrigger(Action action) {
			m_OnTrigger += action;
		}

		public void OffTrigger(Action action) {
			m_OnTrigger -= action;
		}

#if UNITY_EDITOR
		protected void DrawTriggerButton() {
			UnityEditor.EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("触发")) {
				BeforeEditorTrigger();
				Trigger();
			}
			string text = IsTriggered ? "已触发" : "未触发";
			if (CustomEditorGUI.Toggle(IsTriggered, text, CustomEditorGUI.COLOR_TOGGLE_CHECKED, GUILayout.Width(80F)) != IsTriggered) {
				IsTriggered = false;
			}
			UnityEditor.EditorGUILayout.EndHorizontal();
		}
		
		protected virtual void BeforeEditorTrigger() {
		}
#endif
	}
}
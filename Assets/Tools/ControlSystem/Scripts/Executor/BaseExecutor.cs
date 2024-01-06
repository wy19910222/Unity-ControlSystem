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
	public abstract class BaseExecutor : MonoBehaviour {
#if UNITY_EDITOR
		[OnInspectorGUI("DrawExecuteButton", false)]
#endif
		public string title;
		public bool executeOnStart;
		public float executeDelay;
		public bool validInactive;
		
		private Action m_OnExecute;
#if UNITY_EDITOR
		protected virtual bool IsExecuted { get; set; }
#endif

		protected void Start() {
			if (executeOnStart) {
				Execute();
			}
		}

		[ContextMenu("Execute")]
		public void Execute() {
			if (enabled && (validInactive || gameObject.activeInHierarchy)) {
#if UNITY_EDITOR
				IsExecuted = true;
				if (executeDelay > 0 && Application.isPlaying) {
#else
				if (executeDelay > 0) {
#endif
					StartCoroutine(IEExecute());
				} else {
					DoExecute();
					m_OnExecute?.Invoke();
				}
			}
		}

		private IEnumerator IEExecute() {
			yield return new WaitForSeconds(executeDelay);
			DoExecute();
			m_OnExecute?.Invoke();
		}
		
		protected abstract void DoExecute();
		
		public void OnExecute(Action action) {
			m_OnExecute += action;
		}

		public void OffExecute(Action action) {
			m_OnExecute -= action;
		}

#if UNITY_EDITOR
		protected void DrawExecuteButton() {
			UnityEditor.EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("执行")) {
				BeforeEditorExecute();
				Execute();
			}
			string text = IsExecuted ? "已执行" : "未执行";
			if (CustomEditorGUI.Toggle(IsExecuted, text, CustomEditorGUI.COLOR_TOGGLE_CHECKED, GUILayout.Width(80F)) != IsExecuted) {
				IsExecuted = false;
			}
			UnityEditor.EditorGUILayout.EndHorizontal();
		}
		
		protected virtual void BeforeEditorExecute() {
		}
#endif
	}
}
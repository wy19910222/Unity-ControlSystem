/*
 * @Author: wangyun
 * @CreateTime: 2022-03-30 16:21:41 627
 * @LastEditor: wangyun
 * @EditTime: 2022-03-30 16:21:41 627
 */

using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Control {
	public abstract class BaseStateCtrl : MonoBehaviour {
		[StateControllerSelect]
		public StateController controller;
		public abstract void Capture(int uid);
		public abstract void Apply(int uid);

		public abstract void ArrangeStateID(Dictionary<int, int> idMap);

#if UNITY_EDITOR
		[ContextMenu("FillAllStates")]
		private void FillAllStates() {
			UnityEditor.Undo.RecordObject(this, "FillAllStates");
			DoFillAllStates();
		}
		protected abstract void DoFillAllStates();
#endif
	}

	public class BaseStateCtrl<TValue> : BaseStateCtrl {
#if UNITY_EDITOR
		[OnInspectorGUI("DrawValueEditableButton")]
#endif

		[SerializeField]
		private TValue m_DefaultValue;

		[SerializeField, EnableIf("@UnityEditor.EditorPrefs.GetBool(\"BaseStateCtrl.ValueEditable\")")]
		protected List<int> m_ValueKeys = new List<int>();
		[SerializeField, EnableIf("@UnityEditor.EditorPrefs.GetBool(\"BaseStateCtrl.ValueEditable\")")]
		protected List<TValue> m_ValueValues = new List<TValue>();

		private TValue GetValue(int uid) {
			for (int index = 0, length = m_ValueKeys.Count; index < length; ++index) {
				if (m_ValueKeys[index] == uid) {
					return m_ValueValues[index];
				}
			}
			return m_DefaultValue;
		}

		private void SetValue(int uid, TValue value) {
			for (int index = 0, length = m_ValueKeys.Count; index < length; ++index) {
				if (m_ValueKeys[index] == uid) {
					m_ValueValues[index] = value;
					return;
				}
			}
			m_ValueKeys.Add(uid);
			m_ValueValues.Add(value);
		}

		protected virtual void Reset() {
			controller = GetComponentInParent<StateController>();
			m_DefaultValue = TargetValue;
			m_ValueKeys.Clear();
			m_ValueValues.Clear();
		}

		protected virtual TValue TargetValue { get; set; }

		public override void Capture(int uid) {
			SetValue(uid, TargetValue);
#if UNITY_EDITOR
			// 清理用不到的状态
			if (controller) {
				for (int index = m_ValueKeys.Count - 1; index >= 0; --index) {
					var key = m_ValueKeys[index];
					if (!controller.states.Exists(state => state.uid == key)) {
						m_ValueKeys.RemoveAt(index);
						m_ValueValues.RemoveAt(index);
					}
				}
			} else {
				m_ValueKeys.Clear();
				m_ValueValues.Clear();
			}
#endif
		}

		public override void Apply(int uid) {
#if UNITY_EDITOR
			UnityEditor.Undo.RegisterFullObjectHierarchyUndo(this, "Apply");
#endif
			TargetValue = GetValue(uid);
		}

		public override void ArrangeStateID(Dictionary<int, int> idMap) {
			if (idMap != null) {
				for (int i = m_ValueKeys.Count - 1; i >= 0; --i) {
					if (idMap.TryGetValue(m_ValueKeys[i], out int uid)) {
						m_ValueKeys[i] = uid;
					} else {
						m_ValueKeys.RemoveAt(i);
						m_ValueValues.RemoveAt(i);
					}
				}
			}
			for (int i = 0, lastIndex = m_ValueKeys.Count - 1, sortBorder = lastIndex; i < lastIndex; ++i) {
				bool isSortComplete = true;
				int lastSwapIndex = 0;
				for (int j = 0; j < sortBorder; ++j) {
					if (m_ValueKeys[j] > m_ValueKeys[j + 1]) {
						(m_ValueKeys[j], m_ValueKeys[j + 1]) = (m_ValueKeys[j + 1], m_ValueKeys[j]);
						(m_ValueValues[j], m_ValueValues[j + 1]) = (m_ValueValues[j + 1], m_ValueValues[j]);
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
		

#if UNITY_EDITOR
		protected void DrawValueEditableButton() {
			bool valueEditable = UnityEditor.EditorPrefs.GetBool("BaseStateCtrl.ValueEditable");
			if (CustomEditorGUI.Toggle(valueEditable, "编辑数据", CustomEditorGUI.COLOR_TOGGLE_CHECKED_EDITOR) != valueEditable) {
				UnityEditor.EditorPrefs.SetBool("BaseStateCtrl.ValueEditable", !valueEditable);
			}
		}
		
		protected override void DoFillAllStates() {
			if (controller) {
				for (int i = 0, length = controller.states.Count; i < length; ++i) {
					int uid = controller.states[i].uid;
					if (!m_ValueKeys.Contains(uid)) {
						m_ValueKeys.Add(uid);
						m_ValueValues.Add(TargetValue);
					}
				}
			}
		}
#endif
	}
}
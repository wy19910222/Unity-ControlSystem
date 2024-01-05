/*
 * @Author: wangyun
 * @CreateTime: 2022-12-09 14:16:58 253
 * @LastEditor: wangyun
 * @EditTime: 2022-12-14 17:13:10 612
 */

#if UNITY_EDITOR
using System.Reflection;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;

namespace Control {
	public partial class TriggerProcessBase<T> {
		protected override bool IsTriggered {
			get => base.IsTriggered;
			set {
				base.IsTriggered = value;
				if (!value) {
					PropertyInfo pi = typeof(ProcessStepBase).GetProperty("IsTriggered", BindingFlags.Instance | BindingFlags.NonPublic);
					if (pi != null) {
						foreach (var step in steps) {
							pi.SetValue(step, false);
						}
					}
				}
			}
		}
		
		protected override void BeforeEditorTrigger() {
			base.BeforeEditorTrigger();
			foreach (var step in steps) {
				if (step.obj) {
					Undo.RecordObject(step.obj, "Trigger");
				}
				foreach (var objArgument in step.objArguments) {
					if (objArgument) {
						Undo.RecordObject(objArgument, "Trigger");
					}
				}
			}
		}

		[OnInspectorGUI]
		protected void OnInspectorGUI(InspectorProperty property) {
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("克隆最后一步")) {
				property.RecordForUndo("Add");
				int stepCount = steps.Count;
				steps.Add(stepCount > 0 ? SerializeClone(steps[stepCount - 1]) : CreateStep());
			}

			CustomEditorGUI.BeginDisabled(!Clipboard.CanPaste<ProcessStepBase>());
			if (GUILayout.Button("粘贴到最后")) {
				property.RecordForUndo("Add");
				steps.Add(Clipboard.Paste<T>());
			}
			CustomEditorGUI.EndDisabled();
			EditorGUILayout.EndHorizontal();
		}
		
		protected virtual T CreateStep() {
			return new T();
		}

		protected static TCopy SerializeClone<TCopy>(TCopy src) where TCopy : class {
			if (src == null) {
				return null;
			}
			FieldInfo copyModeFi = typeof(Clipboard).GetField("copyMode", BindingFlags.Static | BindingFlags.NonPublic);
			FieldInfo objFi = typeof(Clipboard).GetField("obj", BindingFlags.Static | BindingFlags.NonPublic);
			object copyMode = copyModeFi?.GetValue(null) ?? CopyModes.DeepCopy;
			string systemCopyBuffer = GUIUtility.systemCopyBuffer;
			object obj = objFi?.GetValue(null);
			Clipboard.Copy(src);
			TCopy dst = Clipboard.Paste<TCopy>();
			copyModeFi?.SetValue(null, copyMode);
			GUIUtility.systemCopyBuffer = systemCopyBuffer;
			objFi?.SetValue(null, obj);
			return dst;
		}
	}
}
#endif
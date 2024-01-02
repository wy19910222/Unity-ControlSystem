/*
 * @Author: wangyun
 * @CreateTime: 2022-04-20 20:37:18 063
 * @LastEditor: wangyun
 * @EditTime: 2022-08-26 00:59:15 082
 */

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

namespace Control {
	public partial class TriggerCtrlStateController {
		private static readonly GUIStyle STYLE = new GUIStyle();
		private const float BUTTON_WIDTH_MIN = 50F;
		private static readonly GUILayoutOption BUTTON_WIDTH_MIN_OPTION = GUILayout.MinWidth(BUTTON_WIDTH_MIN);
		
		[OnInspectorGUI]
		private void DrawStates() {
			if (controller) {
				GUILayout.Space(5F);
				
				// string[] options = GetStatePopupOption();
				// System.Text.ASCIIEncoding ascii = new System.Text.ASCIIEncoding();
				// float optionLengthMax = 0;
				// foreach (var option in options) {
				// 	float optionLength = 0;
				// 	byte[] bytes = ascii.GetBytes(option);
				// 	for (int i = 0, length = bytes.Length; i < length; ++i) {
				// 		optionLength += bytes[i] == 63 ? 2.5F : 1F;
				// 	}
				// 	if (optionLength > optionLengthMax) {
				// 		optionLengthMax = optionLength;
				// 	}
				// }
				// int stateCount = options.Length;
				// float itemWidth = Mathf.Max(optionLengthMax * 5F, BUTTON_WIDTH_MIN);
				// float contextWidth = EditorGUIUtility.currentViewWidth * (CustomEditorGUI.IsPrefabComparing() ? 0.5F : 1);
				// int columnMax = Mathf.Max(1, (int) (contextWidth / itemWidth));
				// int row = Mathf.CeilToInt((float) stateCount / columnMax);
				// int col = Mathf.Min(columnMax, stateCount);
				float contextWidth = EditorGUIUtility.currentViewWidth * (CustomEditorGUI.IsPrefabComparing() ? 0.5F : 1) - 20 - 5 - 10;	// 前面空20，后面空5，滚动条10
				string[] options = GetStatePopupOption();
				int stateCount = options.Length;
				int col = 1;
				float sumWidth = 0;
				for (int i = 0; i < stateCount; ++i) {
					float itemWidth = Mathf.Max(STYLE.CalcSize(new GUIContent(options[i])).x + 15, BUTTON_WIDTH_MIN);
					sumWidth += itemWidth;
					if (sumWidth <= contextWidth) {
						col = Mathf.Max(i + 1, 1);
					} else {
						break;
					}
				}
				bool perfect = false;
				while (col > 1 && !perfect) {
					perfect = true;
					for (int i = 0; i < stateCount; i += col) {
						sumWidth = 0;
						for (int j = 0, length = Math.Min(col, stateCount - 1 - i); j < length; ++j) {
							float itemWidth = Mathf.Max(STYLE.CalcSize(new GUIContent(options[i])).x + 15, BUTTON_WIDTH_MIN);
							sumWidth += itemWidth;
						}
						if (sumWidth > contextWidth) {
							perfect = false;
							--col;
							break;
						}
					}
				}
				int row = Mathf.CeilToInt((float) stateCount / col);
				
				if (random) {
					for (int r = 0; r < row; ++r) {
						EditorGUILayout.BeginHorizontal();
						for (int c = 0; c < col; ++c) {
							int i = r * col + c;
							if (i < stateCount) {
								long iMask = (long) 1 << i;
								bool selected = (indexMask & iMask) != 0;
								// 序号按钮
								if (CustomEditorGUI.Toggle(selected, options[i], CustomEditorGUI.COLOR_TOGGLE_CHECKED, BUTTON_WIDTH_MIN_OPTION) != selected) {
									Undo.RecordObject(this, "IndexMask");
									if (selected) {
										indexMask ^= iMask;
									} else {
										indexMask |= iMask;
									}
								}
							}
						}
						EditorGUILayout.EndHorizontal();
					}
				} else {
					for (int r = 0; r < row; ++r) {
						EditorGUILayout.BeginHorizontal();
						for (int c = 0; c < col; ++c) {
							int i = r * col + c;
							if (i < stateCount) {
								bool selected = i == index;
								// 序号按钮
								if (CustomEditorGUI.Toggle(selected, options[i], CustomEditorGUI.COLOR_TOGGLE_CHECKED, BUTTON_WIDTH_MIN_OPTION) && !selected) {
									Undo.RecordObject(this, "Index");
									index = i;
								}
							}
						}
						EditorGUILayout.EndHorizontal();
					}
				}
			} else {
				SerializedObject serializedObject = new SerializedObject(this);
				SerializedProperty serializedProperty = serializedObject.FindProperty("index");
				EditorGUILayout.PropertyField(serializedProperty);
				if (GUI.changed) {
					if (serializedProperty.intValue < 0) {
						serializedProperty.intValue = 0;
					}
					serializedObject.ApplyModifiedProperties();
				}
			}
		}
		
		private string[] GetStatePopupOption() {
			List<State> states = controller.states;
			bool anyNameExist = states.Exists(state => !string.IsNullOrEmpty(state.name));
			int stateCount = states.Count;
			string[] options = new string[stateCount];
			for (int i = 0; i < stateCount; ++i) {
				State state = states[i];
				string ext = anyNameExist ? state.name : state.desc;
				if (!string.IsNullOrEmpty(ext)) {
					ext = ":" + ext;
				}
				options[i] = i + ext;
			}
			return options;
		}
	}
}
#endif
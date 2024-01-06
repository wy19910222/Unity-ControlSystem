/*
 * @Author: wangyun
 * @CreateTime: 2022-12-15 12:39:01 543
 * @LastEditor: wangyun
 * @EditTime: 2023-03-04 17:17:17 717
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

using AnimatorControllerParameter = UnityEngine.AnimatorControllerParameter;
using AnimatorControllerParameterType = UnityEngine.AnimatorControllerParameterType;

namespace Control {
	public partial class BaseProcessStepDrawer<TStep> {
		private void DrawAnimatorParameters() {
			Animator newObj = DrawCompFieldWithThisBtn<Animator>("动画器", Target.obj);
			if (newObj != Target.obj) {
				Property.RecordForUndo("Obj");
				Target.obj = newObj;
			}
			if (newObj != null) {
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("参数", CustomEditorGUI.LabelWidthOption);
		
				if (newObj.gameObject.activeInHierarchy && newObj.runtimeAnimatorController is AnimatorController controller) {
					// if (!newObj.isInitialized) {
					// 	// 随便找个字段改一下
					// 	SerializedObject serializedObject = new SerializedObject(newObj);
					// 	SerializedProperty serializedProperty = serializedObject.FindProperty("m_ApplyRootMotion");
					// 	bool willRevert = serializedProperty.isInstantiatedPrefab && !serializedProperty.prefabOverride;
					// 	serializedProperty.boolValue = !serializedProperty.boolValue;
					// 	serializedProperty.boolValue = !serializedProperty.boolValue;
					// 	if (willRevert && serializedProperty.prefabOverride) {
					// 		PrefabUtility.RevertPropertyOverride(serializedProperty, InteractionMode.AutomatedAction);
					// 	} else {
					// 		serializedObject.ApplyModifiedProperties();
					// 	}
					// }
					List<AnimatorControllerParameter> parameters = new List<AnimatorControllerParameter>(controller.parameters);
					if (parameters.Count > 0) {
						List<string> paramNames = parameters.ConvertAll(parameter => parameter.name);
						int index = paramNames.IndexOf(Target.sArguments[0]);
						int newIndex = EditorGUILayout.Popup(index, paramNames.ToArray());
						if (newIndex != index) {
							Property.RecordForUndo("SArguments");
							Target.sArguments[0] = paramNames[newIndex];
						}
		
						if (newIndex != -1) {
							switch (parameters[newIndex].type) {
								case AnimatorControllerParameterType.Float: {
									GUILayoutOption fieldWidth = GUILayout.Width(s_ContextWidth * 0.3F);
									float newFValue = EditorGUILayout.FloatField(Target.fArguments[0], fieldWidth);
									if (!Mathf.Approximately(newFValue, Target.fArguments[0])) {
										Property.RecordForUndo("FArguments");
										Target.fArguments[0] = newFValue;
									}
									bool newIsRelative = DrawToggle(Target.bArguments[0], "相对偏移", BTN_WIDTH_OPTION);
									if (newIsRelative != Target.bArguments[0]) {
										Property.RecordForUndo("BArguments");
										Target.bArguments[0] = newIsRelative;
									}
									EditorGUILayout.EndHorizontal();
									DrawTween();
									break;
								}
								case AnimatorControllerParameterType.Int: {
									GUILayoutOption fieldWidth = GUILayout.Width(s_ContextWidth * 0.3F);
									int newArgument = EditorGUILayout.IntField(Target.iArguments[0], fieldWidth);
									if (newArgument != Target.iArguments[0]) {
										Property.RecordForUndo("IArguments");
										Target.iArguments[0] = newArgument;
									}
									bool newIsRelative = DrawToggle(Target.bArguments[0], "相对偏移", BTN_WIDTH_OPTION);
									if (newIsRelative != Target.bArguments[0]) {
										Property.RecordForUndo("BArguments");
										Target.bArguments[0] = newIsRelative;
									}
									EditorGUILayout.EndHorizontal();
									DrawTween();
									if (Target.tween) {
										int toIntType = EditorGUILayout.IntPopup(
											"取整方式",
											Target.iArguments[1],
											new [] { "向下取整", "四舍六入五成双", "向上取整" },
											new [] { -1, 0, 1 }
										);
										if (toIntType != Target.iArguments[1]) {
											Property.RecordForUndo("IArguments");
											Target.iArguments[1] = toIntType;
										}
									}
									break;
								}
								case AnimatorControllerParameterType.Bool: {
									bool bValue = Target.bArguments[0];
									if (!DrawToggle(true, bValue ? "√" : "X", BTN_WIDTH_OPTION)) {
										Property.RecordForUndo("BArguments");
										// ReSharper disable once RedundantAssignment
										Target.bArguments[0] = bValue = !bValue;
									}
									GUILayoutOption blankWidth = GUILayout.Width(s_ContextWidth * 0.3F - 80F - 3F);
									EditorGUILayout.LabelField(string.Empty, blankWidth);
									EditorGUILayout.EndHorizontal();
									break;
								}
								case AnimatorControllerParameterType.Trigger: {
									bool isSet = Target.bArguments[0];
									if (!DrawToggle(true, isSet ? "触发" : "取消", BTN_WIDTH_OPTION)) {
										Property.RecordForUndo("BArguments");
										// ReSharper disable once RedundantAssignment
										Target.bArguments[0] = isSet = !isSet;
									}
									GUILayoutOption blankWidth = GUILayout.Width(s_ContextWidth * 0.3F - 80F - 3F);
									EditorGUILayout.LabelField(string.Empty, blankWidth);
									EditorGUILayout.EndHorizontal();
									break;
								}
								default: {
									EditorGUILayout.EndHorizontal();
									break;
								}
							}
						} else {
							GUILayoutOption blankWidth = GUILayout.Width(s_ContextWidth * 0.3F);
							EditorGUILayout.LabelField(string.Empty, blankWidth);
							EditorGUILayout.EndHorizontal();
						}
						return;
					}
				}
				bool prevEnabled = GUI.enabled;
				GUI.enabled = false;
				EditorGUILayout.Popup(0, new []{ Target.sArguments[0] });
				GUI.enabled = prevEnabled;
				
				GUILayoutOption width = GUILayout.Width(s_ContextWidth * 0.3F);
				EditorGUILayout.LabelField(string.Empty, width);
				EditorGUILayout.EndHorizontal();
			}
		}
		private void DrawAnimatorController() {
			Animator newObj = DrawCompFieldWithThisBtn<Animator>("动画器", Target.obj);
			if (newObj != Target.obj) {
				Property.RecordForUndo("Obj");
				Target.obj = newObj;
			}
		
			if (newObj != null) {
				EditorGUILayout.BeginHorizontal();
				RuntimeAnimatorController newController = DrawObjectFieldWithThisBtn<RuntimeAnimatorController>("状态机", Target.objArguments[0]);
				if (newController != Target.objArguments[0]) {
					Property.RecordForUndo("ObjArguments");
					Target.objArguments[0] = newController;
				}
		
				GUILayoutOption width = GUILayout.Width(s_ContextWidth * 0.3F);
				EditorGUILayout.LabelField(string.Empty, width);
				EditorGUILayout.EndHorizontal();
			}
		}
		private void DrawAnimatorAvatar() {
			Animator newObj = DrawCompFieldWithThisBtn<Animator>("动画器", Target.obj);
			if (newObj != Target.obj) {
				Property.RecordForUndo("Obj");
				Target.obj = newObj;
			}
		
			if (newObj != null) {
				EditorGUILayout.BeginHorizontal();
				Avatar newAvatar = DrawObjectFieldWithThisBtn<Avatar>("形象", Target.objArguments[0]);
				if (newAvatar != Target.objArguments[0]) {
					Property.RecordForUndo("ObjArguments");
					Target.objArguments[0] = newAvatar;
				}
		
				GUILayoutOption width = GUILayout.Width(s_ContextWidth * 0.3F);
				EditorGUILayout.LabelField(string.Empty, width);
				EditorGUILayout.EndHorizontal();
			}
		}
		private void DrawAnimatorApplyRootMotion() {
			Animator newObj = DrawCompFieldWithThisBtn<Animator>("动画器", Target.obj);
			if (newObj != Target.obj) {
				Property.RecordForUndo("Obj");
				Target.obj = newObj;
			}
		
			if (newObj != null) {
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("根移动", CustomEditorGUI.LabelWidthOption);
		
				bool newApplyRootMotion = DrawToggle(Target.bArguments[0], Target.bArguments[0] ? "√" : "X", BTN_WIDTH_OPTION);
				if (newApplyRootMotion != Target.bArguments[0]) {
					Property.RecordForUndo("BArguments");
					Target.bArguments[0] = newApplyRootMotion;
				}
				
				GUILayoutOption width = GUILayout.Width(s_ContextWidth * 0.3F);
				EditorGUILayout.LabelField(string.Empty, width);
				EditorGUILayout.EndHorizontal();
			}
		}
	}
}
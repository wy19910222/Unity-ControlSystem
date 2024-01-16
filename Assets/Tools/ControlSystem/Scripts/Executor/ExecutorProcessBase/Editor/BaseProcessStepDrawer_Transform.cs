/*
 * @Author: wangyun
 * @CreateTime: 2022-12-15 13:06:48 704
 * @LastEditor: wangyun
 * @EditTime: 2023-03-04 17:16:30 922
 */

using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace Control {
	public partial class BaseProcessStepDrawer<TStep> {
		private void DrawTransform() {
			Transform newObj = DrawObjectFieldWithThisBtn<Transform>("游戏对象", Target.obj);
			if (newObj != Target.obj) {
				Property.RecordForUndo("Obj");
				Target.obj = newObj;
			}
			if (newObj != null) {
				int newTransformType = EditorGUILayout.IntPopup(
						"类别",
						Target.iArguments[0],
						new [] { "局部坐标", "局部欧拉角", "缩放", "世界坐标", "世界欧拉角" },
						new [] { 0, 1, 2, 3, 4 }
				);
				if (newTransformType != Target.iArguments[0]) {
					Property.RecordForUndo("IArguments");
					Target.iArguments[0] = newTransformType;
				}
				
				int newPart = DrawEnumButtons("坐标轴", Target.iArguments[1], new [] { "X", "Y", "Z" }, new [] { 1 << 0, 1 << 1, 1 << 2 }, true);
				if (newPart != Target.iArguments[1]) {
					Property.RecordForUndo("IArguments");
					Target.iArguments[1] = newPart;
				}

				if (newPart > 0) {
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("目标类型", CustomEditorGUI.LabelWidthOption);
					bool targetAsValue = Target.bArguments[0];
					if (!DrawToggle(true, targetAsValue ? "对象" : "向量", BTN_WIDTH_OPTION)) {
						Property.RecordForUndo("BArguments");
						Target.bArguments[0] = targetAsValue = !targetAsValue;
					}
					EditorGUILayout.EndHorizontal();
					
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("随机", CustomEditorGUI.LabelWidthOption);
					bool newRandom = DrawToggle(Target.bArguments[1], Target.bArguments[1] ? "√" : "X", BTN_WIDTH_OPTION);
					if (newRandom != Target.bArguments[1]) {
						Property.RecordForUndo("BArguments");
						Target.bArguments[1] = newRandom;
						if (!newRandom) {
							Target.objArguments[1] = null;
						}
					}
					if (newRandom && (newPart & newPart - 1) != 0) {
						bool uniform = Target.bArguments[3];
						if (!DrawToggle(true, uniform ? "一起随机" : "分开随机", BTN_WIDTH_OPTION)) {
							Property.RecordForUndo("BArguments");
							// ReSharper disable once RedundantAssignment
							Target.bArguments[3] = uniform = !uniform;
						}
					}
					EditorGUILayout.EndHorizontal();
	
					if (newRandom) {
						if (targetAsValue) {
							Transform newMin = DrawObjectField<Transform>("从", Target.objArguments[0]);
							if (newMin != Target.objArguments[0]) {
								Property.RecordForUndo("ObjArguments");
								Target.objArguments[0] = newMin;
							}
							Transform newMax = DrawObjectField<Transform>("到", Target.objArguments[1]);
							if (newMax != Target.objArguments[1]) {
								Property.RecordForUndo("ObjArguments");
								Target.objArguments[1] = newMax;
							}
						} else {
							Vector3 min = new Vector3(Target.fArguments[0], Target.fArguments[1], Target.fArguments[2]);
							Vector3 newMin = EditorGUILayout.Vector3Field("从", min);
							if (newMin != min) {
								Property.RecordForUndo("ObjArguments");
								Target.fArguments[0] = newMin.x;
								Target.fArguments[1] = newMin.y;
								Target.fArguments[2] = newMin.z;
							}
							Vector3 max = new Vector3(Target.fArguments[3], Target.fArguments[4], Target.fArguments[5]);
							Vector3 newMax = EditorGUILayout.Vector3Field("到", max);
							if (newMax != max) {
								Property.RecordForUndo("ObjArguments");
								Target.fArguments[3] = newMax.x;
								Target.fArguments[4] = newMax.y;
								Target.fArguments[5] = newMax.z;
							}
						}
					} else {
						if (targetAsValue) {
							Transform newValue = DrawObjectField<Transform>("目标", Target.objArguments[0]);
							if (newValue != Target.objArguments[0]) {
								Property.RecordForUndo("ObjArguments");
								Target.objArguments[0] = newValue;
							}
						} else {
							Vector3 value = new Vector3(Target.fArguments[0], Target.fArguments[1], Target.fArguments[2]);
							Vector3 newValue = EditorGUILayout.Vector3Field("目标", value);
							if (newValue != value) {
								Property.RecordForUndo("ObjArguments");
								Target.fArguments[0] = newValue.x;
								Target.fArguments[1] = newValue.y;
								Target.fArguments[2] = newValue.z;
							}
						}
					}
				
					bool relative = Target.bArguments[2];
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("相对", CustomEditorGUI.LabelWidthOption);
					bool newRelative = DrawToggle(relative, relative ? "√" : "X", BTN_WIDTH_OPTION);
					if (newRelative != relative) {
						Property.RecordForUndo("BArguments");
						Target.bArguments[2] = relative = newRelative;
					}
					EditorGUILayout.EndHorizontal();
					
					DrawTween();
				}
			}
		}
		
		private void DrawLookAt() {
			Transform newObj = DrawObjectFieldWithThisBtn<Transform>("游戏对象", Target.obj);
			if (newObj != Target.obj) {
				Property.RecordForUndo("Obj");
				Target.obj = newObj;
			}
			if (newObj != null) {
				Transform newTarget = DrawObjectField<Transform>("目标", Target.objArguments[0]);
				if (newTarget != Target.objArguments[0]) {
					Property.RecordForUndo("ObjArguments");
					Target.objArguments[0] = newTarget;
				}
				if (newTarget) {
					int newLookPart = EditorGUILayout.IntPopup(
						"朝向轴",
						Target.iArguments[0],
						new [] { "None", "X", "Y", "Z", "-X", "-Y", "-Z" },
						new [] { -1, 0, 1, 2, 3, 4, 5 }
					);
					if (newLookPart != Target.iArguments[0]) {
						Property.RecordForUndo("IArguments");
						Target.iArguments[0] = newLookPart;
					}
					int newUpPart = EditorGUILayout.IntPopup(
						"向上轴",
						Target.iArguments[1],
						new [] { "None", "X", "Y", "Z", "-X", "-Y", "-Z" },
						new [] { -1, 0, 1, 2, 3, 4, 5 }
					);
					if (newUpPart != Target.iArguments[1]) {
						Property.RecordForUndo("IArguments");
						Target.iArguments[1] = newUpPart;
					}
				}
				DrawTween();
			}
		}
		
		private void DrawCameraAnchor() {
			Transform newObj = DrawObjectFieldWithThisBtn<Transform>("游戏对象", Target.obj);
			if (newObj != Target.obj) {
				Property.RecordForUndo("Obj");
				Target.obj = newObj;
			}
			if (newObj != null) {
#if CINEMACHINE_EXIST
				Behaviour newCamera = DrawCompFieldWithThisBtn<Behaviour>("摄像机", Target.objArguments[0], typeof(Camera), typeof(Cinemachine.CinemachineVirtualCamera));
#else
				Camera newCamera = DrawCompFieldWithThisBtn<Camera>("摄像机", Target.objArguments[0]);
#endif
				if (newCamera != Target.objArguments[0]) {
					Property.RecordForUndo("ObjArguments");
					Target.objArguments[0] = newCamera;
				}
				int newPart = DrawEnumButtons("坐标轴", Target.iArguments[0], new [] { "X", "Y", "Z" }, new [] { 1 << 0, 1 << 1, 1 << 2 }, true);
				if (newPart != Target.iArguments[0]) {
					Property.RecordForUndo("IArguments");
					Target.iArguments[0] = newPart;
				}
				if ((newPart & 1 << 0) != 0) {
					float xAnchor = Target.fArguments[0];
					MethodInfo mi = typeof(EditorGUILayout).GetMethod("Slider", BindingFlags.Static | BindingFlags.NonPublic, null,
						new[] { typeof(GUIContent), typeof(float), typeof(float), typeof(float), typeof(float), typeof(float), typeof(GUILayoutOption[]) }, null);
					float newXAnchor = mi == null ?
							EditorGUILayout.FloatField("X方向", xAnchor) :
							(float) mi.Invoke(null, new object[] { new GUIContent("X方向"), xAnchor, 0F, 1F, -Mathf.Infinity, Mathf.Infinity, null });
					if (!Mathf.Approximately(newXAnchor, xAnchor)) {
						Property.RecordForUndo("FArguments");
						Target.fArguments[0] = newXAnchor;
					}
				}
				if ((newPart & 1 << 1) != 0) {
					float yAnchor = Target.fArguments[1];
					MethodInfo mi = typeof(EditorGUILayout).GetMethod("Slider", BindingFlags.Static | BindingFlags.NonPublic, null,
						new[] { typeof(GUIContent), typeof(float), typeof(float), typeof(float), typeof(float), typeof(float), typeof(GUILayoutOption[]) }, null);
					float newYAnchor = mi == null ?
							EditorGUILayout.FloatField("Y方向", yAnchor) :
							(float) mi.Invoke(null, new object[] { new GUIContent("Y方向"), yAnchor, 0F, 1F, -Mathf.Infinity, Mathf.Infinity, null });
					if (!Mathf.Approximately(newYAnchor, yAnchor)) {
						Property.RecordForUndo("FArguments");
						Target.fArguments[1] = newYAnchor;
					}
				}
				if ((newPart & 1 << 2) != 0) {
					float newZAnchor = EditorGUILayout.FloatField("Z方向", Target.fArguments[2]);
					if (!Mathf.Approximately(newZAnchor, Target.fArguments[2])) {
						Property.RecordForUndo("FArguments");
						Target.fArguments[2] = newZAnchor;
					}
				}
				DrawTween();
			}
		}
	}
}
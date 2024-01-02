/*
 * @Author: wangyun
 * @CreateTime: 2022-12-15 11:47:18 527
 * @LastEditor: wangyun
 * @EditTime: 2023-03-04 17:16:10 446
 */

using UnityEngine;
using UnityEditor;

namespace Control {
	public partial class ProcessStepDrawerBase<TStep> {
		private void DrawInstantiate() {
			Transform newObj = DrawObjectField<Transform>("预制体", Target.obj);
			if (newObj != Target.obj) {
				Property.RecordForUndo("Obj");
				Target.obj = newObj;
			}
			if (newObj != null) {
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("父节点", CustomEditorGUI.LabelWidthOption);
				bool specifyParent = Target.bArguments[0];
				if (!DrawToggle(true, specifyParent ? "指定节点" : "当前节点", BTN_WIDTH_OPTION)) {
					Property.RecordForUndo("BArguments");
					Target.bArguments[0] = specifyParent = !specifyParent;
				}
				if (specifyParent) {
					Transform newParent = DrawObjectField<Transform>("", Target.objArguments[0]);
					if (newParent != Target.objArguments[0]) {
						Property.RecordForUndo("ObjArguments");
						Target.objArguments[0] = newParent;
					}
				} else {
					Target.objArguments[0] = null;
				}
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("位置", CustomEditorGUI.LabelWidthOption);
				bool newResetPos = DrawToggle(Target.bArguments[1], "重置", BTN_WIDTH_OPTION);
				if (newResetPos != Target.bArguments[1]) {
					Property.RecordForUndo("BArguments");
					Target.bArguments[1] = newResetPos;
				}
				if (newResetPos) {
					Transform newParent = DrawObjectField<Transform>("", Target.objArguments[1]);
					if (newParent != Target.objArguments[1]) {
						Property.RecordForUndo("ObjArguments");
						Target.objArguments[1] = newParent;
					}
				} else {
					Target.objArguments[1] = null;
				}
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("旋转", CustomEditorGUI.LabelWidthOption);
				bool newResetRot = DrawToggle(Target.bArguments[2], "重置", BTN_WIDTH_OPTION);
				if (newResetRot != Target.bArguments[2]) {
					Property.RecordForUndo("BArguments");
					Target.bArguments[2] = newResetRot;
				}
				if (newResetRot) {
					Transform newParent = DrawObjectField<Transform>("", Target.objArguments[2]);
					if (newParent != Target.objArguments[2]) {
						Property.RecordForUndo("ObjArguments");
						Target.objArguments[2] = newParent;
					}
				} else {
					Target.objArguments[2] = null;
				}
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("缩放", CustomEditorGUI.LabelWidthOption);
				bool newResetScale = DrawToggle(Target.bArguments[3], "重置", BTN_WIDTH_OPTION);
				if (newResetScale != Target.bArguments[3]) {
					Property.RecordForUndo("BArguments");
					Target.bArguments[3] = newResetScale;
				}
				if (newResetScale) {
					Transform newParent = DrawObjectField<Transform>("", Target.objArguments[3]);
					if (newParent != Target.objArguments[3]) {
						Property.RecordForUndo("ObjArguments");
						Target.objArguments[3] = newParent;
					}
				} else {
					Target.objArguments[3] = null;
				}
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("活跃状态", CustomEditorGUI.LabelWidthOption);
				bool newActiveAtOnce = DrawToggle(Target.bArguments[4], Target.bArguments[4] ? "设为活跃" : "不改变", BTN_WIDTH_OPTION);
				if (newActiveAtOnce != Target.bArguments[4]) {
					Property.RecordForUndo("BArguments");
					Target.bArguments[4] = newActiveAtOnce;
				}
				EditorGUILayout.EndHorizontal();
			}
		}
		
		private void DrawDestroy() {
			GameObject newObj = DrawObjectField<GameObject>("游戏对象", Target.obj);
			if (newObj != Target.obj) {
				Property.RecordForUndo("Obj");
				Target.obj = newObj;
			}
			if (newObj != null) {
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("销毁目标", CustomEditorGUI.LabelWidthOption);

				bool newOnlyDestroyChildren = DrawToggle(Target.bArguments[0], "仅子节点", BTN_WIDTH_OPTION);
				if (newOnlyDestroyChildren != Target.bArguments[0]) {
					Property.RecordForUndo("BArguments");
					Target.bArguments[0] = newOnlyDestroyChildren;
				}
				
				GUILayoutOption width = GUILayout.Width(s_ContextWidth * 0.3F);
				EditorGUILayout.LabelField("", width);
				EditorGUILayout.EndHorizontal();
			}
		}
		
		private void DrawParent() {
			Transform newObj = DrawObjectField<Transform>("游戏对象", Target.obj);
			if (newObj != Target.obj) {
				Property.RecordForUndo("Obj");
				Target.obj = newObj;
			}
			if (newObj != null) {
				EditorGUILayout.BeginHorizontal();
				Transform newParent = DrawObjectField<Transform>("父节点", Target.objArguments[0]);
				if (newParent != Target.objArguments[0]) {
					Property.RecordForUndo("ObjArguments");
					Target.objArguments[0] = newParent;
				}
				
				bool newWorldPositionStays = DrawToggle(Target.bArguments[0], "留在原地", BTN_WIDTH_OPTION);
				if (newWorldPositionStays != Target.bArguments[0]) {
					Property.RecordForUndo("BArguments");
					Target.bArguments[0] = newWorldPositionStays;
				}
				EditorGUILayout.LabelField("", GUILayout.Width(s_ContextWidth * 0.3F - 80F - 3F));
				EditorGUILayout.EndHorizontal();

				if (newParent) {
					EditorGUILayout.BeginHorizontal();
					int newSiblingIndex = EditorGUILayout.IntField("节点序号", Target.iArguments[0]);
					if (newSiblingIndex != Target.iArguments[0]) {
						Property.RecordForUndo("IArguments");
						Target.iArguments[0] = newSiblingIndex;
					}
					EditorGUILayout.LabelField("", GUILayout.Width(s_ContextWidth * 0.3F));
					EditorGUILayout.EndHorizontal();
				}
			}
		}
		
		private void DrawActive() {
			GameObject newObj = DrawObjectField<GameObject>("游戏对象", Target.obj);
			if (newObj != Target.obj) {
				Property.RecordForUndo("Obj");
				Target.obj = newObj;
			}
			if (newObj != null) {
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("活跃", CustomEditorGUI.LabelWidthOption);
				
				bool newActive = DrawToggle(Target.bArguments[0], Target.bArguments[0] ? "√" : "X", BTN_WIDTH_OPTION);
				if (newActive != Target.bArguments[0]) {
					Property.RecordForUndo("BArguments");
					Target.bArguments[0] = newActive;
				}
				
				GUILayoutOption width = GUILayout.Width(s_ContextWidth * 0.3F);
				EditorGUILayout.LabelField("", width);
				EditorGUILayout.EndHorizontal();
			}
		}
		
		private void DrawEnabled() {
			Component newObj = DrawCompField<Component>("组件", Target.obj, typeof(Behaviour), typeof(Renderer), typeof(Collider), typeof(LODGroup), typeof(Cloth));
			if (newObj != Target.obj) {
				Property.RecordForUndo("Obj");
				Target.obj = newObj;
			}
			if (newObj != null) {
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("启用", CustomEditorGUI.LabelWidthOption);
				
				bool newEnabled = DrawToggle(Target.bArguments[0], Target.bArguments[0] ? "√" : "X", BTN_WIDTH_OPTION);
				if (newEnabled != Target.bArguments[0]) {
					Property.RecordForUndo("BArguments");
					Target.bArguments[0] = newEnabled;
				}
				
				GUILayoutOption width = GUILayout.Width(s_ContextWidth * 0.3F);
				EditorGUILayout.LabelField("", width);
				EditorGUILayout.EndHorizontal();
			}
		}
	}
}
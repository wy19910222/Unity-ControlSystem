/*
 * @Author: wangyun
 * @CreateTime: 2022-12-15 12:41:44 458
 * @LastEditor: wangyun
 * @EditTime: 2023-03-04 17:18:31 001
 */

#if SPINE_EXIST
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Spine;
using Spine.Unity;

namespace Control {
	public partial class ProcessStepDrawerBase<TStep> {
		private void DrawSpineSetValue() {
			Behaviour newObj = DrawCompFieldWithThisBtn<Behaviour>("骨骼动画", Target.obj, typeof(SkeletonAnimation), typeof(SkeletonGraphic));
			if (newObj != Target.obj) {
				Property.RecordForUndo("Obj");
				Target.obj = newObj;
			}
			if (newObj != null) {
				EditorGUILayout.BeginHorizontal();
				int newCtrlType = EditorGUILayout.IntPopup(
					"变量",
					Target.iArguments[0],
					new [] { "动画名称", "播放速度", "是否循环" },
					new [] { 0, 1, 2 }
				);
				if (newCtrlType != Target.iArguments[0]) {
					Property.RecordForUndo("IArguments");
					Target.iArguments[0] = newCtrlType;
				}

				switch (newCtrlType) {
					case 0:
						SkeletonData data = null;
						switch (newObj) {
							case SkeletonAnimation sa:
								data = sa.skeleton?.Data;
								break;
							case SkeletonGraphic sg:
								data = sg.SkeletonData;
								break;
						}
						List<string> names = data == null ? new List<string>() : new List<Spine.Animation>(data.Animations).ConvertAll(animation => animation.Name);
					
						EditorGUI.BeginDisabledGroup(names.Count <= 0);
						List<string> options = new List<string>(names);
						names.Insert(0, string.Empty);
						options.Insert(0, "无");
						int index = names.IndexOf(Target.sArguments[0]);
						int newIndex = EditorGUILayout.Popup(index, options.ToArray(), GUILayout.Width(s_ContextWidth * 0.3F));
						if (newIndex != index) {
							Target.sArguments[0] = names[newIndex];
						}
						EditorGUI.EndDisabledGroup();
						break;
					case 1:
						float newTimeScale = EditorGUILayout.FloatField(Target.fArguments[0], GUILayout.Width(s_ContextWidth * 0.3F));
						if (!Mathf.Approximately(newTimeScale, Target.fArguments[0])) {
							Property.RecordForUndo("FArguments");
							Target.fArguments[0] = newTimeScale;
						}
						break;
					case 2:
						bool newLoop = DrawToggle(Target.bArguments[0], Target.bArguments[0] ? "√" : "X", GUILayout.Width(s_ContextWidth * 0.18F));
						if (newLoop != Target.bArguments[0]) {
							Property.RecordForUndo("BArguments");
							Target.bArguments[0] = newLoop;
						}

						if (Target.bArguments.Count < 2) {
							Target.bArguments.Add(false);
						}
						if (newLoop) {
							bool newResume = DrawToggle(Target.bArguments[1], "继续", GUILayout.Width(s_ContextWidth * 0.12F - 3F));
							if (newResume != Target.bArguments[1]) {
								Property.RecordForUndo("BArguments");
								Target.bArguments[1] = newResume;
							}
						} else {
							bool newStop = DrawToggle(Target.bArguments[1], "停止", GUILayout.Width(s_ContextWidth * 0.12F - 3F));
							if (newStop != Target.bArguments[1]) {
								Property.RecordForUndo("BArguments");
								Target.bArguments[1] = newStop;
							}
						}
						break;
				}
				EditorGUILayout.EndHorizontal();
			}
		}
		
		private void DrawSpineRestart() {
			Behaviour newObj = DrawCompFieldWithThisBtn<Behaviour>("骨骼动画", Target.obj, typeof(SkeletonAnimation), typeof(SkeletonGraphic));
			if (newObj != Target.obj) {
				Property.RecordForUndo("Obj");
				Target.obj = newObj;
			}
			if (newObj != null) {
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("特殊情况", CustomEditorGUI.LabelWidthOption);
				bool newWhenIsNotStarted = DrawToggle(Target.bArguments[0], "没播过时");
				if (newWhenIsNotStarted != Target.bArguments[0]) {
					Property.RecordForUndo("BArguments");
					Target.bArguments[0] = newWhenIsNotStarted;
				}
				bool newWhenIsNotComplete = DrawToggle(Target.bArguments[1], "播到一半时");
				if (newWhenIsNotComplete != Target.bArguments[1]) {
					Property.RecordForUndo("BArguments");
					Target.bArguments[1] = newWhenIsNotComplete;
				}
				EditorGUILayout.EndHorizontal();
			}
		}

		private void DrawSpineStop() {
			Behaviour newObj = DrawCompFieldWithThisBtn<Behaviour>("骨骼动画", Target.obj, typeof(SkeletonAnimation), typeof(SkeletonGraphic));
			if (newObj != Target.obj) {
				Property.RecordForUndo("Obj");
				Target.obj = newObj;
			}
		}
	}
}
#endif
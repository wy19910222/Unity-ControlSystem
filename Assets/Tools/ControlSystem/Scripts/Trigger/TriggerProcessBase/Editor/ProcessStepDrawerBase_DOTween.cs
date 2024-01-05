/*
 * @Author: wangyun
 * @CreateTime: 2022-12-15 12:40:42 307
 * @LastEditor: wangyun
 * @EditTime: 2023-03-04 17:18:08 369
 */

using UnityEngine;
using UnityEditor;
using DG.Tweening;
using DG.Tweening.Core;

namespace Control {
	public partial class ProcessStepDrawerBase<TStep> {
		private void DrawDOTweenRestart() {
			ABSAnimationComponent newObj = DrawCompFieldWithThisBtn<ABSAnimationComponent>("缓动器", Target.obj);
			if (newObj != Target.obj) {
				Property.RecordForUndo("Obj");
				Target.obj = newObj;
			}
			if (newObj != null) {
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("参数", CustomEditorGUI.LabelWidthOption);
		
				bool newIncludeDelay = DrawToggle(Target.bArguments[0], "Include Delay", GUILayout.Width(90F));
				if (newIncludeDelay != Target.bArguments[0]) {
					Property.RecordForUndo("BArguments");
					Target.bArguments[0] = newIncludeDelay;
				}
					
				bool fromHereValid = false;
				switch (newObj) {
					case DOTweenAnimation anim when anim.isRelative: {
						if (!(anim.animationType == DOTweenAnimation.AnimationType.None ||
								anim.animationType > DOTweenAnimation.AnimationType.LocalMove &&
								anim.animationType <= DOTweenAnimation.AnimationType.UIWidthHeight)) {
							fromHereValid = true;
						}
						break;
					}
					case DOTweenPath path when path.relative && !path.isLocal:
						fromHereValid = true;
						break;
				}
				bool prevEnabled = GUI.enabled;
				GUI.enabled = fromHereValid && prevEnabled;
				bool newFromHere = DrawToggle(Target.bArguments[1], "From Here", BTN_WIDTH_OPTION);
				if (newFromHere != Target.bArguments[1]) {
					Property.RecordForUndo("BArguments");
					Target.bArguments[1] = newFromHere;
				}
				GUI.enabled = prevEnabled;
				EditorGUILayout.EndHorizontal();
			}
		}
		private void DrawDOTweenCtrl() {
			ABSAnimationComponent newObj = DrawCompFieldWithThisBtn<ABSAnimationComponent>("缓动器", Target.obj);
			if (newObj != Target.obj) {
				Property.RecordForUndo("Obj");
				Target.obj = newObj;
			}
			if (newObj != null) {
				int newCtrlType = EditorGUILayout.IntPopup(
					"操作",
					Target.iArguments[0],
					new [] { "正播", "倒播", "暂停", "继续", "暂停或继续" },
					new [] { 0, 1, 2, 3, 4 }
				);
				if (newCtrlType != Target.iArguments[0]) {
					Property.RecordForUndo("IArguments");
					Target.iArguments[0] = newCtrlType;
				}
			}
		}
		private void DrawDOTweenGoto() {
			ABSAnimationComponent newObj = DrawCompFieldWithThisBtn<ABSAnimationComponent>("缓动器", Target.obj);
			if (newObj != Target.obj) {
				Property.RecordForUndo("Obj");
				Target.obj = newObj;
			}
			if (newObj != null) {
				EditorGUILayout.BeginHorizontal();
				bool loop = false;
				switch (newObj) {
					case DOTweenAnimation doTweenAnim:
						loop = doTweenAnim.loops == -1;
						break;
					case DOTweenPath doTweenPath:
						loop = doTweenPath.loops == -1;
						break;
				}
				
				float newTime = !loop && Target.bArguments[0] ? EditorGUILayout.Slider("进度", Target.fArguments[0], 0, 1) :
						Mathf.Max(EditorGUILayout.FloatField("进度", Target.fArguments[0]), 0);
				if (!Mathf.Approximately(newTime, Target.fArguments[0])) {
					Property.RecordForUndo("FArguments");
					Target.fArguments[0] = newTime;
				}
		
				bool prevEnabled = GUI.enabled;
				GUI.enabled = !loop && prevEnabled;
				bool newIsPercent = DrawToggle(Target.bArguments[0] && !loop, "%", BTN_WIDTH_OPTION);
				if (newIsPercent != Target.bArguments[0]) {
					Property.RecordForUndo("BArguments");
					Target.bArguments[0] = newIsPercent;
				}
				GUI.enabled = prevEnabled;
				
				GUILayoutOption width = GUILayout.Width(s_ContextWidth * 0.3F - 80F - 3F);
				EditorGUILayout.LabelField(string.Empty, width);
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("其他参数", CustomEditorGUI.LabelWidthOption);
				bool newPauseIfPlaying = DrawToggle(Target.bArguments[1], "暂停", BTN_WIDTH_OPTION);
				if (newPauseIfPlaying != Target.bArguments[1]) {
					Property.RecordForUndo("BArguments");
					Target.bArguments[1] = newPauseIfPlaying;
				}
				if (newTime == 0) {
					bool newIncludeDelay = DrawToggle(Target.bArguments[2], "包括延迟", BTN_WIDTH_OPTION);
					if (newIncludeDelay != Target.bArguments[2]) {
						Property.RecordForUndo("BArguments");
						Target.bArguments[2] = newIncludeDelay;
					}
				}
				EditorGUILayout.EndHorizontal();
				
			}
		}
		private void DrawDOTweenLife() {
			ABSAnimationComponent newObj = DrawCompFieldWithThisBtn<ABSAnimationComponent>("缓动器", Target.obj);
			if (newObj != Target.obj) {
				Property.RecordForUndo("Obj");
				Target.obj = newObj;
			}
			if (newObj) {
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("操作", CustomEditorGUI.LabelWidthOption);
				int lifeType = newObj is DOTweenAnimation ? Target.iArguments[0] : 0;
				if (DrawToggle(lifeType == 0, "杀掉")) {
					lifeType = 0;
				}
				if (DrawToggle(lifeType == 1, "重生")) {
					lifeType = 1;
				}
				if (lifeType != Target.iArguments[0]) {
					Property.RecordForUndo("IArguments");
					Target.iArguments[0] = lifeType;
				}
				EditorGUILayout.EndHorizontal();

				if (lifeType == 0) {
					bool loop = false;
					switch (newObj) {
						case DOTweenAnimation doTweenAnim:
							loop = doTweenAnim.loops == -1;
							break;
						case DOTweenPath doTweenPath:
							loop = doTweenPath.loops == -1;
							break;
					}
					if (!loop) {
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("直接结束", CustomEditorGUI.LabelWidthOption);
						bool newComplete = DrawToggle(Target.bArguments[0], Target.bArguments[0] ? "√" : "X", BTN_WIDTH_OPTION);
						if (newComplete != Target.bArguments[0]) {
							Property.RecordForUndo("IArguments");
							Target.bArguments[0] = newComplete;
						}
						EditorGUILayout.EndHorizontal();
					}
				}
			}
		}
	}
}
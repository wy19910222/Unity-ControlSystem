/*
 * @Author: wangyun
 * @CreateTime: 2022-12-15 12:39:59 659
 * @LastEditor: wangyun
 * @EditTime: 2023-03-04 17:17:46 389
 */

using UnityEngine;
using UnityEngine.Playables;
using UnityEditor;

namespace Control {
	public partial class ProcessStepDrawerBase<TStep> {
		private void DrawPlayableCtrl() {
			PlayableDirector newObj = DrawCompField<PlayableDirector>("导演", Target.obj);
			if (newObj != Target.obj) {
				Property.RecordForUndo("Obj");
				Target.obj = newObj;
			}
			if (newObj != null) {
				int newCtrlType = EditorGUILayout.IntPopup(
					"操作",
					Target.iArguments[0],
					new [] { "播放", "停止", "暂停", "继续", "暂停或继续" },
					new [] { 0, 1, 2, 3, 4 }
				);
				if (newCtrlType != Target.iArguments[0]) {
					Property.RecordForUndo("IArguments");
					Target.iArguments[0] = newCtrlType;
				}
			}
		}
		
		private void DrawPlayableGoto() {
			PlayableDirector newObj = DrawCompField<PlayableDirector>("导演", Target.obj);
			if (newObj != Target.obj) {
				Property.RecordForUndo("Obj");
				Target.obj = newObj;
			}
			if (newObj != null) {
				EditorGUILayout.BeginHorizontal();
				float newTime = Target.bArguments[0] ? EditorGUILayout.Slider("进度", Target.fArguments[0], 0, 1) :
						Mathf.Max(EditorGUILayout.FloatField("进度", Target.fArguments[0]), 0);
				if (!Mathf.Approximately(newTime, Target.fArguments[0])) {
					Property.RecordForUndo("FArguments");
					Target.fArguments[0] = newTime;
				}
		
				bool newIsPercent = DrawToggle(Target.bArguments[0], "%", BTN_WIDTH_OPTION);
				if (newIsPercent != Target.bArguments[0]) {
					Property.RecordForUndo("BArguments");
					Target.bArguments[0] = newIsPercent;
				}
				
				GUILayoutOption width = GUILayout.Width(s_ContextWidth * 0.3F - 80F - 3F);
				EditorGUILayout.LabelField("", width);
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("刷新", CustomEditorGUI.LabelWidthOption);
				bool newEvaluate = DrawToggle(Target.bArguments[1], Target.bArguments[1] ? "√" : "X", BTN_WIDTH_OPTION);
				if (newEvaluate != Target.bArguments[1]) {
					Property.RecordForUndo("BArguments");
					Target.bArguments[1] = newEvaluate;
				}
				EditorGUILayout.EndHorizontal();
			}
		}
	}
}
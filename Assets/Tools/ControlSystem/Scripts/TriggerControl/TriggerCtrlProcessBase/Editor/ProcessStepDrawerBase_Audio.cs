/*
 * @Author: wangyun
 * @CreateTime: 2022-12-15 11:47:24 814
 * @LastEditor: wangyun
 * @EditTime: 2023-03-05 16:55:26 513
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Control {
	public partial class ProcessStepDrawerBase<TStep> {
		private void DrawAudioOneShot() {
			AudioClip newObj = DrawObjectField<AudioClip>("音频", Target.obj);
			if (newObj != Target.obj) {
				Property.RecordForUndo("Obj");
				Target.obj = newObj;
			}
			if (newObj != null) {
				EditorGUILayout.BeginHorizontal();
				float newVolumeScale = EditorGUILayout.FloatField("音量", Target.fArguments[0]);
				if (!Mathf.Approximately(newVolumeScale, Target.fArguments[0])) {
					Property.RecordForUndo("FArguments");
					Target.fArguments[0] = newVolumeScale;
				}
				GUILayoutOption width = GUILayout.Width(s_ContextWidth * 0.3F);
				EditorGUILayout.LabelField(string.Empty, width);
				EditorGUILayout.EndHorizontal();
			}
		}
		private void DrawAudioSourceCtrl() {
			AudioSource newObj = DrawCompFieldWithThisBtn<AudioSource>("音源", Target.obj);
			if (newObj != Target.obj) {
				Property.RecordForUndo("Obj");
				Target.obj = newObj;
			}
			if (newObj != null) {
				EditorGUILayout.BeginHorizontal();
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
				float newFadeDuration = EditorGUILayout.FloatField("渐变时长", Target.fArguments[0], GUILayout.Width(s_ContextWidth * 0.3F));
				if (!Mathf.Approximately(newFadeDuration, Target.fArguments[0])) {
					Property.RecordForUndo("FArguments");
					Target.fArguments[0] = newFadeDuration;
				}
				EditorGUILayout.EndHorizontal();
			}
		}
		
		// DrawAudiosPlay
		private ReorderableList m_AudioList;
		private void DrawAudiosPlay() {
			if (m_AudioList == null) {
				m_AudioList = new ReorderableList(Target.objArguments, typeof(Object), true, true, false, false) {
					drawHeaderCallback = rect => {
						Rect leftRect = new Rect(rect.x, rect.y, rect.width, rect.height);
						EditorGUI.LabelField(leftRect, $"音频列表({Target.objArguments.Count})");
						Rect middleRect = new Rect(rect.x + rect.width - 124 - 1, rect.y - 1, 100, rect.height + 2);
						if (GUI.Button(middleRect, "添加选中对象")) {
							List<AudioClip> list = new List<AudioClip>();
							foreach (var obj in Selection.objects) {
								if (obj is AudioClip clip) {
									list.Add(clip);
								}
							}
							Target.objArguments.AddRange(list);
						}
						Rect rightRect = new Rect(middleRect.x + middleRect.width + 1, rect.y - 1, 30, rect.height + 2);
						if (GUI.Button(rightRect, "+")) {
							int count = Target.objArguments.Count;
							Target.objArguments.Add(count > 0 ? Target.objArguments[count - 1] : null);
						}
					},
					drawElementCallback = (rect, index, isActive, isFocused) => {
						Rect leftRect = new Rect(rect.x, rect.y + 1, rect.width - 24, rect.height - 2);
						Target.objArguments[index] = DrawObjectField<AudioClip>(leftRect, "", Target.objArguments[index]);
						Rect rightRect = new Rect(leftRect.x + leftRect.width + 2, rect.y + 1, 28, rect.height - 2);
						if (GUI.Button(rightRect, "×")) {
							EditorApplication.delayCall += () => Target.objArguments.RemoveAt(index);
						}
					},
					elementHeight = 20, footerHeight = 0
				};
			}
			m_AudioList.list = Target.objArguments;
			Property.RecordForUndo("ObjArguments");
			m_AudioList.DoLayoutList();
			int totalCount = Target.objArguments.Count;
			if (totalCount > 1) {
				int newPlayCount = Mathf.Max(EditorGUILayout.IntField("播放个数", Target.iArguments[0]), 1);
				if (newPlayCount != Target.iArguments[0]) {
					Property.RecordForUndo("IArguments");
					Target.iArguments[0] = newPlayCount;
				}
				
				int playType = DrawEnumButtons("其他参数", Target.iArguments[1], new [] { "依次播放", "随机播放" }, new [] { 0, 1 });
				if (playType != Target.iArguments[1]) {
					Property.RecordForUndo("IArguments");
					Target.iArguments[1] = playType;
					Target.iArguments[2] = 0;
				}
				switch (playType) {
					case 0: {
						int shuffleType = DrawEnumButtons("洗牌", Target.iArguments[2], new [] { "不洗牌", "初始时洗一次", "循环时洗牌" }, new [] { 0, 1, 2 });
						if (shuffleType != Target.iArguments[2]) {
							Property.RecordForUndo("IArguments");
							Target.iArguments[2] = shuffleType;
						}
						break;
					}
					case 1: {
						if (newPlayCount + newPlayCount <= totalCount) {
							int randomType = DrawEnumButtons("限制", Target.iArguments[2], new [] { "无限制", "不重复" }, new [] { 0, 1 });
							if (randomType != Target.iArguments[2]) {
								Property.RecordForUndo("IArguments");
								Target.iArguments[2] = randomType;
							}
						}
						break;
					}
				}
			}
		}

	}
}
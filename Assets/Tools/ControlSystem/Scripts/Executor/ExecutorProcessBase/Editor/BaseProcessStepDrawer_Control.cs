/*
 * @Author: wangyun
 * @CreateTime: 2022-12-15 12:43:22 468
 * @LastEditor: wangyun
 * @EditTime: 2023-03-04 17:16:04 174
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Control {
	public partial class BaseProcessStepDrawer<TStep> {
		private ReorderableList m_ExecutorList;
		private void DrawExecutor() {
			if (m_ExecutorList == null) {
				m_ExecutorList = new ReorderableList(Target.objArguments, typeof(UnityEngine.Object), true, true, false, false) {
					drawHeaderCallback = rect => {
						Rect leftRect = new Rect(rect.x, rect.y, rect.width, rect.height);
						EditorGUI.LabelField(leftRect, $"执行器列表({Target.objArguments.Count})");
						Rect middleRect = new Rect(rect.x + rect.width - 124 - 1, rect.y - 1, 100, rect.height + 2);
						if (GUI.Button(middleRect, "添加选中对象")) {
							List<BaseExecutor> list = new List<BaseExecutor>();
							foreach (var obj in Selection.objects) {
								switch (obj) {
									case GameObject go: {
										BaseExecutor executor = go.GetComponent<BaseExecutor>();
										if (executor) {
											list.Add(executor);
										}
										break;
									}
									case Component comp: {
										if (comp is BaseExecutor executor) {
											list.Add(executor);
										} else {
											executor = comp.GetComponent<BaseExecutor>();
											if (executor) {
												list.Add(executor);
											}
										}
										break;
									}
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
						Target.objArguments[index] = DrawCompField<BaseExecutor>(leftRect, "", Target.objArguments[index]);
						Rect rightRect = new Rect(leftRect.x + leftRect.width + 2, rect.y + 1, 28, rect.height - 2);
						if (GUI.Button(rightRect, "×")) {
							EditorApplication.delayCall += () => Target.objArguments.RemoveAt(index);
						}
					},
					elementHeight = 20, footerHeight = 0
				};
			}
			m_ExecutorList.list = Target.objArguments;
			Property.RecordForUndo("ObjArguments");
			m_ExecutorList.DoLayoutList();
			int totalCount = Target.objArguments.Count;
			if (totalCount > 1) {
				int newExecutorCount = Mathf.Max(EditorGUILayout.IntField("执行个数", Target.iArguments[0]), 1);
				if (newExecutorCount != Target.iArguments[0]) {
					Property.RecordForUndo("IArguments");
					Target.iArguments[0] = newExecutorCount;
				}
				
				int executeType = DrawEnumButtons("其他参数", Target.iArguments[1], new [] { "依次执行", "随机执行" }, new [] { 0, 1 });
				if (executeType != Target.iArguments[1]) {
					Property.RecordForUndo("IArguments");
					Target.iArguments[1] = executeType;
					Target.iArguments[2] = 0;
				}
				switch (executeType) {
					case 0: {
						int shuffleType = DrawEnumButtons("洗牌", Target.iArguments[2], new [] { "不洗牌", "初始时洗一次", "循环时洗牌" }, new [] { 0, 1, 2 });
						if (shuffleType != Target.iArguments[2]) {
							Property.RecordForUndo("IArguments");
							Target.iArguments[2] = shuffleType;
						}
						break;
					}
					case 1: {
						if (newExecutorCount + newExecutorCount <= totalCount) {
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

		private void DrawStateController() {
			StateController newObj = DrawCompFieldWithThisBtn<StateController>("控制器", Target.obj);
			if (newObj != Target.obj) {
				Property.RecordForUndo("Obj");
				Target.obj = newObj;
			}
			if (newObj != null) {
				List<State> states = newObj.states;
				int stateCount = states.Count;
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("赋值", CustomEditorGUI.LabelWidthOption);
				bool newRelative = DrawToggle(Target.bArguments[0], "相对偏移");
				if (newRelative != Target.bArguments[0]) {
					Property.RecordForUndo("BArguments");
					Target.bArguments[0] = newRelative;
				}
				
				bool recordIndex = Target.bArguments[1];
				if (newRelative) {
					GUILayoutUtility.GetRect(EditorGUIUtility.TrTempContent("以索引形式储存"), "Button");
				} else {
					bool newRecordIndex = DrawToggle(recordIndex, "以索引形式储存");
					if (newRecordIndex != recordIndex) {
						Property.RecordForUndo("BArguments");
						Target.bArguments[1] = newRecordIndex;
						Property.RecordForUndo("IArguments");
						if (newRecordIndex) {
							// uid -> index
							Dictionary<int, int> uidIndexDict = new Dictionary<int, int>();
							for (int i = 0; i < stateCount; ++i) {
								State state = states[i];
								uidIndexDict.Add(state.uid, i);
							}
							List<int> indexes = new List<int>();
							for (int i = 0, length = Target.iArguments.Count; i < length; ++i) {
								if (uidIndexDict.TryGetValue(Target.iArguments[i], out int index)) {
									indexes.Add(index);
								}
							}
							Target.iArguments = indexes;
						} else {
							// index -> uid
							List<int> allUIDs = states.ConvertAll(state => state.uid);
							List<int> uids = new List<int>();
							for (int i = 0, length = Target.iArguments.Count; i < length; ++i) {
								if (Target.iArguments[i] < stateCount) {
									uids.Add(allUIDs[Target.iArguments[i]]);
								}
							}
							Target.iArguments = uids;
						}
					}
				}
				EditorGUILayout.LabelField("", GUILayout.Width(s_ContextWidth * 0.3F));
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				if (newRelative) {
					int newOffset = EditorGUILayout.IntField("偏移", Target.iArguments[0]);
					if (newOffset != Target.iArguments[0]) {
						Property.RecordForUndo("IArguments");
						Target.iArguments[0] = newOffset;
					}
					bool newLoop = DrawToggle(Target.bArguments[1], "循环", BTN_WIDTH_OPTION);
					if (newLoop != Target.bArguments[1]) {
						Property.RecordForUndo("BArguments");
						// ReSharper disable once RedundantAssignment
						Target.bArguments[1] = newLoop;
					}
					EditorGUILayout.LabelField("", GUILayout.Width(s_ContextWidth * 0.3F - 80F - 3F));
				} else {
					string[] options = new string[stateCount];
					bool anyNameExist = states.Exists(state => !string.IsNullOrEmpty(state.name));
					for (int i = 0; i < stateCount; ++i) {
						string ext = anyNameExist ? states[i].name : states[i].desc;
						if (!string.IsNullOrEmpty(ext)) {
							ext = ":" + ext;
						}
						options[i] = i + ext;
					}
					
					bool random = Target.bArguments[2];
					if (random) {
						EditorGUILayout.LabelField("状态", CustomEditorGUI.LabelWidthOption);
						if (recordIndex) {
							List<int> indexes = new List<int>();
							for (int i = 0; i < stateCount; ++i) {
								indexes.Add(i);
							}
							int[] selectedIndexes = Target.iArguments.Contains(-1) ? indexes.ToArray() : Target.iArguments.ToArray();
							IntPopupMultiSelect(selectedIndexes, options, indexes.ToArray(), newSelectedIndexes => {
								Property.RecordForUndo("IArguments");
								Target.iArguments.Clear();
								if (newSelectedIndexes.Length == indexes.Count) {
									Target.iArguments.Add(-1);
								} else {
									Target.iArguments.AddRange(newSelectedIndexes);
								}
							});
						} else {
							List<int> uids = states.ConvertAll(state => state.uid);
							int[] selectedUIDs = Target.iArguments.Contains(-1) ? uids.ToArray() : Target.iArguments.ToArray();
							IntPopupMultiSelect(selectedUIDs, options, uids.ToArray(), newSelectedUIDs => {
								Property.RecordForUndo("IArguments");
								Target.iArguments.Clear();
								if (newSelectedUIDs.Length == uids.Count) {
									Target.iArguments.Add(-1);
								} else {
									Target.iArguments.AddRange(newSelectedUIDs);
								}
							});
						}
					} else {
						int index = recordIndex ? Target.iArguments[0] : states.FindIndex(state => state.uid == Target.iArguments[0]);
						int newStateIndex = EditorGUILayout.Popup("状态", index, options);
						if (newStateIndex != index) {
							Property.RecordForUndo("IArguments");
							Target.iArguments[0] = recordIndex ? newStateIndex : states[newStateIndex].uid;
						}
					}

					bool newRandom = DrawToggle(random, "随机", BTN_WIDTH_OPTION);
					if (newRandom != random) {
						Property.RecordForUndo("BArguments");
						Target.bArguments[2] = newRandom;
					}
					if (newRandom) {
						GUILayoutOption noRepeatBtnOption = GUILayout.Width(s_ContextWidth * 0.3F - 80F - 3F);
						int selectedCount = Target.iArguments.Contains(-1) ? states.Count : Target.iArguments.Count;
						if (selectedCount > 1) {
							while (Target.bArguments.Count < 4) {
								Target.bArguments.Add(false);
							}
							bool noRepeat = Target.bArguments[3];
							bool newNoRepeat = DrawToggle(noRepeat, "不重复", noRepeatBtnOption);
							if (newNoRepeat != noRepeat) {
								Property.RecordForUndo("BArguments");
								Target.bArguments[3] = newNoRepeat;
							}
						} else {
							CustomEditorGUI.BeginDisabled(true);
							DrawToggle(false, "不重复", noRepeatBtnOption);
							CustomEditorGUI.EndDisabled();
						}
					} else {
						EditorGUILayout.LabelField("", GUILayout.Width(s_ContextWidth * 0.3F - 80F - 3F));
					}
				}
				
				EditorGUILayout.EndHorizontal();
			}
		}
		
		private void DrawProgressController() {
			ProgressController newObj = DrawCompFieldWithThisBtn<ProgressController>("控制器", Target.obj);
			if (newObj != Target.obj) {
				Property.RecordForUndo("Obj");
				Target.obj = newObj;
			}
			if (newObj != null) {
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("赋值", CustomEditorGUI.LabelWidthOption);
				bool newIsRelative = DrawToggle(Target.bArguments[0], "相对偏移", BTN_WIDTH_OPTION);
				if (newIsRelative != Target.bArguments[0]) {
					Property.RecordForUndo("BArguments");
					Target.bArguments[0] = newIsRelative;
				}
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal();
				if (newIsRelative) {
					float newOffset = EditorGUILayout.FloatField("偏移", Target.fArguments[0]);
					if (!Mathf.Approximately(newOffset, Target.fArguments[0])) {
						Property.RecordForUndo("IArguments");
						Target.fArguments[0] = newOffset;
					}
					bool newLoop = DrawToggle(Target.bArguments[1], "循环", BTN_WIDTH_OPTION);
					if (newLoop != Target.bArguments[1]) {
						Property.RecordForUndo("BArguments");
						Target.bArguments[1] = newLoop;
					}
				} else {
					bool random = Target.bArguments[1];
					if (random) {
						EditorGUILayout.LabelField("进度", CustomEditorGUI.LabelWidthOption);
						float newMinValue = Target.fArguments[0];
						float newMaxValue = Target.fArguments[1];
						newMinValue = Mathf.Min(EditorGUILayout.FloatField(newMinValue, GUILayout.Width(60F)), newMaxValue);
						if (!Mathf.Approximately(newMinValue, Target.fArguments[0])) {
							Property.RecordForUndo("FArguments");
							Target.fArguments[0] = newMinValue;
						}
						EditorGUILayout.MinMaxSlider(ref newMinValue, ref newMaxValue, 0, 1);
						if (!Mathf.Approximately(newMinValue, Target.fArguments[0])) {
							Property.RecordForUndo("FArguments");
							Target.fArguments[0] = newMinValue;
						}
						if (!Mathf.Approximately(newMaxValue, Target.fArguments[1])) {
							Property.RecordForUndo("FArguments");
							Target.fArguments[1] = newMaxValue;
						}
						newMaxValue = Mathf.Max(EditorGUILayout.FloatField(newMaxValue, GUILayout.Width(60F)), newMinValue);
						if (!Mathf.Approximately(newMaxValue, Target.fArguments[1])) {
							Property.RecordForUndo("FArguments");
							Target.fArguments[1] = newMaxValue;
						}
					} else {
						float newProgress = EditorGUILayout.Slider("进度", Target.fArguments[0], 0, 1);
						if (!Mathf.Approximately(newProgress, Target.fArguments[0])) {
							Property.RecordForUndo("FArguments");
							Target.fArguments[0] = newProgress;
						}
					}
				
					bool newRandom = DrawToggle(random, "随机", BTN_WIDTH_OPTION);
					if (newRandom != random) {
						Property.RecordForUndo("BArguments");
						Target.bArguments[1] = newRandom;
					}
				}
				GUILayoutOption width = GUILayout.Width(s_ContextWidth * 0.3F - 80F - 3F);
				EditorGUILayout.LabelField("", width);
				EditorGUILayout.EndHorizontal();
				
				DrawTween();
			}
		}

		private void DrawCustomEvent() {
			EditorGUILayout.BeginHorizontal();
			string newEventName = EditorGUILayout.TextField("事件", Target.sArguments[0]);
			if (newEventName != Target.sArguments[0]) {
				Property.RecordForUndo("SArguments");
				Target.sArguments[0] = newEventName;
			}
			bool newBroadcast = DrawToggle(Target.bArguments[0], "广播", BTN_WIDTH_OPTION);
			if (newBroadcast != Target.bArguments[0]) {
				Property.RecordForUndo("BArguments");
				Target.bArguments[0] = newBroadcast;
			}
			GUILayoutOption width = GUILayout.Width(s_ContextWidth * 0.3F - 80 - 3F);
			EditorGUILayout.LabelField("", width);
			EditorGUILayout.EndHorizontal();
			if (!newBroadcast) {
				GameObject newTarget = DrawObjectFieldWithThisBtn<GameObject>("目标", Target.objArguments[0]);
				if (newTarget != Target.objArguments[0]) {
					Property.RecordForUndo("ObjArguments");
					Target.objArguments[0] = newTarget;
				}
			}
		}
		
		private static void IntPopupMultiSelect(int[] selectedUIDs, string[] displayedOptions, int[] optionUIDs, Action<int[]> onChange) {
			string label;
			int selectedLength = selectedUIDs.Length;
			if (selectedLength <= 0) {
				label = "Nothing";
			} else {
				int[] allUIDs = Array.FindAll(optionUIDs, uid => uid >= 0);
				HashSet<int> selectedUIDSet = new HashSet<int>(selectedUIDs);
				HashSet<int> allUIDSet = new HashSet<int>(allUIDs);
				bool selectionIsDirty = Array.Exists(selectedUIDs, uid => !allUIDSet.Contains(uid));
				bool everything = Array.TrueForAll(allUIDs, uid => selectedUIDSet.Contains(uid));
				if (everything) {
					label = selectionIsDirty ? "Everything,Dirty" : "Everything";
				} else {
					if (selectedUIDSet.Count == 1) {
						label = selectionIsDirty ? "Dirty" : displayedOptions[Array.IndexOf(optionUIDs, selectedUIDs[0])];
					} else {
						if (selectionIsDirty) {
							label = "Mixed,Dirty";
						} else {
							bool[] isSelects = Array.ConvertAll(allUIDs, uid => selectedUIDSet.Contains(uid));
							List<string> list = new List<string>();
							for (int i = 0, length = isSelects.Length; i < length; ++i) {
								if (isSelects[i]) {
									if (i < length - 2 && isSelects[i + 1] && isSelects[i + 2]) {
										int j = i + 2;
										for (int k = j + 1; k < length; ++k) {
											if (isSelects[k]) {
												j = k;
											} else {
												break;
											}
										}
										list.Add(i + "-" + j);
										i = j;
									} else {
										list.Add(i + "");
									}
								}
							}
							label = string.Join(",", list);
						}
					}
				}
			}
			CustomEditorGUI.IntPopupMultiSelect(onChange, label, selectedUIDs, displayedOptions, optionUIDs);
		}
	}
}
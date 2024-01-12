/*
 * @Author: wangyun
 * @CreateTime: 2022-03-30 16:21:41 627
 * @LastEditor: wangyun
 * @EditTime: 2023-02-11 05:30:03 155
 */

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;

using UObject = UnityEngine.Object;

namespace Control {
	[CanEditMultipleObjects]
	[CustomEditor(typeof(StateController), true)]
	public class StateControllerInspector : Editor {
		private static readonly GUILayoutOption BTN_WIDTH_OPTION = GUILayout.Width(80F);
		private static readonly GUILayoutOption BTN_DOUBLE_HEIGHT_OPTION = GUILayout.Height(EditorGUIUtility.singleLineHeight * 2);
		
		private static readonly GUIStyle STYLE = new GUIStyle();
		private const float BUTTON_WIDTH_MIN = 50F;
		private static readonly GUILayoutOption BUTTON_WIDTH_MIN_OPTION = GUILayout.MinWidth(BUTTON_WIDTH_MIN);

		protected StateController m_Target;
		protected StateController[] m_AllTargets;
		
		[SerializeField]
		private bool m_BriefMode = true;
		[SerializeField]
		private bool m_DisableCtrl;
		[SerializeField]
		private bool m_AutoCapture;
		
		private bool m_IsCheckedPrefabComparing;
		private bool m_IsPrefabComparing;

		private StateRelateState m_StateRelationToPaste;
		private StateRelateProgress m_ProgressRelationToPaste;
		private StateRelateExecutor m_ExecutorRelationToPaste;

		private PropertyModification[] m_Modifications = Array.Empty<PropertyModification>();

		private void OnEnable() {
			m_Target = target as StateController;
			m_AllTargets = Array.ConvertAll(targets, t => t as StateController);
			Texture icon = EditorGUIUtility.IconContent("Button Icon").image;
			MethodInfo setIconForObjectMI = typeof(EditorGUIUtility).GetMethod("SetIconForObject", BindingFlags.Static | BindingFlags.NonPublic);
			setIconForObjectMI?.Invoke(null, new object[] { m_Target, icon });
		}

		public override void OnInspectorGUI() {
			if (targets.Length > 1) {
				DrawTitleMixed();
				DrawOtherFieldsMixed();
				EditorGUILayout.Space(5F, false);
				DrawStatesMixed();
			} else {
				if (!m_IsCheckedPrefabComparing) {
					m_IsCheckedPrefabComparing = true;
					m_IsPrefabComparing = CustomEditorGUI.IsPrefabComparing();
					m_BriefMode = !m_IsPrefabComparing;
				}
				m_Modifications = PrefabUtility.GetPropertyModifications(m_Target) ?? m_Modifications;
				DrawTitle();
				if (m_BriefMode) {
					EditorGUILayout.Space(5F, false);
					DrawStatesBrief();
					DrawRelationsBrief();
					EditorGUILayout.Space(5F, false);
				} else {
					DrawOtherFields();
					EditorGUILayout.Space(5F, false);
					DrawStates();
					EditorGUILayout.Space(5F, false);
					DrawStateRelations(m_Target.relations);
					EditorGUILayout.Space(2F, false);
					DrawProgressRelations(m_Target.progressRelations);
					EditorGUILayout.Space(2F, false);
					DrawExecutorRelations(m_Target.executorRelations);
					EditorGUILayout.Space(5F, false);
					DrawTargets();
					EditorGUILayout.Space(5F, false);
				}
				DrawCapture();
			}
			if (GUI.changed) {
				EditorUtility.SetDirty(m_Target);
			}
		}
		
		private void DrawTitleMixed() {
			// 标题
			EditorGUI.showMixedValue = Array.Exists(m_AllTargets, t => t.title != m_Target.title);
			EditorGUI.BeginChangeCheck();
			CustomEditorGUI.BeginLabelWidth(30F);
			string newTitle = EditorGUILayout.TextField("标题:", m_Target.title);
			CustomEditorGUI.EndLabelWidth();
			if (EditorGUI.EndChangeCheck()) {
				Array.ForEach(m_AllTargets, t => {
					Undo.RecordObject(t, "Title");
					t.title = newTitle;
				});
			}
			EditorGUI.showMixedValue = false;

			// 初始索引
			EditorGUI.showMixedValue = Array.Exists(m_AllTargets, t => t.initialIndex != m_Target.initialIndex);
			EditorGUI.BeginChangeCheck();
			CustomEditorGUI.BeginLabelWidth(54F);
			string[] options = GetStatePopupOption(m_Target.states);
			int newInitialIndex = EditorGUILayout.Popup("初始状态:", m_Target.initialIndex, options);
			CustomEditorGUI.EndLabelWidth();
			if (EditorGUI.EndChangeCheck()) {
				Array.ForEach(m_AllTargets, t => {
					Undo.RecordObject(t, "InitialIndex");
					t.initialIndex = newInitialIndex;
				});
			}
			EditorGUI.showMixedValue = false;
		}
		
		private void DrawOtherFieldsMixed() {
			EditorGUILayout.BeginHorizontal();
			// 延迟初始化按钮
			bool lazyInit = m_Target.lazyInit;
			bool lazyInitMixed = Array.Exists(m_AllTargets, t => t.lazyInit != lazyInit);
			EditorGUI.BeginChangeCheck();
			bool newLazyInit = CustomEditorGUI.Toggle(
					lazyInit && !lazyInitMixed,
					lazyInitMixed ? "延迟初始化(Mixed)" : "延迟初始化",
					lazyInitMixed ? (CustomEditorGUI.COLOR_TOGGLE_CHECKED + CustomEditorGUI.COLOR_NORMAL) * 0.5F : CustomEditorGUI.COLOR_NORMAL,
					CustomEditorGUI.COLOR_TOGGLE_CHECKED
			);
			if (EditorGUI.EndChangeCheck()) {
				Array.ForEach(m_AllTargets, t => {
					Undo.RecordObject(t, "LazyInit");
					t.lazyInit = newLazyInit;
				});
			}

			// 禁用缓动按钮
			bool invalidateTween = m_Target.invalidateTween;
			bool invalidateTweenMixed = Array.Exists(m_AllTargets, t => t.invalidateTween != invalidateTween);
			EditorGUI.BeginChangeCheck();
			bool newInvalidateTween = CustomEditorGUI.Toggle(
					invalidateTween && !invalidateTweenMixed,
					invalidateTweenMixed ? "禁用对象缓动(Mixed)" : "禁用对象缓动",
					invalidateTweenMixed ? (CustomEditorGUI.COLOR_TOGGLE_CHECKED + CustomEditorGUI.COLOR_NORMAL) * 0.5F : CustomEditorGUI.COLOR_NORMAL,
					CustomEditorGUI.COLOR_TOGGLE_CHECKED,
					GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.52F - 12F)
			);
			if (EditorGUI.EndChangeCheck()) {
				Array.ForEach(m_AllTargets, t => {
					Undo.RecordObject(t, "InvalidateTween");
					t.invalidateTween = newInvalidateTween;
				});
			}
			EditorGUILayout.EndHorizontal();
		}
		
		private void DrawStatesMixed() {
			// 如果状态数量不一致，则不显示按钮
			int stateCount = m_Target.StateCount;
			if (Array.Exists(m_AllTargets, t => t.StateCount != stateCount)) {
				return;
			}
			
			// 如果选中状态不一致，则全不选中
			int stateIndex = m_Target.Index;
			if (Array.Exists(m_AllTargets, t => t.Index != stateIndex)) {
				stateIndex = -1;
			}
			// 所有状态文本不一致的按钮显示为横线
			string[] options = GetStatePopupOption(m_Target.states);
			foreach (var t in m_AllTargets) {
				string[] _options = GetStatePopupOption(t.states);
				for (int i = 0; i < stateCount; ++i) {
					if (_options[i] != options[i]) {
						options[i] = i + ": — ";
					}
				}
			}
			// 计算状态按钮列数和行数
			float contextWidth = CustomEditorGUI.GetContextWidth() - 20 - 5 - 10;	// 前面空20，后面空5，滚动条10
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
						float itemWidth = Mathf.Max(STYLE.CalcSize(new GUIContent(options[i + j])).x + 15, BUTTON_WIDTH_MIN);
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
			// 绘制按钮
			for (int r = 0; r < row; ++r) {
				EditorGUILayout.BeginHorizontal();
				for (int c = 0; c < col; ++c) {
					int i = r * col + c;
					if (i < stateCount) {
						bool selected = i == stateIndex;
						// 序号按钮
						if (CustomEditorGUI.Toggle(selected, options[i], CustomEditorGUI.COLOR_TOGGLE_CHECKED, BUTTON_WIDTH_MIN_OPTION) && !selected) {
							Array.ForEach(m_AllTargets, t => {
								Undo.RegisterFullObjectHierarchyUndo(t, "State.Select");
								t.Index = i;
							});
						}
					}
				}
				EditorGUILayout.EndHorizontal();
			}
		}
		
		private void DrawTitle() {
			EditorGUILayout.BeginHorizontal();
				EditorGUILayout.BeginVertical();
					// 标题
					CheckModification("title", -18F, 30F, () => {
						CustomEditorGUI.BeginLabelWidth(30F);
						string newTitle = EditorGUILayout.TextField("标题:", m_Target.title);
						if (newTitle != m_Target.title) {
							Undo.RecordObject(m_Target, "Title");
							m_Target.title = newTitle;
						}
						CustomEditorGUI.EndLabelWidth();
					});
					
					// 初始索引
					CheckModification("initialIndex", -18F, 54F, () => {
						CustomEditorGUI.BeginLabelWidth(54F);
						string[] options = GetStatePopupOption(m_Target.states);
						int newInitialIndex = EditorGUILayout.Popup("初始状态:", m_Target.initialIndex, options);
						if (newInitialIndex != m_Target.initialIndex) {
							Undo.RecordObject(m_Target, "InitialIndex");
							m_Target.initialIndex = newInitialIndex;
						}
						CustomEditorGUI.EndLabelWidth();
					});
				EditorGUILayout.EndVertical();

				EditorGUILayout.BeginVertical(BTN_WIDTH_OPTION);
					// 简略模式按钮
					if (CustomEditorGUI.Toggle(m_BriefMode, "简略模式", CustomEditorGUI.COLOR_TOGGLE_CHECKED_EDITOR) != m_BriefMode) {
						Undo.RecordObject(this, "BriefMode");
						m_BriefMode = !m_BriefMode;
					}
					// 屏蔽控制按钮
					if (CustomEditorGUI.Toggle(m_DisableCtrl, "屏蔽控制", CustomEditorGUI.COLOR_TOGGLE_CHECKED_EDITOR) != m_DisableCtrl) {
						Undo.RecordObject(this, "DisableCtrl");
						m_DisableCtrl = !m_DisableCtrl;
					}
				EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
		}

		private void DrawStatesBrief() {
			float contextWidth = CustomEditorGUI.GetContextWidth() - 20 - 5 - 10;	// 前面空20，后面空5，滚动条10
			string[] options = GetStatePopupOption(m_Target.states);
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
						float itemWidth = Mathf.Max(STYLE.CalcSize(new GUIContent(options[i + j])).x + 15, BUTTON_WIDTH_MIN);
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

			Rect rect = EditorGUILayout.BeginVertical();
			for (int r = 0; r < row; ++r) {
				EditorGUILayout.BeginHorizontal();
				for (int c = 0; c < col; ++c) {
					int i = r * col + c;
					if (i < stateCount) {
						bool isModified = IsModified($"^states\\.Array\\.data\\[{i}\\]", true);
						if (isModified) {
							CustomEditorGUI.BeginBold(true);
						}
						bool selected = i == m_Target.Index;
						// 序号按钮
						if (CustomEditorGUI.Toggle(selected, options[i], CustomEditorGUI.COLOR_TOGGLE_CHECKED, BUTTON_WIDTH_MIN_OPTION) && !selected) {
							OnStateBtnClick(i);
						}
						if (isModified) {
							CustomEditorGUI.EndBold();
						}
					}
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();
			if (IsModified("m_Index")) {
				DrawModification(new Rect(rect.x - 18F, rect.y, 18F, rect.height), "m_Index");
			}
		}

		private void DrawRelationsBrief() {
			StringBuilder sb = new StringBuilder();
			int stateRelationCount = m_Target.relations.Count;
			if (stateRelationCount > 0) {
				sb.Append("关联状态控制:");
				sb.Append(stateRelationCount);
				sb.Append("\t");
			}
			int progressRelationCount = m_Target.progressRelations.Count;
			if (progressRelationCount > 0) {
				sb.Append("关联进度控制:");
				sb.Append(progressRelationCount);
				sb.Append("\t");
			}
			int executorRelationCount = m_Target.executorRelations.Count;
			if (executorRelationCount > 0) {
				sb.Append("关联执行器:");
				sb.Append(executorRelationCount);
				sb.Append("\t");
			}
			if (sb.Length > 0) {
				EditorGUILayout.LabelField(sb.ToString());
			}
		}
		
		private void DrawOtherFields() {
			EditorGUILayout.BeginHorizontal();
			{
				// 延迟初始化按钮
				if (CustomEditorGUI.Toggle(m_Target.lazyInit, "延迟初始化", CustomEditorGUI.COLOR_TOGGLE_CHECKED) != m_Target.lazyInit) {
					Undo.RecordObject(m_Target, "lazyInit");
					m_Target.lazyInit = !m_Target.lazyInit;
				}

				// 禁用缓动按钮
				if (CustomEditorGUI.Toggle(m_Target.invalidateTween, "禁用对象缓动", CustomEditorGUI.COLOR_TOGGLE_CHECKED) != m_Target.invalidateTween) {
					Undo.RecordObject(m_Target, "invalidateTween");
					m_Target.invalidateTween = !m_Target.invalidateTween;
				}
			}
			EditorGUILayout.EndHorizontal();
		}

		private void DrawStates() {
			bool fold = EditorPrefs.GetBool("DrawStates", false);
			bool newFold = GUILayout.Toggle(fold, fold ? "\u25BA 状态列表:" : "\u25BC 状态列表:", "BoldLabel");
			if (newFold != fold) {
				EditorPrefs.SetBool("DrawStates", newFold);
			}

			if (newFold) {
				return;
			}

			float contextWidth = CustomEditorGUI.GetContextWidth();
			float stateNameWidth = contextWidth * 0.4F - 40F;
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space(10F, false);
			EditorGUILayout.LabelField("序号:", GUILayout.Width(30F));
			EditorGUILayout.LabelField("名称:", GUILayout.Width(stateNameWidth));
			EditorGUILayout.LabelField("描述:");
			EditorGUILayout.EndHorizontal();

			List<State> states = m_Target.states;
			for (int i = 0, length = states.Count; i < length; ++i) {
				bool selected = i == m_Target.Index;
				State state = states[i];

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space(10F, false);

				// 序号按钮
				if (CustomEditorGUI.Toggle(selected, i + string.Empty, CustomEditorGUI.COLOR_TOGGLE_CHECKED, GUILayout.Width(30F)) && !selected) {
					OnStateBtnClick(i);
				}

				// 名称
				string newName = EditorGUILayout.TextField(state.name, GUILayout.Width(stateNameWidth));
				if (newName != state.name) {
					Undo.RecordObject(m_Target, "State.Name");
					state.name = newName;
				}

				// 描述
				string newDesc = EditorGUILayout.TextField(state.desc);
				if (newDesc != state.desc) {
					Undo.RecordObject(m_Target, "State.Desc");
					state.desc = newDesc;
				}

				bool prevEnabled = GUI.enabled;
				// 上移下移删除按钮
				GUI.enabled = prevEnabled && i > 0;
				bool up = GUILayout.Button("\u25B2", GUILayout.Width(23F));
				GUI.enabled = prevEnabled && i < length - 1;
				bool down = GUILayout.Button("\u25BC", GUILayout.Width(23F));
				GUI.enabled = prevEnabled && length > 1;
				bool remove = GUILayout.Button("X", GUILayout.Width(24F));
				GUI.enabled = prevEnabled;

				// 上移下移删除
				if (remove) {
					Undo.RecordObject(m_Target, "State.Remove");
					m_Target.RemoveState(i);
					--length;
					--i;
				} else if (up) {
					Undo.RecordObject(m_Target, "State.Up");
					states[i] = states[i - 1];
					states[i - 1] = state;
				} else if (down) {
					Undo.RecordObject(m_Target, "State.Down");
					states[i] = states[i + 1];
					states[i + 1] = state;
				}

				EditorGUILayout.EndHorizontal();
			}
			{
				// 新增按钮
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space(10F, false);
				if (GUILayout.Button("+", GUILayout.Width(30F))) {
					Undo.RecordObject(m_Target, "State.Add");
					m_Target.AddState();
				}
				EditorGUILayout.Space(5F, false);
				if (GUILayout.Button("+5", GUILayout.Width(40F))) {
					Undo.RecordObject(m_Target, "State.Add");
					for (int i = 0; i < 5; ++i) {
						m_Target.AddState();
					}
				}
				EditorGUILayout.Space(5F, false);
				if (GUILayout.Button("+10", GUILayout.Width(40F))) {
					Undo.RecordObject(m_Target, "State.Add");
					for (int i = 0; i < 10; ++i) {
						m_Target.AddState();
					}
				}
				EditorGUILayout.EndHorizontal();
			}
		}

		private void DrawStateRelations(IList<StateRelateState> relations) {
			bool fold = EditorPrefs.GetBool("DrawStateRelations", false);
			bool newFold = GUILayout.Toggle(fold, fold ? "\u25BA 关联状态控制:" : "\u25BC 关联状态控制:", "BoldLabel");
			if (newFold != fold) {
				EditorPrefs.SetBool("DrawStateRelations", newFold);
			}
			if (!newFold) {
				if (relations.Count > 0) {
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.Space(10F, false);
					DrawRelations(relations);
					EditorGUILayout.EndHorizontal();
				}
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space(10F, false);
				if (GUILayout.Button("+", GUILayout.Width(30F))) {
					Undo.RecordObject(m_Target, "StateRelation.Add");
					relations.Add(new StateRelateState());
				}
				EditorGUILayout.Space(5F, false);
				CustomEditorGUI.BeginDisabled(m_StateRelationToPaste == null && relations.Count <= 0);
				if (GUILayout.Button("粘贴", GUILayout.Width(40F))) {
					StateRelateState basedRelation = m_StateRelationToPaste ?? relations[relations.Count - 1];
					StateRelateState relation = new StateRelateState {
						fromUIDs = new List<int>(basedRelation.fromUIDs),
						toUIDs = new List<int>(basedRelation.toUIDs),
						delay = basedRelation.delay,
						single = basedRelation.single,
						controller = basedRelation.controller,
						targetUID = basedRelation.targetUID
					};
					Undo.RecordObject(m_Target, "StateRelation.Add");
					relations.Add(relation);
				}
				CustomEditorGUI.EndDisabled();
				EditorGUILayout.EndHorizontal();
			}
		}

		private void DrawProgressRelations(IList<StateRelateProgress> relations) {
			bool fold = EditorPrefs.GetBool("DrawProgressRelations", false);
			bool newFold = GUILayout.Toggle(fold, fold ? "\u25BA 关联进度控制:" : "\u25BC 关联进度控制:", "BoldLabel");
			if (newFold != fold) {
				EditorPrefs.SetBool("DrawProgressRelations", newFold);
			}
			if (!newFold) {
				if (relations.Count > 0) {
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.Space(10F, false);
					DrawRelations(relations);
					EditorGUILayout.EndHorizontal();
				}
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space(10F, false);
				if (GUILayout.Button("+", GUILayout.Width(30F))) {
					Undo.RecordObject(m_Target, "ProgressRelation.Add");
					relations.Add(new StateRelateProgress());
				}
				EditorGUILayout.Space(5F, false);
				CustomEditorGUI.BeginDisabled(m_ProgressRelationToPaste == null && relations.Count <= 0);
				if (GUILayout.Button("粘贴", GUILayout.Width(40F))) {
					StateRelateProgress basedRelation = m_ProgressRelationToPaste ?? relations[relations.Count - 1];
					StateRelateProgress relation = new StateRelateProgress {
						fromUIDs = new List<int>(basedRelation.fromUIDs),
						toUIDs = new List<int>(basedRelation.toUIDs),
						delay = basedRelation.delay,
						single = basedRelation.single,
						controller = basedRelation.controller,
						targetProgress = basedRelation.targetProgress
					};
					Undo.RecordObject(m_Target, "ProgressRelation.Add");
					relations.Add(relation);
				}
				CustomEditorGUI.EndDisabled();
				EditorGUILayout.EndHorizontal();
			}
		}

		private void DrawExecutorRelations(IList<StateRelateExecutor> relations) {
			bool fold = EditorPrefs.GetBool("DrawExecutorRelations", false);
			bool newFold = GUILayout.Toggle(fold, fold ? "\u25BA 关联执行器:" : "\u25BC 关联执行器:", "BoldLabel");
			if (newFold != fold) {
				EditorPrefs.SetBool("DrawExecutorRelations", newFold);
			}
			if (!newFold) {
				if (relations.Count > 0) {
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.Space(10F, false);
					DrawRelations(relations);
					EditorGUILayout.EndHorizontal();
				}
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space(10F, false);
				if (GUILayout.Button("+", GUILayout.Width(30F))) {
					Undo.RecordObject(m_Target, "ExecutorRelation.Add");
					relations.Add(new StateRelateExecutor());
				}
				EditorGUILayout.Space(5F, false);
				CustomEditorGUI.BeginDisabled(m_ExecutorRelationToPaste == null && relations.Count <= 0);
				if (GUILayout.Button("粘贴", GUILayout.Width(40F))) {
					StateRelateExecutor basedRelation = m_ExecutorRelationToPaste ?? relations[relations.Count - 1];
					StateRelateExecutor relation = new StateRelateExecutor {
						fromUIDs = new List<int>(basedRelation.fromUIDs),
						toUIDs = new List<int>(basedRelation.toUIDs),
						delay = basedRelation.delay,
						single = basedRelation.single,
						executor = basedRelation.executor
					};
					Undo.RecordObject(m_Target, "ExecutorRelation.Add");
					relations.Add(relation);
				}
				CustomEditorGUI.EndDisabled();
				EditorGUILayout.EndHorizontal();
			}
		}

		private void DrawRelations<T>(IList<T> relations) where T : StateRelate, new() {
			EditorGUILayout.BeginVertical();
			
			string[] options = GetStatePopupOption(m_Target.states);
			int[] uids = m_Target.states.ConvertAll(state => state.uid).ToArray();
			for (int i = 0, length = relations.Count; i < length; ++i) {
				T relation = relations[i];
			
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("从", GUILayout.Width(13F));
				int[] selectedFromUIDs = relation.fromUIDs.Contains(-1) ? uids : relation.fromUIDs.ToArray();
				IntPopupMultiSelect(selectedFromUIDs, options, uids, newSelectedUIDs => {
					Undo.RecordObject(m_Target, "Relation.From");
					relation.fromUIDs.Clear();
					if (newSelectedUIDs.Length == uids.Length) {
						relation.fromUIDs.Add(-1);
					} else {
						relation.fromUIDs.AddRange(newSelectedUIDs);
					}
				});
				EditorGUILayout.LabelField("到", GUILayout.Width(13F));
				int[] selectedToUIDs = relation.toUIDs.Contains(-1) ? uids : relation.toUIDs.ToArray();
				IntPopupMultiSelect(selectedToUIDs, options, uids, newSelectedUIDs => {
					Undo.RecordObject(m_Target, "Relation.To");
					relation.toUIDs.Clear();
					if (newSelectedUIDs.Length == uids.Length) {
						relation.toUIDs.Add(-1);
					} else {
						relation.toUIDs.AddRange(newSelectedUIDs);
					}
				});
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal();
				switch (relation) {
					case StateRelateState stateRelation: {
						// 关联状态
						EditorGUILayout.LabelField("切换状态", GUILayout.Width(49F));
						stateRelation.controller = DrawComponentSelector(
								stateRelation.controller,
								controller => string.IsNullOrEmpty(controller.title) ? "匿名" : controller.title,
								() => Undo.RecordObject(m_Target, "StateRelation.Controller")
						);
						if (stateRelation.controller) {
							List<State> targetStates = stateRelation.controller.states;
							List<int> targetUIDs = targetStates.ConvertAll(state => state.uid);
							List<string> targetOptions = new List<string>(GetStatePopupOption(targetStates));
							targetUIDs.InsertRange(0, new [] {
								StateRelateState.TARGET_NONE,
								StateRelateState.TARGET_SAME_INDEX,
								StateRelateState.TARGET_SAME_NAME
							});
							targetOptions.InsertRange(0, new [] {
								"无",
								"相同序号",
								"相同名称"
							});
							int newTargetUID = EditorGUILayout.IntPopup(stateRelation.targetUID, targetOptions.ToArray(), targetUIDs.ToArray());
							if (newTargetUID != stateRelation.targetUID) {
								Undo.RecordObject(m_Target, "StateRelation.Target");
								stateRelation.targetUID = newTargetUID;
							}
						} else {
							CustomEditorGUI.BeginDisabled(true);
							EditorGUILayout.Popup(0, new [] { "无" });
							CustomEditorGUI.EndDisabled();
						}
						break;
					}
					case StateRelateProgress progressRelation: {
						// 关联进度
						EditorGUILayout.LabelField("改变进度", GUILayout.Width(49F));
						progressRelation.controller = DrawComponentSelector(
								progressRelation.controller,
								controller => string.IsNullOrEmpty(controller.title) ? "匿名" : controller.title,
								() => Undo.RecordObject(m_Target, "ProgressRelation.Controller")
						);
						if (progressRelation.controller) {
							float newProgress = EditorGUILayout.Slider(progressRelation.targetProgress, 0, 1);
							if (Mathf.Abs(newProgress - progressRelation.targetProgress) >= Mathf.Epsilon) {
								Undo.RecordObject(m_Target, "ProgressRelation.Progress");
								progressRelation.targetProgress = newProgress;
							}
						} else {
							CustomEditorGUI.BeginDisabled(true);
							EditorGUILayout.Slider(0, 0, 1);
							CustomEditorGUI.EndDisabled();
						}
						break;
					}
					case StateRelateExecutor executorRelation: {
						// 执行
						EditorGUILayout.LabelField("执行", GUILayout.Width(25F));
						executorRelation.executor = DrawComponentSelector(
								executorRelation.executor,
								executor => executor.GetType().Name + " - " + (string.IsNullOrEmpty(executor.title) ? "匿名" : executor.title),
								() => Undo.RecordObject(m_Target, "ExecutorRelation.Execute")
						);
						break;
					}
				}
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal();
				CustomEditorGUI.BeginLabelWidth(26F);
				float newDelay = EditorGUILayout.FloatField("延迟", relation.delay, GUILayout.MinWidth(70F));
				if (Mathf.Abs(newDelay - relation.delay) > Mathf.Epsilon) {
					Undo.RecordObject(m_Target, "Relation.Delay");
					relation.delay = newDelay;
				}
				CustomEditorGUI.EndLabelWidth();

				GUIContent content = new GUIContent("允许掐掉", "选中后，如果两次执行间隔小于延迟，则上一次延迟会被掐掉");
				bool newSingle = CustomEditorGUI.Toggle(relation.single, content, CustomEditorGUI.COLOR_TOGGLE_CHECKED, GUILayout.Width(58F));
				if (newSingle != relation.single) {
					Undo.RecordObject(m_Target, "Relation.Single");
					relation.single = newSingle;
				}
				
				GUILayout.FlexibleSpace();
				
				switch (relation) {
					case StateRelateState stateRelation: {
						bool isSelect = m_StateRelationToPaste == null ? i == length - 1 : m_StateRelationToPaste == stateRelation;
						if (CustomEditorGUI.Toggle(isSelect, "复制", CustomEditorGUI.COLOR_TOGGLE_CHECKED_EDITOR, GUILayout.Width(34F)) && !isSelect) {
							m_StateRelationToPaste = stateRelation;
						}
						break;
					}
					case StateRelateProgress progressRelation: {
						bool isSelect = m_ProgressRelationToPaste == null ? i == length - 1 : m_ProgressRelationToPaste == progressRelation;
						if (CustomEditorGUI.Toggle(isSelect, "复制", CustomEditorGUI.COLOR_TOGGLE_CHECKED_EDITOR, GUILayout.Width(34F)) && !isSelect) {
							m_ProgressRelationToPaste = progressRelation;
						}
						break;
					}
					case StateRelateExecutor executorRelation: {
						bool isSelect = m_ExecutorRelationToPaste == null ? i == length - 1 : m_ExecutorRelationToPaste == executorRelation;
						if (CustomEditorGUI.Toggle(isSelect, "复制", CustomEditorGUI.COLOR_TOGGLE_CHECKED_EDITOR, GUILayout.Width(34F)) && !isSelect) {
							m_ExecutorRelationToPaste = executorRelation;
						}
						break;
					}
				}
				
				// 上移下移删除按钮
				CustomEditorGUI.BeginDisabled(i <= 0);
				if (GUILayout.Button("\u25B2", GUILayout.Width(23F))) {
					Undo.RecordObject(m_Target, "Relation.Up");
					relations[i] = relations[i - 1];
					relations[i - 1] = relation;
				}
				CustomEditorGUI.ChangeDisabled(i >= length - 1);
				if (GUILayout.Button("\u25BC", GUILayout.Width(23F))) {
					Undo.RecordObject(m_Target, "Relation.Down");
					relations[i] = relations[i + 1];
					relations[i + 1] = relation;
				}
				CustomEditorGUI.EndDisabled();
				if (GUILayout.Button("X", GUILayout.Width(24F))) {
					Undo.RecordObject(m_Target, "Relation.Remove");
					relations.RemoveAt(i);
					--length;
					--i;
					if (m_StateRelationToPaste == relation) {
						m_StateRelationToPaste = null;
					} else if (m_ProgressRelationToPaste == relation) {
						m_ProgressRelationToPaste = null;
					} else if (m_ExecutorRelationToPaste == relation) {
						m_ExecutorRelationToPaste = null;
					}
				}
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.Space(2F, false);
			}
			
			EditorGUILayout.EndVertical();
		}

		private void DrawTargets() {
			bool fold = EditorPrefs.GetBool("DrawTargets", false);
			bool newFold = GUILayout.Toggle(fold, fold ? "\u25BA 控制对象:" : "\u25BC 控制对象:", "BoldLabel");
			if (newFold != fold) {
				EditorPrefs.SetBool("DrawTargets", newFold);
			}

			if (newFold) {
				return;
			}

			bool prevEnabled = GUI.enabled;
			GUI.enabled = false;
			BaseStateCtrl[] _targets = m_Target.Targets;
			foreach (var _target in _targets) {
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space(10F, false);

				EditorGUILayout.ObjectField(_target, typeof(BaseStateCtrl), true);

				EditorGUILayout.Space(10F, false);
				EditorGUILayout.EndHorizontal();
			}
			GUI.enabled = prevEnabled;
		}

		private void DrawCapture() {
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("记录状态", BTN_DOUBLE_HEIGHT_OPTION)) {
				Undo.RegisterFullObjectHierarchyUndo(m_Target, "StateCapture");
				m_Target.Capture();
			}
			if (CustomEditorGUI.Toggle(m_AutoCapture, "切换状态时\n自动记录", CustomEditorGUI.COLOR_TOGGLE_CHECKED_EDITOR, BTN_WIDTH_OPTION, BTN_DOUBLE_HEIGHT_OPTION) != m_AutoCapture) {
				m_AutoCapture = !m_AutoCapture;
			}
			EditorGUILayout.EndHorizontal();
		}

		private static string[] GetStatePopupOption(List<State> states) {
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

		private void OnStateBtnClick(int i) {
			if (m_AutoCapture) {
				Undo.RegisterFullObjectHierarchyUndo(m_Target, "Capture");
				m_Target.Capture();
				Undo.FlushUndoRecordObjects();
			}
			Undo.RegisterFullObjectHierarchyUndo(m_Target, "State.Select");
			if (m_DisableCtrl) {
				FieldInfo indexFI = m_Target.GetType().GetField("m_Index", BindingFlags.Instance | BindingFlags.NonPublic);
				FieldInfo prevIndexFI = m_Target.GetType().GetField("m_PrevIndex", BindingFlags.Instance | BindingFlags.NonPublic);
				if (indexFI?.GetValue(m_Target) is int index) {
					prevIndexFI?.SetValue(m_Target, index);
				}
				indexFI?.SetValue(m_Target, i);
			} else {
				m_Target.Index = i;
			}
		}

		private void CheckModification(string propertyPath, float offsetX, float originWidth, Action drawAction) {
			bool isModified = IsModified(propertyPath);
			if (isModified) {
				CustomEditorGUI.BeginBold(true);
			}
			drawAction?.Invoke();
			if (isModified) {
				CustomEditorGUI.EndBold();
				Rect rect = CustomEditorGUI.GetLastRect();
				DrawModification(new Rect(rect.x + offsetX, rect.y, originWidth - offsetX, rect.height), propertyPath);
			}
		}

		private void DrawModification(Rect rect, string propertyPath) {
			Rect wireRect = new Rect(rect.x, rect.y, 2, rect.height);
			EditorGUI.DrawRect(wireRect, CustomEditorGUI.COLOR_MODIFIED);
			if (Event.current.type == EventType.MouseUp && Event.current.button == 1) {
				Vector2 mousePosition = Event.current.mousePosition;
				if (rect.Contains(mousePosition)) {
					GenericMenu genericMenu = new GenericMenu();
					List<Component> sources = new List<Component>();
					for (Component temp = m_Target; temp != null; temp = PrefabUtility.GetCorrespondingObjectFromSource(temp)) {
						sources.Add(temp);
					}
					for (int i = 1, length = sources.Count; i < length; ++i) {
						GameObject prefab = sources[i].transform.root.gameObject;
						string text = i == length - 1 ? $"Apply to Prefab '{prefab.name}'" : $"Apply as Override in Prefab '{prefab.name}'";
						genericMenu.AddItem(new GUIContent(text), false, () => {
							PrefabUtility.ApplyPropertyOverride(serializedObject.FindProperty(propertyPath), AssetDatabase.GetAssetPath(prefab), InteractionMode.UserAction);
						});
					}
					genericMenu.AddItem(new GUIContent("Revert"), false, () => {
						PrefabUtility.RevertPropertyOverride(serializedObject.FindProperty(propertyPath), InteractionMode.UserAction);
					});
					genericMenu.DropDown(new Rect(mousePosition, Vector2.zero));
				}
			}
		}

		private bool IsModified(string propertyPath, bool isRegex = false) {
			UObject revertTarget = PrefabUtility.GetCorrespondingObjectFromSource(m_Target);
			if (revertTarget) {
				foreach (var modification in m_Modifications) {
					if (modification.target == revertTarget) {
						if (isRegex) {
							if (Regex.IsMatch(modification.propertyPath, propertyPath)) {
								return true;
							}
						} else {
							if (modification.propertyPath == propertyPath) {
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		public static T DrawComponentSelector<T>(T t, Converter<T, string> nameConvert = null, Action beforeChange = null) where T : Component {
			T newT = EditorGUILayout.ObjectField(t, typeof(T), true) as T;
			if (newT != t) {
				beforeChange?.Invoke();
				t = newT;
			}
			if (t != null) {
				List<T> ts = new List<T>(t.GetComponents<T>());
				List<string> names = ts.ConvertAll(nameConvert ?? (_t => _t.GetType().Name));
				ts.Insert(0, null);
				names.Insert(0, "无");
				for (int i = 0, length = names.Count; i < length; ++i) {
					names[i] = i + ":" + names[i];
				}
				int index = ts.IndexOf(t);
				int newIndex = EditorGUILayout.Popup(index, names.ToArray());
				if (newIndex != index) {
					beforeChange?.Invoke();
					t = ts[newIndex];
				}
			}
			return t;
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
/*
 * @Author: wangyun
 * @CreateTime: 2022-04-19 01:21:57 858
 * @LastEditor: wangyun
 * @EditTime: 2023-02-11 07:18:48 942
 */

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using DG.Tweening;
using UObject = UnityEngine.Object;

namespace Control {
	[CanEditMultipleObjects]
	[CustomEditor(typeof(ProgressController), true)]
	public class ProgressControllerInspector : Editor {
		private static readonly Color MODIFIED_COLOR = new Color(0.06F, 0.5F, 0.75F);
		private static readonly GUILayoutOption BTN_WIDTH_OPTION = GUILayout.Width(80F);
		private static readonly GUILayoutOption BTN_DOUBLE_HEIGHT_OPTION = GUILayout.Height(EditorGUIUtility.singleLineHeight * 2);

		protected ProgressController m_Target;
		protected ProgressController[] m_AllTargets;
		
		[SerializeField]
		private bool m_BriefMode = true;
		[SerializeField]
		private bool m_DisableCtrl;
		
		private bool m_IsCheckedPrefabComparing;
		private bool m_IsPrefabComparing;

		private ProgressRelateProgress m_ProgressRelationToPaste;
		private ProgressRelateState m_StateRelationToPaste;
		private ProgressRelateExecutor m_ExecutorRelationToPaste;

		private PropertyModification[] m_Modifications = Array.Empty<PropertyModification>();

		private void OnEnable() {
			m_Target = target as ProgressController;
			m_AllTargets = Array.ConvertAll(targets, t => t as ProgressController);
			Texture icon = EditorGUIUtility.IconContent("Slider Icon").image;
			MethodInfo setIconForObjectMI = typeof(EditorGUIUtility).GetMethod("SetIconForObject", BindingFlags.Static | BindingFlags.NonPublic);
			setIconForObjectMI?.Invoke(null, new object[] { m_Target, icon });
		}

		public override void OnInspectorGUI() {
			if (targets.Length > 1) {
				DrawTitleMixed();
				DrawOtherFieldsMixed();
				EditorGUILayout.Space(5F, false);
				DrawProgressMixed();
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
					DrawProgress();
					DrawRelationsBrief();
					EditorGUILayout.Space(5F, false);
				} else {
					DrawOtherFields();
					EditorGUILayout.Space(5F, false);
					DrawProgress();
					EditorGUILayout.Space(5F, false);
					DrawProgressRelations(m_Target.relations);
					EditorGUILayout.Space(2F, false);
					DrawStateRelations(m_Target.stateRelations);
					EditorGUILayout.Space(2F, false);
					DrawExecutorRelations(m_Target.executorRelations);
					EditorGUILayout.Space(5F, false);
					DrawTargets();
					EditorGUILayout.Space(5F, false);
				}
				EditorGUILayout.Space(5F, false);
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
			EditorGUI.showMixedValue = Array.Exists(m_AllTargets, t => !Mathf.Approximately(t.initialProgress, m_Target.initialProgress));
			EditorGUI.BeginChangeCheck();
			CustomEditorGUI.BeginLabelWidth(54F);
			float newInitialProgress = EditorGUILayout.Slider("初始进度:", m_Target.initialProgress, 0, 1);
			CustomEditorGUI.EndLabelWidth();
			if (EditorGUI.EndChangeCheck()) {
				Array.ForEach(m_AllTargets, t => {
					Undo.RecordObject(t, "InitialProgress");
					t.initialProgress = newInitialProgress;
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
						GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.37F - 8F)
				);
				if (EditorGUI.EndChangeCheck()) {
					Array.ForEach(m_AllTargets, t => {
						Undo.RecordObject(t, "InvalidateTween");
						t.invalidateTween = newInvalidateTween;
					});
				}
				
				// 进度缓动按钮
				bool tween = m_Target.tween;
				bool tweenMixed = Array.Exists(m_AllTargets, t => t.tween != tween);
				EditorGUI.BeginChangeCheck();
				bool newTween = CustomEditorGUI.Toggle(
						tween && !tweenMixed,
						tweenMixed ? "禁用对象缓动(Mixed)" : "禁用对象缓动",
						tweenMixed ? (CustomEditorGUI.COLOR_TOGGLE_CHECKED + CustomEditorGUI.COLOR_NORMAL) * 0.5F : CustomEditorGUI.COLOR_NORMAL,
						CustomEditorGUI.COLOR_TOGGLE_CHECKED,
						GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.3F - 7F)
				);
				if (EditorGUI.EndChangeCheck()) {
					Array.ForEach(m_AllTargets, t => {
						Undo.RecordObject(t, "Tween");
						t.tween = newTween;
					});
				}
			EditorGUILayout.EndHorizontal();
			
			if (tween || tweenMixed) {
				EditorGUILayout.BeginHorizontal();
					EditorGUI.showMixedValue = Array.Exists(m_AllTargets, t => !Mathf.Approximately(t.tweenDelay, m_Target.tweenDelay));
					EditorGUI.BeginChangeCheck();
					CustomEditorGUI.BeginLabelWidth(38F);
					float newDelay = EditorGUILayout.FloatField("Delay:", m_Target.tweenDelay);
					CustomEditorGUI.EndLabelWidth();
					if (EditorGUI.EndChangeCheck()) {
						Array.ForEach(m_AllTargets, t => {
							Undo.RecordObject(t, "TweenDelay");
							t.tweenDelay = newDelay;
						});
					}
					EditorGUI.showMixedValue = false;
					
					EditorGUI.showMixedValue = Array.Exists(m_AllTargets, t => !Mathf.Approximately(t.tweenDuration, m_Target.tweenDuration));
					EditorGUI.BeginChangeCheck();
					CustomEditorGUI.BeginLabelWidth(54F);
					float newDuration = EditorGUILayout.FloatField("Duration:", m_Target.tweenDuration);
					CustomEditorGUI.EndLabelWidth();
					if (EditorGUI.EndChangeCheck()) {
						Array.ForEach(m_AllTargets, t => {
							Undo.RecordObject(t, "TweenDuration");
							t.tweenDuration = newDuration;
						});
					}
					EditorGUI.showMixedValue = false;
					
					EditorGUI.showMixedValue = Array.Exists(m_AllTargets, t => t.tweenEase != m_Target.tweenEase);
					EditorGUI.BeginChangeCheck();
					Ease newEase = (Ease) EditorGUILayout.EnumPopup(m_Target.tweenEase);
					if (EditorGUI.EndChangeCheck()) {
						Array.ForEach(m_AllTargets, t => {
							Undo.RecordObject(t, "TweenEase");
							t.tweenEase = newEase;
						});
					}
					EditorGUI.showMixedValue = false;
				EditorGUILayout.EndHorizontal();
				
				if (newEase == Ease.INTERNAL_Custom) {
					EditorGUI.showMixedValue = Array.Exists(m_AllTargets, t => !t.tweenEaseCurve.Equals(m_Target.tweenEaseCurve));
					EditorGUI.BeginChangeCheck();
					AnimationCurve newCurve = EditorGUILayout.CurveField(m_Target.tweenEaseCurve);
					if (EditorGUI.EndChangeCheck()) {
						Array.ForEach(m_AllTargets, t => {
							Undo.RecordObject(t, "TweenEaseCurve");
							t.tweenEaseCurve = newCurve;
						});
					}
					EditorGUI.showMixedValue = false;
				}
			}
		}
		
		private void DrawProgressMixed() {
			FieldInfo snapCountField = typeof(ProgressController).GetField("snapCount", BindingFlags.Instance | BindingFlags.NonPublic);
			int snapCount = 0;
			if (snapCountField != null) {
				snapCount = (int) snapCountField.GetValue(m_Target);
				if (Array.Exists(m_AllTargets, t => (int) snapCountField.GetValue(t) != snapCount)) {
					snapCount = -1;
				}
			}
			
			bool isProgressDiff = Array.Exists(m_AllTargets, t => !Mathf.Approximately(t.Progress, m_Target.Progress));
			EditorGUI.showMixedValue = isProgressDiff;
			EditorGUI.BeginChangeCheck();
			float newProgress = EditorGUILayout.Slider(m_Target.Progress, 0, 1);
			if (snapCount > 0) {
				float length = 1F / snapCount;
				newProgress = Mathf.RoundToInt(newProgress / length) * length;
			}
			if (EditorGUI.EndChangeCheck()) {
				Array.ForEach(m_AllTargets, t => {
					Undo.RegisterFullObjectHierarchyUndo(t, "Progress.Change");
					t.Progress = newProgress;
				});
				isProgressDiff = false;
			}
			EditorGUI.showMixedValue = false;

			EditorGUILayout.BeginHorizontal();
			{
				if (snapCount > 0) {
					EditorGUI.showMixedValue = isProgressDiff;
					EditorGUI.BeginChangeCheck();
					CustomEditorGUI.BeginLabelWidth(62F);
					int snapIndex = Mathf.RoundToInt(newProgress * snapCount);
					int newSnapIndex = Mathf.Clamp(EditorGUILayout.IntField("当前吸附点", snapIndex, GUILayout.Width(114F)), 0, snapCount);
					CustomEditorGUI.EndLabelWidth();
					if (EditorGUI.EndChangeCheck()) {
						newProgress = (float) newSnapIndex / snapCount;
						Array.ForEach(m_AllTargets, t => {
							Undo.RegisterFullObjectHierarchyUndo(t, "Progress.Change");
							t.Progress = newProgress;
						});
					}
					EditorGUI.showMixedValue = false;
				} else {
					EditorGUILayout.LabelField("");
				}
				
				GUILayout.FlexibleSpace();
				
				EditorGUI.showMixedValue = snapCount == -1;
				EditorGUI.BeginChangeCheck();
				CustomEditorGUI.BeginBackgroundColor(CustomEditorGUI.COLOR_TOGGLE_CHECKED_EDITOR * 2.75F);
				CustomEditorGUI.BeginLabelWidth(50F);
				GUIContent label = new GUIContent("吸附段数", "影响拖动进度条效果，用于测试离散式赋值");
				int newSnapCount = EditorGUILayout.IntField(label, snapCount, GUILayout.Width(102F));
				CustomEditorGUI.EndLabelWidth();
				CustomEditorGUI.EndBackgroundColor();
				if (EditorGUI.EndChangeCheck()) {
					Array.ForEach(m_AllTargets, t => {
						Undo.RecordObject(t, "SnapCount");
						snapCountField?.SetValue(t, newSnapCount);
					});
				}
				EditorGUI.showMixedValue = false;
			}
			EditorGUILayout.EndHorizontal();
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
						float newInitialProgress = EditorGUILayout.Slider("初始进度:", m_Target.initialProgress, 0, 1);
						if (Mathf.Abs(newInitialProgress - m_Target.initialProgress) > Mathf.Epsilon) {
							Undo.RecordObject(m_Target, "InitialProgress");
							m_Target.initialProgress = newInitialProgress;
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
		
		private void DrawOtherFields() {
			EditorGUILayout.BeginHorizontal();
			{
				// 延迟初始化按钮
				if (CustomEditorGUI.Toggle(m_Target.lazyInit, "延迟初始化", CustomEditorGUI.COLOR_TOGGLE_CHECKED) != m_Target.lazyInit) {
					Undo.RecordObject(m_Target, "LazyInit");
					m_Target.lazyInit = !m_Target.lazyInit;
				}

				// 禁用缓动按钮
				if (CustomEditorGUI.Toggle(m_Target.invalidateTween, "禁用对象缓动", CustomEditorGUI.COLOR_TOGGLE_CHECKED) != m_Target.invalidateTween) {
					Undo.RecordObject(m_Target, "InvalidateTween");
					m_Target.invalidateTween = !m_Target.invalidateTween;
				}
				
				// 进度缓动按钮
				if (CustomEditorGUI.Toggle(m_Target.tween, "进度缓动", CustomEditorGUI.COLOR_TOGGLE_CHECKED) != m_Target.tween) {
					Undo.RecordObject(m_Target, "Tween");
					m_Target.tween = !m_Target.tween;
				}
			}
			EditorGUILayout.EndHorizontal();
			
			if (m_Target.tween) {
				float prevLabelWidth = EditorGUIUtility.labelWidth;
				EditorGUILayout.BeginHorizontal();
				EditorGUIUtility.labelWidth = 38F;
				float newDelay = EditorGUILayout.FloatField("Delay:", m_Target.tweenDelay);
				if (Mathf.Abs(newDelay - m_Target.tweenDelay) > Mathf.Epsilon) {
					Undo.RecordObject(m_Target, "TweenDelay");
					m_Target.tweenDelay = newDelay;
				}
				EditorGUIUtility.labelWidth = 54F;
				float newDuration = EditorGUILayout.FloatField("Duration:", m_Target.tweenDuration);
				if (Mathf.Abs(newDuration - m_Target.tweenDuration) > Mathf.Epsilon) {
					Undo.RecordObject(m_Target, "TweenDuration");
					m_Target.tweenDuration = newDuration;
				}
				Ease newEase = (Ease) EditorGUILayout.EnumPopup(m_Target.tweenEase);
				if (newEase != m_Target.tweenEase) {
					Undo.RecordObject(m_Target, "TweenEase");
					m_Target.tweenEase = newEase;
				}
				EditorGUILayout.EndHorizontal();
				EditorGUIUtility.labelWidth = prevLabelWidth;
				
				if (newEase == Ease.INTERNAL_Custom) {
					EditorGUILayout.BeginHorizontal();
					AnimationCurve newCurve = EditorGUILayout.CurveField(m_Target.tweenEaseCurve);
					if (!newCurve.Equals(m_Target.tweenEaseCurve)) {
						Undo.RecordObject(m_Target, "TweenEaseCurve");
						m_Target.tweenEaseCurve = newCurve;
					}
					EditorGUILayout.EndHorizontal();
				}
			}
		}

		private void DrawProgress() {
			FieldInfo snapCountField = typeof(ProgressController).GetField("snapCount", BindingFlags.Instance | BindingFlags.NonPublic);
			int snapCount = snapCountField == null ? 0 : (int) snapCountField.GetValue(m_Target);
			
			// 进度
			CheckModification("^m_Progress$", -18F, 0F, () => {
				float newProgress = EditorGUILayout.Slider(m_Target.Progress, 0, 1);
				if (snapCount > 0) {
					float length = 1F / snapCount;
					newProgress = Mathf.RoundToInt(newProgress / length) * length;
				}
				if (Mathf.Abs(newProgress - m_Target.Progress) > Mathf.Epsilon) {
					OnProgressChange(newProgress);
				}
			});

			EditorGUILayout.BeginHorizontal();
			{
				float progress = m_Target.Progress;
				if (snapCount > 0) {
					CustomEditorGUI.BeginLabelWidth(62F);
					int snapIndex = Mathf.RoundToInt(progress * snapCount);
					int newSnapIndex = Mathf.Clamp(EditorGUILayout.IntField("当前吸附点", snapIndex, GUILayout.Width(114F)), 0, snapCount);
					if (newSnapIndex != snapIndex) {
						progress = (float) newSnapIndex / snapCount;
						OnProgressChange(progress);
					}
					CustomEditorGUI.EndLabelWidth();
				} else {
					EditorGUILayout.LabelField("");
				}
				GUILayout.FlexibleSpace();

				CheckModification("^snapCount$", -18F, 50F, () => {
					CustomEditorGUI.BeginLabelWidth(50F);
					CustomEditorGUI.BeginBackgroundColor(CustomEditorGUI.COLOR_TOGGLE_CHECKED_EDITOR * 2.75F);
					GUIContent label = new GUIContent("吸附段数", "影响拖动进度条效果，用于测试离散式赋值");
					int newSnapCount = EditorGUILayout.IntField(label, snapCount, GUILayout.Width(102F));
					if (newSnapCount != snapCount) {
						Undo.RecordObject(m_Target, "SnapCount");
						snapCountField?.SetValue(m_Target, newSnapCount);
					}
					CustomEditorGUI.EndBackgroundColor();
					CustomEditorGUI.EndLabelWidth();
				});
			}
			EditorGUILayout.EndHorizontal();
		}

		private void DrawRelationsBrief() {
			StringBuilder sb = new StringBuilder();
			int stateRelationCount = m_Target.relations.Count;
			if (stateRelationCount > 0) {
				sb.Append("关联状态控制:");
				sb.Append(stateRelationCount);
				sb.Append("\t");
			}
			int progressRelationCount = m_Target.stateRelations.Count;
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

		private void DrawProgressRelations(IList<ProgressRelateProgress> relations) {
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
					relations.Add(new ProgressRelateProgress());
				}
				EditorGUILayout.Space(5F, false);
				CustomEditorGUI.BeginDisabled(m_ProgressRelationToPaste == null && relations.Count <= 0);
				if (GUILayout.Button("粘贴", GUILayout.Width(40F))) {
					ProgressRelateProgress basedRelation = m_ProgressRelationToPaste ?? relations[relations.Count - 1];
					ProgressRelateProgress relation = new ProgressRelateProgress {
						minProgress = basedRelation.minProgress,
						maxProgress = basedRelation.maxProgress,
						delay = basedRelation.delay,
						single = basedRelation.single,
						controller = basedRelation.controller,
						targetMinProgress = basedRelation.targetMinProgress,
						targetMaxProgress = basedRelation.targetMaxProgress
					};
					Undo.RecordObject(m_Target, "ProgressRelation.Add");
					relations.Add(relation);
				}
				CustomEditorGUI.EndDisabled();
				EditorGUILayout.EndHorizontal();
			}
		}

		private void DrawStateRelations(IList<ProgressRelateState> relations) {
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
					relations.Add(new ProgressRelateState());
				}
				EditorGUILayout.Space(5F, false);
				CustomEditorGUI.BeginDisabled(m_StateRelationToPaste == null && relations.Count <= 0);
				if (GUILayout.Button("粘贴", GUILayout.Width(40F))) {
					ProgressRelateState basedRelation = m_StateRelationToPaste ?? relations[relations.Count - 1];
					ProgressRelateState relation = new ProgressRelateState {
						minProgress = basedRelation.minProgress,
						maxProgress = basedRelation.maxProgress,
						delay = basedRelation.delay,
						single = basedRelation.single,
						controller = basedRelation.controller,
						targetUID = basedRelation.targetUID,
						targetIndex = basedRelation.targetIndex
					};
					Undo.RecordObject(m_Target, "StateRelation.Add");
					relations.Add(relation);
				}
				CustomEditorGUI.EndDisabled();
				EditorGUILayout.EndHorizontal();
			}
		}

		private void DrawExecutorRelations(IList<ProgressRelateExecutor> relations) {
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
					relations.Add(new ProgressRelateExecutor());
				}
				EditorGUILayout.Space(5F, false);
				CustomEditorGUI.BeginDisabled(m_ExecutorRelationToPaste == null && relations.Count <= 0);
				if (GUILayout.Button("粘贴", GUILayout.Width(40F))) {
					ProgressRelateExecutor basedRelation = m_ExecutorRelationToPaste ?? relations[relations.Count - 1];
					ProgressRelateExecutor relation = new ProgressRelateExecutor {
						minProgress = basedRelation.minProgress,
						maxProgress = basedRelation.maxProgress,
						delay = basedRelation.delay,
						single = basedRelation.single,
						executor = basedRelation.executor,
						canExecuteAgain = basedRelation.canExecuteAgain
					};
					Undo.RecordObject(m_Target, "ExecutorRelation.Add");
					relations.Add(relation);
				}
				CustomEditorGUI.EndDisabled();
				EditorGUILayout.EndHorizontal();
			}
		}
		
		private void DrawRelations<T>(IList<T> relations) where T : ProgressRelate, new() {
			EditorGUILayout.BeginVertical();
			
			for (int i = 0, length = relations.Count; i < length; ++i) {
				T relation = relations[i];

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("当", GUILayout.Width(13F));
				DrawMinMaxSlider(
						ref relation.minProgress, ref relation.maxProgress,
						() => Undo.RecordObject(m_Target, "Relation.Progress")
				);
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				switch (relation) {
					case ProgressRelateProgress progressRelation: {
						// 关联进度
						EditorGUILayout.LabelField("改变进度", GUILayout.Width(49F));
						progressRelation.controller = DrawComponentSelector(
								progressRelation.controller,
								controller => string.IsNullOrEmpty(controller.title) ? "匿名" : controller.title,
								() => Undo.RecordObject(m_Target, "ProgressRelation.Controller")
						);
						if (progressRelation.controller) {
							DrawMinMaxSlider(
									ref progressRelation.targetMinProgress, ref progressRelation.targetMaxProgress,
									() => Undo.RecordObject(m_Target, "ProgressRelation.Target")
							);
						} else {
							CustomEditorGUI.BeginDisabled(true);
							EditorGUILayout.Slider(0, 0, 1);
							CustomEditorGUI.EndDisabled();
						}
						break;
					}
					case ProgressRelateState stateRelation: {
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
								stateRelation.targetIndex = newTargetUID > 0 ? targetUIDs.IndexOf(newTargetUID) - 3 : newTargetUID;
							}
						} else {
							CustomEditorGUI.BeginDisabled(true);
							EditorGUILayout.Popup(0, new [] { "无" });
							CustomEditorGUI.EndDisabled();
						}
						break;
					}
					case ProgressRelateExecutor executorRelation: {
						// 执行
						EditorGUILayout.LabelField("执行", GUILayout.Width(25F));
						executorRelation.executor = DrawComponentSelector(
								executorRelation.executor,
								executor => executor.GetType().Name + " - " + (string.IsNullOrEmpty(executor.title) ? "匿名" : executor.title),
								() => Undo.RecordObject(m_Target, "ExecutorRelation.Execute")
						);
						// 重复执行按钮
						bool newCanExecuteAgain = CustomEditorGUI.Toggle(executorRelation.canExecuteAgain, "可重复响应", CustomEditorGUI.COLOR_TOGGLE_CHECKED, GUILayout.Width(76F));
						if (newCanExecuteAgain != executorRelation.canExecuteAgain) {
							Undo.RecordObject(m_Target, "ExecutorRelation.CanTriggerAgain");
							executorRelation.canExecuteAgain = newCanExecuteAgain;
						}
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
					case ProgressRelateProgress progressRelation: {
						bool isSelect = m_ProgressRelationToPaste == null ? i == length - 1 : m_ProgressRelationToPaste == progressRelation;
						if (CustomEditorGUI.Toggle(isSelect, "复制", CustomEditorGUI.COLOR_TOGGLE_CHECKED_EDITOR, GUILayout.Width(34F)) && !isSelect) {
							m_ProgressRelationToPaste = progressRelation;
						}
						break;
					}
					case ProgressRelateState stateRelation: {
						bool isSelect = m_StateRelationToPaste == null ? i == length - 1 : m_StateRelationToPaste == stateRelation;
						if (CustomEditorGUI.Toggle(isSelect, "复制", CustomEditorGUI.COLOR_TOGGLE_CHECKED_EDITOR, GUILayout.Width(34F)) && !isSelect) {
							m_StateRelationToPaste = stateRelation;
						}
						break;
					}
					case ProgressRelateExecutor executorRelation: {
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
					if (m_ProgressRelationToPaste == relation) {
						m_ProgressRelationToPaste = null;
					} else if (m_StateRelationToPaste == relation) {
						m_StateRelationToPaste = null;
					} else if (m_ExecutorRelationToPaste == relation) {
						m_ExecutorRelationToPaste = null;
					}
				}
				EditorGUILayout.EndHorizontal();
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
			BaseProgressCtrl[] _targets = m_Target.Targets;
			foreach (var _target in _targets) {
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space(10F, false);

				EditorGUILayout.ObjectField(_target, typeof(BaseProgressCtrl), true);

				EditorGUILayout.Space(10F, false);
				EditorGUILayout.EndHorizontal();
			}
			GUI.enabled = prevEnabled;
		}

		private void DrawCapture() {
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("记录状态", BTN_DOUBLE_HEIGHT_OPTION)) {
				Undo.RegisterFullObjectHierarchyUndo(m_Target, "ProgressCapture");
				m_Target.Capture();
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

		private static void DrawMinMaxSlider(ref float minValue, ref float maxValue, Action beforeChange = null) {
			// ReSharper disable once RedundantAssignment
			float newMinValue = minValue;
			float newMaxValue = maxValue;
			
			newMinValue = EditorGUILayout.FloatField(minValue, GUILayout.Width(60F));
			if (Mathf.Abs(newMinValue - minValue) >= Mathf.Epsilon) {
				beforeChange?.Invoke();
				minValue = newMinValue;
			}
			EditorGUILayout.MinMaxSlider(ref newMinValue, ref newMaxValue, 0, 1);
			if (Mathf.Abs(newMinValue - minValue) >= Mathf.Epsilon) {
				beforeChange?.Invoke();
				minValue = newMinValue;
			}
			if (Mathf.Abs(newMaxValue - maxValue) >= Mathf.Epsilon) {
				beforeChange?.Invoke();
				maxValue = newMaxValue;
			}
			newMaxValue = EditorGUILayout.FloatField(maxValue, GUILayout.Width(60F));
			if (Mathf.Abs(newMaxValue - maxValue) >= Mathf.Epsilon) {
				beforeChange?.Invoke();
				maxValue = newMaxValue;
			}
		}

		private void OnProgressChange(float progress) {
			Undo.RegisterFullObjectHierarchyUndo(m_Target, "Progress.Change");
			if (m_DisableCtrl) {
				FieldInfo field = m_Target.GetType().GetField("m_Progress", BindingFlags.Instance | BindingFlags.NonPublic);
				field?.SetValue(m_Target, progress);
			} else {
				m_Target.Progress = progress;
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
			EditorGUI.DrawRect(wireRect, MODIFIED_COLOR);
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

		private static T DrawComponentSelector<T>(T t, Converter<T, string> nameConvert = null, Action beforeChange = null) where T : Component {
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
	}
}
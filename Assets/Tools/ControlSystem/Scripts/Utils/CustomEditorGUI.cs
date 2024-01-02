/*
 * @Author: wangyun
 * @CreateTime: 2023-01-26 22:26:26 084
 * @LastEditor: wangyun
 * @EditTime: 2023-01-26 22:26:26 090
 */

#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Control {
	public static class CustomEditorGUI {
		public static Color COLOR_NORMAL => Color.white;
#if UNITY_2021_1_OR_NEWER
		public static Color COLOR_TOGGLE_CHECKED => new(0, 0.97F, 0.75F, 1);
		public static Color COLOR_TOGGLE_CHECKED_EDITOR => new(1.7F, 0.6F, 0, 1);
#else
		public static Color COLOR_TOGGLE_CHECKED => new Color(0, 0.8F, 0.8F, 1);
		public static Color COLOR_TOGGLE_CHECKED_EDITOR => new Color(1, 0.5F, 0, 1);
#endif
		public static Color COLOR_MODIFIED => new Color(0.06F, 0.5F, 0.75F);
		
		public static GUILayoutOption LabelWidthOption => GUILayout.Width(EditorGUIUtility.labelWidth - 1F);
		
		private static readonly Stack<float> s_LabelWidthStack = new Stack<float>();
		public static void BeginLabelWidth(float labelWidth) {
			s_LabelWidthStack.Push(EditorGUIUtility.labelWidth);
			EditorGUIUtility.labelWidth = labelWidth;
		}
		public static void EndLabelWidth() {
			if (s_LabelWidthStack.Count == 0) {
				Debug.LogError("LabelWidth stack is empty, did you call BeginLabelWidth first?");
			} else {
				EditorGUIUtility.labelWidth = s_LabelWidthStack.Pop();
			}
		}
		
		private static readonly Stack<Vector2> s_IconSizeStack = new Stack<Vector2>();
		public static void BeginIconSize(float iconWidth, float iconHeight) {
			s_IconSizeStack.Push(EditorGUIUtility.GetIconSize());
			EditorGUIUtility.SetIconSize(new Vector2(iconWidth, iconHeight));
		}
		public static void EndIconSize() {
			if (s_IconSizeStack.Count == 0) {
				Debug.LogError("IconSize stack is empty, did you call BeginIconSize first?");
			} else {
				EditorGUIUtility.SetIconSize(s_IconSizeStack.Pop());
			}
		}
		
		private static readonly Stack<Color> s_BackgroundColorStack = new Stack<Color>();
		public static void BeginBackgroundColor(Color color) {
			s_BackgroundColorStack.Push(GUI.backgroundColor);
			GUI.backgroundColor = color;
		}
		public static void EndBackgroundColor() {
			if (s_BackgroundColorStack.Count == 0) {
				Debug.LogError("BackgroundColor stack is empty, did you call BeginBackgroundColor first?");
			} else {
				GUI.backgroundColor = s_BackgroundColorStack.Pop();
			}
		}
		
		private static readonly Stack<(float, Vector2)> s_RotateStack = new Stack<(float, Vector2)>();
		public static void BeginRotate(float angle) {
			BeginRotate(angle, GetNextRect().min);
		}
		public static void BeginRotate(float angle, Vector2 pivotPoint) {
			s_RotateStack.Push((angle, pivotPoint));
			GUIUtility.RotateAroundPivot(angle, pivotPoint);
		}
		public static void EndRotate() {
			if (s_RotateStack.Count == 0) {
				Debug.LogError("Rotate stack is empty, did you call BeginRotate first?");
			} else {
				(float angle, Vector2 pivotPoint) = s_RotateStack.Pop();
				GUIUtility.RotateAroundPivot(-angle, pivotPoint);
			}
		}
		
		private static readonly Stack<(Vector2, Vector2)> s_ScaleStack = new Stack<(Vector2, Vector2)>();
		public static void BeginScale(Vector2 scale) {
			BeginScale(scale, GetNextRect().min);
		}
		public static void BeginScale(Vector2 scale, Vector2 pivotPoint) {
			s_ScaleStack.Push((scale, pivotPoint));
			GUIUtility.ScaleAroundPivot(scale, pivotPoint);
		}
		public static void EndScale() {
			if (s_ScaleStack.Count == 0) {
				Debug.LogError("Rotate stack is empty, did you call BeginScale first?");
			} else {
				(Vector2 scale, Vector2 pivotPoint) = s_ScaleStack.Pop();
				GUIUtility.ScaleAroundPivot(Vector2.one / scale, pivotPoint);
			}
		}
		
		public static void BeginDisabled(bool disabled) {
			EditorGUI.BeginDisabledGroup(disabled);
		}
		public static void EndDisabled() {
			EditorGUI.EndDisabledGroup();
		}
		public static void ChangeDisabled(bool disabled) {
			EndDisabled();
			BeginDisabled(disabled);
		}
		
		private static readonly Stack<bool> s_BoldStack = new Stack<bool>();
		public static void BeginBold(bool isBold) {
			s_BoldStack.Push(IsBold());
			SetBold(isBold);
		}
		public static void EndBold() {
			if (s_BoldStack.Count == 0) {
				Debug.LogError("Bold stack is empty, did you call BeginBold first?");
			} else {
				bool isBold = s_BoldStack.Pop();
				SetBold(isBold);
			}
		}
		private static bool IsBold() {
			// return EditorGUIUtility.GetBoldDefaultFont();
			MethodInfo getBoldDefaultFontMI = typeof(EditorGUIUtility).GetMethod("GetBoldDefaultFont", BindingFlags.Static | BindingFlags.NonPublic);
			return getBoldDefaultFontMI?.Invoke(null, null) is bool isBold && isBold;
		}
		private static void SetBold(bool isBold) {
			// EditorGUIUtility.SetBoldDefaultFont(isBold);
			MethodInfo getBoldDefaultFontMI = typeof(EditorGUIUtility).GetMethod("SetBoldDefaultFont", BindingFlags.Static | BindingFlags.NonPublic);
			getBoldDefaultFontMI?.Invoke(null, new object[] { isBold });
		}
		
	
		public static void Space(float space, bool expand = true) {
			if (expand) {
				if (IsVertical()) {
					GUILayoutUtility.GetRect(0, space, GUILayout.ExpandWidth(true));
				} else {
					GUILayoutUtility.GetRect(space, 0, GUILayout.ExpandWidth(true));
				}
				if (Event.current.type == EventType.Layout) {
					// GUILayoutUtility.current.topLevel.entries[GUILayoutUtility.current.topLevel.entries.Count - 1].consideredForMargin = false;
					object topLevel = GetTopLevel();
					FieldInfo entriesFI = topLevel?.GetType().GetField("entries", BindingFlags.Instance | BindingFlags.Public);
					IList entries = entriesFI?.GetValue(topLevel) as IList;
					object last = entries?[entries.Count - 1];
					FieldInfo consideredForMarginFI = last?.GetType().GetField("consideredForMargin", BindingFlags.Instance | BindingFlags.Public);
					consideredForMarginFI?.SetValue(last, false);
				}
			} else {
				GUILayout.Space(space);
			}
		}
	
		private const BindingFlags FLAG_INSTANCE = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		private const BindingFlags FLAG_INSTANCE_IGNORE_CASE = FLAG_INSTANCE | BindingFlags.IgnoreCase;
		private static readonly string[] CUSTOM_LABEL_KEYS = {"id", "title"};
		public static string GetCustomLabel(Component comp) {
			Type type = comp.GetType();
			foreach (var key in CUSTOM_LABEL_KEYS) {
				FieldInfo fi = type.GetField(key, FLAG_INSTANCE) ?? type.GetField(key, FLAG_INSTANCE_IGNORE_CASE);
				if (fi != null) {
					object value = fi.GetValue(comp);
					string valueStr = value?.ToString();
					if (!string.IsNullOrEmpty(valueStr)) {
						return valueStr;
					}
				}
				PropertyInfo pi = type.GetProperty(key, FLAG_INSTANCE) ?? type.GetProperty(key, FLAG_INSTANCE_IGNORE_CASE);
				if (pi != null) {
					object value = pi.GetValue(comp);
					string valueStr = value?.ToString();
					if (!string.IsNullOrEmpty(valueStr)) {
						return valueStr;
					}
				}
			}
			return null;
		}
		
		public static float GetContextWidth() {
			// return EditorGUIUtility.contextWidth;
			PropertyInfo pi = typeof(EditorGUIUtility).GetProperty("contextWidth", BindingFlags.Static | BindingFlags.NonPublic);
			return (float) (pi?.GetValue(null) ?? EditorGUIUtility.currentViewWidth);
		}
		
		public static bool IsVertical() {
			// return GUILayoutUtility.current.topLevel.isVertical;
			object topLevel = GetTopLevel();
			FieldInfo isVerticalFI = topLevel?.GetType().GetField("isVertical", BindingFlags.Instance | BindingFlags.Public);
			return isVerticalFI?.GetValue(topLevel) is bool bIsVertical && bIsVertical;
		}
		
		public static int GetCursor() {
			// return GUILayoutUtility.current.topLevel.m_Cursor;
			object topLevel = GetTopLevel();
			FieldInfo cursorFI = topLevel?.GetType().GetField("m_Cursor", BindingFlags.Instance | BindingFlags.NonPublic);
			return cursorFI?.GetValue(topLevel) is int iCursor ? iCursor : 0;
		}
		
		public static bool IsPrefabComparing() {
			// return (GUIView.current as HostView)?.actualView is PopupWindowWithoutFocus;
			Type viewType = typeof(EditorGUIUtility).Assembly.GetType("UnityEditor.GUIView");
			PropertyInfo currentPI = viewType?.GetProperty("current", BindingFlags.Static | BindingFlags.Public);
			object view = currentPI?.GetValue(null);
			PropertyInfo actualViewPI = view?.GetType().GetProperty("actualView", BindingFlags.Instance | BindingFlags.NonPublic);
			object window = actualViewPI?.GetValue(view);
			return window?.GetType().Name == "PopupWindowWithoutFocus";
		}
		
		public static Rect GetLastRect() {
			switch (Event.current.type) {
				case EventType.Layout:
				case EventType.Repaint:
					return GUILayoutUtility.GetLastRect();
				default:
					// return EditorGUILayout.s_LastRect;
					FieldInfo lastRectFI = typeof(EditorGUILayout).GetField("s_LastRect", BindingFlags.Static | BindingFlags.NonPublic);
					return lastRectFI?.GetValue(null) is Rect rect ? rect : new Rect();
			}
		}
		
		public static Rect GetNextRect() {
			// GUILayoutGroup topLevel = GUILayoutUtility.current.topLevel;
			// return topLevel.m_Cursor > 0 ? topLevel.PeekNext() : topLevel.rect;
			object topLevel = GetTopLevel();
			FieldInfo cursorFI = topLevel?.GetType().GetField("m_Cursor", BindingFlags.Instance | BindingFlags.NonPublic);
			if (cursorFI?.GetValue(topLevel) is int iCursor && iCursor > 0) {
				MethodInfo peekNextFI = topLevel.GetType().GetMethod("PeekNext", BindingFlags.Instance | BindingFlags.Public);
				return peekNextFI?.Invoke(topLevel, null) is Rect rect ? rect : new Rect();
			} else {
				FieldInfo rectFI = topLevel?.GetType().GetField("rect", BindingFlags.Instance | BindingFlags.Public);
				return rectFI?.GetValue(topLevel) is Rect rect ? rect : new Rect();
			}
		}
		
		public static object GetTopLevel() {
			// return GUILayoutUtility.current.topLevel;
			FieldInfo currentFI = typeof(GUILayoutUtility).GetField("current", BindingFlags.Static | BindingFlags.NonPublic);
			object current = currentFI?.GetValue(null);
			FieldInfo topLevelFI = current?.GetType().GetField("topLevel", BindingFlags.Instance | BindingFlags.NonPublic);
			return topLevelFI?.GetValue(current);
		}
		
		public static void Repaint() {
			// GUIView.current.Repaint();
			Type viewType = typeof(EditorGUIUtility).Assembly.GetType("UnityEditor.GUIView");
			PropertyInfo currentPI = viewType?.GetProperty("current", BindingFlags.Static | BindingFlags.Public);
			object view = currentPI?.GetValue(null);
			MethodInfo repaintMI = viewType?.GetMethod("Repaint", BindingFlags.Instance | BindingFlags.Public);
			repaintMI?.Invoke(view, null);
		}
	
		public static void RepaintAllInspectors() {
			// InspectorWindow.RepaintAllInspectors();
			Type inspectorType = typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow");
			MethodInfo repaintAllInspectorsFI = inspectorType?.GetMethod("RepaintAllInspectors", BindingFlags.Static | BindingFlags.NonPublic);
			repaintAllInspectorsFI?.Invoke(null, null);
			// // InspectorWindow.m_AllInspectors.ForEach(inspector => inspector.Repaint());
			// Type inspectorType = typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow");
			// FieldInfo allInspectorsFI = inspectorType?.GetField("m_AllInspectors", BindingFlags.Static | BindingFlags.NonPublic);
			// if (allInspectorsFI?.GetValue(null) is IList allInspectors) {
			// 	foreach (var inspector in allInspectors) {
			// 		if (inspector is EditorWindow window) {
			// 			window.Repaint();
			// 		}
			// 	}
			// }
		}
	
		public static void RepaintEditorWindows<T>() where T : EditorWindow {
			T[] windows = Resources.FindObjectsOfTypeAll<T>();
			foreach (var window in windows) {
				window.Repaint();
			}
		}
	
		public static void RepaintScene() {
			EditorApplication.QueuePlayerLoopUpdate();
		}
	
		public static void RepaintAllViews() {
			UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
		}
		
		public static bool Button(string text, Color buttonColor, params GUILayoutOption[] options) {
			return Button(EditorGUIUtility.TrTempContent(text), buttonColor, options);
		}
		private static bool Button(GUIContent content, Color buttonColor, params GUILayoutOption[] options) {
			BeginBackgroundColor(buttonColor);
			bool newValue = GUILayout.Button(content, "Button", options);
			EndBackgroundColor();
			return newValue;
		}
		
		public static bool Toggle(bool value, string text, Color checkedColor, params GUILayoutOption[] options) {
			return Toggle(value, EditorGUIUtility.TrTempContent(text), checkedColor, options);
		}
		public static bool Toggle(bool value, GUIContent content, Color checkedColor, params GUILayoutOption[] options) {
			return Toggle(value, content, COLOR_NORMAL, checkedColor, options);
		}
		public static bool Toggle(bool value, string text, Color uncheckedColor, Color checkedColor, params GUILayoutOption[] options) {
			return Toggle(value, EditorGUIUtility.TrTempContent(text), uncheckedColor, checkedColor, options);
		}
		public static bool Toggle(bool value, GUIContent content, Color uncheckedColor, Color checkedColor, params GUILayoutOption[] options) {
			BeginBackgroundColor(value ? checkedColor : uncheckedColor);
			bool newValue = GUILayout.Toggle(value, content, "Button", options);
			EndBackgroundColor();
			return newValue;
		}
		
		public static void IntPopupMultiSelect(Action<int[]> onChange, Rect rect, int[] selectedValues, string[] displayedOptions, int[] optionValues) {
			IntPopupMultiSelect(onChange, rect, (GUIContent) null, selectedValues, displayedOptions, optionValues);
		}
		public static void IntPopupMultiSelect(Action<int[]> onChange, Rect rect, string label, int[] selectedValues, string[] displayedOptions, int[] optionValues) {
			IntPopupMultiSelect(onChange, rect, label, selectedValues, displayedOptions, optionValues, "MiniPullDown");
		}
		public static void IntPopupMultiSelect(Action<int[]> onChange, Rect rect, GUIContent content, int[] selectedValues, string[] displayedOptions, int[] optionValues) {
			IntPopupMultiSelect(onChange, rect, content, selectedValues, displayedOptions, optionValues, "MiniPullDown");
		}
		public static void IntPopupMultiSelect(Action<int[]> onChange, Rect rect, string label, int[] selectedValues, string[] displayedOptions, int[] optionValues, GUIStyle style) {
			IntPopupMultiSelect(onChange, rect, label == null ? null : EditorGUIUtility.TrTempContent(label), selectedValues, displayedOptions, optionValues, style);
		}
		public static void IntPopupMultiSelect(Action<int[]> onChange, Rect rect, GUIContent content, int[] selectedValues, string[] displayedOptions, int[] optionValues, GUIStyle style) {
			IntPopupMultiSelect(onChange, rect, content, selectedValues, EditorGUIUtility.TrTempContent(displayedOptions), optionValues, style);
		}
		public static void IntPopupMultiSelect(Action<int[]> onChange, Rect rect, int[] selectedValues, GUIContent[] displayedOptions, int[] optionValues) {
			IntPopupMultiSelect(onChange, rect, (GUIContent) null, selectedValues, displayedOptions, optionValues);
		}
		public static void IntPopupMultiSelect(Action<int[]> onChange, Rect rect, string label, int[] selectedValues, GUIContent[] displayedOptions, int[] optionValues) {
			IntPopupMultiSelect(onChange, rect, label, selectedValues, displayedOptions, optionValues, "MiniPullDown");
		}
		public static void IntPopupMultiSelect(Action<int[]> onChange, Rect rect, GUIContent content, int[] selectedValues, GUIContent[] displayedOptions, int[] optionValues) {
			IntPopupMultiSelect(onChange, rect, content, selectedValues, displayedOptions, optionValues, "MiniPullDown");
		}
		public static void IntPopupMultiSelect(Action<int[]> onChange, Rect rect, string label, int[] selectedValues, GUIContent[] displayedOptions, int[] optionValues, GUIStyle style) {
			IntPopupMultiSelect(onChange, rect, label == null ? null : EditorGUIUtility.TrTempContent(label), selectedValues, displayedOptions, optionValues, style);
		}
		public static void IntPopupMultiSelect(Action<int[]> onChange, Rect rect, GUIContent content, int[] selectedValues, GUIContent[] displayedOptions, int[] optionValues, GUIStyle style) {
			(bool[] isSelects, List<GUIContent> displayedContents) = GetSelects(selectedValues, displayedOptions, optionValues);
			PopupMultiSelect(onChange == null ? (Action<bool[]>) null : _isSelects => {
				List<int> selectValueList = new List<int>();
				for (int i = 0, optionLength = optionValues.Length; i < optionLength; ++i) {
					if (_isSelects[i]) {
						selectValueList.Add(optionValues[i]);
					}
				}
				onChange(selectValueList.ToArray());
			}, rect, content, isSelects, displayedContents.ToArray(), style);
		}
		public static void IntPopupMultiSelect(Action<int[]> onChange, int[] selectedValues, string[] displayedOptions, int[] optionValues, params GUILayoutOption[] options) {
			IntPopupMultiSelect(onChange, (GUIContent) null, selectedValues, displayedOptions, optionValues, options);
		}
		public static void IntPopupMultiSelect(Action<int[]> onChange, string label, int[] selectedValues, string[] displayedOptions, int[] optionValues, params GUILayoutOption[] options) {
			IntPopupMultiSelect(onChange, label, selectedValues, displayedOptions, optionValues, "MiniPullDown", options);
		}
		public static void IntPopupMultiSelect(Action<int[]> onChange, GUIContent content, int[] selectedValues, string[] displayedOptions, int[] optionValues, params GUILayoutOption[] options) {
			IntPopupMultiSelect(onChange, content, selectedValues, displayedOptions, optionValues, "MiniPullDown", options);
		}
		public static void IntPopupMultiSelect(Action<int[]> onChange, string label, int[] selectedValues, string[] displayedOptions, int[] optionValues, GUIStyle style, params GUILayoutOption[] options) {
			IntPopupMultiSelect(onChange, label == null ? null : EditorGUIUtility.TrTempContent(label), selectedValues, displayedOptions, optionValues, style, options);
		}
		public static void IntPopupMultiSelect(Action<int[]> onChange, GUIContent content, int[] selectedValues, string[] displayedOptions, int[] optionValues, GUIStyle style, params GUILayoutOption[] options) {
			IntPopupMultiSelect(onChange, content, selectedValues, EditorGUIUtility.TrTempContent(displayedOptions), optionValues, style, options);
		}
		public static void IntPopupMultiSelect(Action<int[]> onChange, int[] selectedValues, GUIContent[] displayedOptions, int[] optionValues, params GUILayoutOption[] options) {
			IntPopupMultiSelect(onChange, (GUIContent) null, selectedValues, displayedOptions, optionValues, options);
		}
		public static void IntPopupMultiSelect(Action<int[]> onChange, string label, int[] selectedValues, GUIContent[] displayedOptions, int[] optionValues, params GUILayoutOption[] options) {
			IntPopupMultiSelect(onChange, label, selectedValues, displayedOptions, optionValues, "MiniPullDown", options);
		}
		public static void IntPopupMultiSelect(Action<int[]> onChange, GUIContent content, int[] selectedValues, GUIContent[] displayedOptions, int[] optionValues, params GUILayoutOption[] options) {
			IntPopupMultiSelect(onChange, content, selectedValues, displayedOptions, optionValues, "MiniPullDown", options);
		}
		public static void IntPopupMultiSelect(Action<int[]> onChange, string label, int[] selectedValues, GUIContent[] displayedOptions, int[] optionValues, GUIStyle style, params GUILayoutOption[] options) {
			IntPopupMultiSelect(onChange, label == null ? null : EditorGUIUtility.TrTempContent(label), selectedValues, displayedOptions, optionValues, style, options);
		}
		public static void IntPopupMultiSelect(Action<int[]> onChange, GUIContent content, int[] selectedValues, GUIContent[] displayedOptions, int[] optionValues, GUIStyle style, params GUILayoutOption[] options) {
			(bool[] isSelects, List<GUIContent> displayedContents) = GetSelects(selectedValues, displayedOptions, optionValues);
			PopupMultiSelect(onChange == null ? (Action<bool[]>) null : _isSelects => {
				List<int> selectValueList = new List<int>();
				for (int i = 0, optionLength = optionValues.Length; i < optionLength; ++i) {
					if (_isSelects[i]) {
						selectValueList.Add(optionValues[i]);
					}
				}
				onChange(selectValueList.ToArray());
			}, content, isSelects, displayedContents.ToArray(), style, options);
		}
		private static (bool[] isSelects, List<GUIContent> displayedContents) GetSelects(int[] selectedValues, GUIContent[] displayedOptions, int[] optionValues) {
			int optionLength = optionValues.Length;
			List<GUIContent> displayedContents = new List<GUIContent>(displayedOptions);
			for (int i = displayedContents.Count; i < optionLength; ++i) {
				displayedContents.Add(EditorGUIUtility.TrTempContent(optionValues[i] + ""));
			}
			for (int i = displayedContents.Count - 1; i >= optionLength; --i) {
				displayedContents.RemoveAt(i);
			}
			HashSet<int> selectedValueSet = new HashSet<int>(selectedValues);
			bool[] isSelects = new bool[optionLength];
			for (int i = 0; i < optionLength; ++i) {
				isSelects[i] = selectedValueSet.Contains(optionValues[i]);
			}
			return (isSelects, displayedContents);
		}
		
		public static void PopupMultiSelect(Action<bool[]> onChange, Rect rect, bool[] isSelects, string[] displayedOptions) {
			PopupMultiSelect(onChange, rect, (GUIContent) null, isSelects, displayedOptions);
		}
		public static void PopupMultiSelect(Action<bool[]> onChange, Rect rect, string label, bool[] isSelects, string[] displayedOptions) {
			PopupMultiSelect(onChange, rect, label, isSelects, displayedOptions, "MiniPullDown");
		}
		public static void PopupMultiSelect(Action<bool[]> onChange, Rect rect, GUIContent content, bool[] isSelects, string[] displayedOptions) {
			PopupMultiSelect(onChange, rect, content, isSelects, displayedOptions, "MiniPullDown");
		}
		public static void PopupMultiSelect(Action<bool[]> onChange, Rect rect, string label, bool[] isSelects, string[] displayedOptions, GUIStyle style) {
			PopupMultiSelect(onChange, rect, EditorGUIUtility.TrTempContent(label), isSelects, displayedOptions, style);
		}
		public static void PopupMultiSelect(Action<bool[]> onChange, Rect rect, GUIContent content, bool[] isSelects, string[] displayedOptions, GUIStyle style) {
			PopupMultiSelect(onChange, rect, content, isSelects, EditorGUIUtility.TrTempContent(displayedOptions), style);
		}
		public static void PopupMultiSelect(Action<bool[]> onChange, Rect rect, bool[] isSelects, GUIContent[] displayedOptions) {
			PopupMultiSelect(onChange, rect, (GUIContent) null, isSelects, displayedOptions);
		}
		public static void PopupMultiSelect(Action<bool[]> onChange, Rect rect, string label, bool[] isSelects, GUIContent[] displayedOptions) {
			PopupMultiSelect(onChange, rect, label, isSelects, displayedOptions, "MiniPullDown");
		}
		public static void PopupMultiSelect(Action<bool[]> onChange, Rect rect, GUIContent content, bool[] isSelects, GUIContent[] displayedOptions) {
			PopupMultiSelect(onChange, rect, content, isSelects, displayedOptions, "MiniPullDown");
		}
		public static void PopupMultiSelect(Action<bool[]> onChange, Rect rect, string label, bool[] isSelects, GUIContent[] displayedOptions, GUIStyle style) {
			PopupMultiSelect(onChange, rect, EditorGUIUtility.TrTempContent(label), isSelects, displayedOptions, style);
		}
		public static void PopupMultiSelect(Action<bool[]> onChange, Rect rect, GUIContent content, bool[] isSelects, GUIContent[] displayedOptions, GUIStyle style) {
			if (content == null) {
				content = GetContent(isSelects, displayedOptions);
				Debug.LogError(content);
			}
			if (EditorGUI.DropdownButton(rect, content, FocusType.Keyboard, style)) {
				PopupWindow.Show(rect, new PopupMultiSelectContent(onChange, isSelects, displayedOptions));
			}
		}
		public static void PopupMultiSelect(Action<bool[]> onChange, bool[] isSelects, string[] displayedOptions, params GUILayoutOption[] options) {
			PopupMultiSelect(onChange, (GUIContent) null, isSelects, displayedOptions, options);
		}
		public static void PopupMultiSelect(Action<bool[]> onChange, string label, bool[] isSelects, string[] displayedOptions, params GUILayoutOption[] options) {
			PopupMultiSelect(onChange, label, isSelects, displayedOptions, "MiniPullDown", options);
		}
		public static void PopupMultiSelect(Action<bool[]> onChange, GUIContent content, bool[] isSelects, string[] displayedOptions, params GUILayoutOption[] options) {
			PopupMultiSelect(onChange, content, isSelects, displayedOptions, "MiniPullDown", options);
		}
		public static void PopupMultiSelect(Action<bool[]> onChange, string label, bool[] isSelects, string[] displayedOptions, GUIStyle style, params GUILayoutOption[] options) {
			PopupMultiSelect(onChange, EditorGUIUtility.TrTempContent(label), isSelects, displayedOptions, style, options);
		}
		public static void PopupMultiSelect(Action<bool[]> onChange, GUIContent content, bool[] isSelects, string[] displayedOptions, GUIStyle style, params GUILayoutOption[] options) {
			PopupMultiSelect(onChange, content, isSelects, EditorGUIUtility.TrTempContent(displayedOptions), style, options);
		}
		public static void PopupMultiSelect(Action<bool[]> onChange, bool[] isSelects, GUIContent[] displayedOptions, params GUILayoutOption[] options) {
			PopupMultiSelect(onChange, (GUIContent) null, isSelects, displayedOptions, options);
		}
		public static void PopupMultiSelect(Action<bool[]> onChange, string label, bool[] isSelects, GUIContent[] displayedOptions, params GUILayoutOption[] options) {
			PopupMultiSelect(onChange, label, isSelects, displayedOptions, "MiniPullDown", options);
		}
		public static void PopupMultiSelect(Action<bool[]> onChange, GUIContent content, bool[] isSelects, GUIContent[] displayedOptions, params GUILayoutOption[] options) {
			PopupMultiSelect(onChange, content, isSelects, displayedOptions, "MiniPullDown", options);
		}
		public static void PopupMultiSelect(Action<bool[]> onChange, string label, bool[] isSelects, GUIContent[] displayedOptions, GUIStyle style, params GUILayoutOption[] options) {
			PopupMultiSelect(onChange, EditorGUIUtility.TrTempContent(label), isSelects, displayedOptions, style, options);
		}
		public static void PopupMultiSelect(Action<bool[]> onChange, GUIContent content, bool[] isSelects, GUIContent[] displayedOptions, GUIStyle style, params GUILayoutOption[] options) {
			if (content == null) {
				content = GetContent(isSelects, displayedOptions);
			}
			if (EditorGUILayout.DropdownButton(content, FocusType.Keyboard, style, options)) {
				PopupWindow.Show(GetLastRect(), new PopupMultiSelectContent(onChange, isSelects, displayedOptions));
			}
		}
		private static GUIContent GetContent(IReadOnlyList<bool> isSelects, IReadOnlyList<GUIContent> displayedOptions) {
			int isSelectLength = isSelects.Count;
			bool nothing = true;
			for (int i = 0; i < isSelectLength; ++i) {
				if (isSelects[i]) {
					nothing = false;
					break;
				}
			}
			if (nothing) {
				return EditorGUIUtility.TrTempContent("Nothing");
			}
			
			bool everything = true;
			int optionLength = displayedOptions.Count;
			if (isSelectLength != optionLength) {
				everything = false;
			} else {
				for (int i = 0; i < isSelectLength; ++i) {
					if (!isSelects[i]) {
						everything = false;
						break;
					}
				}
			}
			if (everything) {
				return EditorGUIUtility.TrTempContent("Everything");
			}
			
			List<string> list = new List<string>();
			for (int i = 0; i < isSelectLength; ++i) {
				if (isSelects[i]) {
					list.Add(i < optionLength ? displayedOptions[i].text : string.Empty);
				}
			}
			return EditorGUIUtility.TrTempContent(string.Join(",", list));
		}

		private class PopupMultiSelectContent : PopupWindowContent {
			private const float WIDTH_MAX = 200F;
			private const float HEIGHT_MAX = 600F;
			private const float LINE_HEIGHT = 18F;
			
			private static readonly GUIStyle s_Style = new GUIStyle();
			
			private Action<bool[]> OnChange { get; }
			private List<bool> IsSelects { get; }
			private List<GUIContent> DisplayedOptions { get; }

			private Vector2 m_ScrollPosition;

			public PopupMultiSelectContent(Action<bool[]> onChange, bool[] isSelects, GUIContent[] displayedOptions) {
				OnChange = onChange;

				IsSelects = new List<bool>(isSelects);
				DisplayedOptions = new List<GUIContent>(displayedOptions);
				
				int optionCount = DisplayedOptions.Count;
				for (int i = IsSelects.Count; i < optionCount; ++i) {
					IsSelects.Add(false);
				}
				for (int i = IsSelects.Count - 1; i >= optionCount; --i) {
					IsSelects.RemoveAt(i);
				}
			}

			public override void OnGUI(Rect rect) {
				int optionCount = DisplayedOptions.Count;
				
				m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);
				EditorGUI.BeginChangeCheck();
				
				bool everythingSelected = IsSelects.TrueForAll(select => select);
				bool newEverythingSelected = Toggle(everythingSelected, "All", COLOR_TOGGLE_CHECKED, GUILayout.Height(LINE_HEIGHT));
				if (newEverythingSelected != everythingSelected) {
					for (int i = 0; i < optionCount; ++i) {
						IsSelects[i] = newEverythingSelected;
					}
				}
				
				if (GUILayout.Button("Revert", "Button", GUILayout.Height(LINE_HEIGHT))) {
					for (int i = 0; i < optionCount; ++i) {
						IsSelects[i] = !IsSelects[i];
					}
				}
				
				for (int i = 0; i < optionCount; ++i) {
					IsSelects[i] = GUILayout.Toggle(IsSelects[i], DisplayedOptions[i], "MenuToggleItem", GUILayout.Height(LINE_HEIGHT));
				}
				
				if (EditorGUI.EndChangeCheck()) {
					OnChange?.Invoke(IsSelects.ToArray());
				}
				EditorGUILayout.EndScrollView();
			}
			
			public override Vector2 GetWindowSize() {
				float width = 0;
				float height = 0;
				foreach (var displayedOption in DisplayedOptions) {
					float _width = s_Style.CalcSize(displayedOption).x;
					if (width < _width) {
						width = _width;
					}
					height += LINE_HEIGHT;
				}
				float scrollBarWidth = 0;
				float scrollBarHeight = 0;
				if (width > WIDTH_MAX) {
					width = WIDTH_MAX;
					scrollBarHeight = 13F;
				}
				if (height > HEIGHT_MAX) {
					height = HEIGHT_MAX;
					scrollBarWidth = 13F;
				}
				width = Mathf.Max(width, 60F) + 44F + scrollBarWidth;
				height = Mathf.Max(height, 0F) + 2F + LINE_HEIGHT + 2F + LINE_HEIGHT + 2F + scrollBarHeight;
				return new Vector2(width, height);
			}
		}
	}
}

#endif
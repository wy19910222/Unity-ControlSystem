/*
 * @Author: wangyun
 * @CreateTime: 2022-12-15 12:41:44 458
 * @LastEditor: wangyun
 * @EditTime: 2023-03-04 17:18:52 263
 */

#if LUA_BEHAVIOUR_EXIST
using System;
using UnityEngine;
using UnityEditor;
using LuaApp;

namespace Control {
	public partial class ProcessStepDrawerBase<TStep> {
		private LuaInjectionDataDrawer m_InjectionDrawer;
		private LuaInjectionDataDrawer InjectionDrawer => m_InjectionDrawer ?? (m_InjectionDrawer = new LuaInjectionDataDrawer());
		
		private void DrawLuaCodeExecute() {
			LuaBehaviour newObj = DrawCompFieldWithThisBtn<LuaBehaviour>("self", Target.obj);
			if (newObj != Target.obj) {
				Property.RecordForUndo("Obj");
				Target.obj = newObj;
			}

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("参数", CustomEditorGUI.LabelWidthOption);
			InjectionDrawer.DrawDict(Target.luaInjectionList, GetTargetTrigger(), true);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("代码", CustomEditorGUI.LabelWidthOption);
			string newLuaCode = EditorGUILayout.TextArea(Target.sArguments[0]);
			if (newLuaCode != Target.sArguments[0]) {
				Property.RecordForUndo("SArguments");
				Target.sArguments[0] = newLuaCode;
			}
			EditorGUILayout.EndHorizontal();
		}

		private void DrawLuaSetValue() {
			LuaBehaviourWithPath newObj = DrawCompFieldWithThisBtn<LuaBehaviourWithPath>("作用域", Target.obj);
			if (newObj != Target.obj) {
				Property.RecordForUndo("Obj");
				Target.obj = newObj;
			}
			if (newObj) {
				EditorGUILayout.BeginHorizontal();
				string[] fieldNames = LuaInjectionDataDeduce.GetFieldNames(newObj.luaPath);
				int count = fieldNames.Length;
				string[] displayOptions = new string[count + 1];
				int[] optionValues = new int[count + 1];
				displayOptions[0] = "手动填写";
				optionValues[0] = -1;
				for (int i = 0; i < count; ++i) {
					displayOptions[i + 1] = fieldNames[i];
					optionValues[i + 1] = i;
				}
				string fieldName = Target.sArguments[0];
				int index = Array.IndexOf(fieldNames, fieldName);
				int newIndex = EditorGUILayout.IntPopup("相对路径", index, displayOptions, optionValues);
				if (newIndex != index) {
					Property.RecordForUndo("SArguments");
					Target.sArguments[0] = fieldName = newIndex == -1 ? null : fieldNames[newIndex];
				}
				if (string.IsNullOrEmpty(fieldName)) {
					string newFieldPath = EditorGUILayout.TextField(Target.sArguments[1], GUILayout.Width(s_ContextWidth * 0.3F));
					if (newFieldPath != Target.sArguments[1]) {
						Property.RecordForUndo("SArguments");
						Target.sArguments[1] = newFieldPath;
					}
				}
				EditorGUILayout.EndHorizontal();
			} else {
				Target.sArguments[0] = null;
				string newFieldPath = EditorGUILayout.TextField("绝对路径", Target.sArguments[1]);
				if (newFieldPath != Target.sArguments[1]) {
					Property.RecordForUndo("SArguments");
					Target.sArguments[1] = newFieldPath;
				}
			}
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("参数", CustomEditorGUI.LabelWidthOption);
			InjectionDrawer.DrawSingle(Target.luaInjectionList[0], GetTargetTrigger(), true);
			EditorGUILayout.EndHorizontal();
		}

		private void DrawLuaFunctionInvoke() {
			LuaBehaviourWithPath newObj = DrawCompFieldWithThisBtn<LuaBehaviourWithPath>("作用域", Target.obj);
			if (newObj != Target.obj) {
				Property.RecordForUndo("Obj");
				Target.obj = newObj;
			}
			if (newObj) {
				EditorGUILayout.BeginHorizontal();
				string[] funcNames = LuaInjectionDataDeduce.GetMethodNames(newObj.luaPath);
				int count = funcNames.Length;
				string[] displayOptions = new string[count + 1];
				int[] optionValues = new int[count + 1];
				displayOptions[0] = "手动填写";
				optionValues[0] = -1;
				for (int i = 0; i < count; ++i) {
					displayOptions[i + 1] = funcNames[i];
					optionValues[i + 1] = i;
				}
				string funcName = Target.sArguments[0];
				int index = Array.IndexOf(funcNames, funcName);
				int newIndex = EditorGUILayout.IntPopup("相对路径", index, displayOptions, optionValues);
				if (newIndex != index) {
					Property.RecordForUndo("SArguments");
					Target.sArguments[0] = funcName = newIndex == -1 ? null : funcNames[newIndex];
				}
				if (string.IsNullOrEmpty(funcName)) {
					string newFuncPath = EditorGUILayout.TextField(Target.sArguments[1], GUILayout.Width(s_ContextWidth * 0.3F));
					if (newFuncPath != Target.sArguments[1]) {
						Property.RecordForUndo("SArguments");
						Target.sArguments[1] = newFuncPath;
					}
				}
				EditorGUILayout.EndHorizontal();
			} else {
				Target.sArguments[0] = null;
				string newFuncPath = EditorGUILayout.TextField("绝对路径", Target.sArguments[1]);
				if (newFuncPath != Target.sArguments[1]) {
					Property.RecordForUndo("SArguments");
					Target.sArguments[1] = newFuncPath;
				}
			}
			bool newIsInstanceInvoke = DrawEnumButtons("调用方式", Target.bArguments[0] ? 1 : 0, new [] { ".Call", ":Call" }, new [] { 0, 1 }) > 0;
			if (newIsInstanceInvoke != Target.bArguments[0]) {
				Property.RecordForUndo("BArguments");
				Target.bArguments[0] = newIsInstanceInvoke;
			}
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("参数", CustomEditorGUI.LabelWidthOption);
			InjectionDrawer.DrawList(Target.luaInjectionList, GetTargetTrigger(), true);
			EditorGUILayout.EndHorizontal();
		}
	}
}
#endif
/*
 * @Author: wangyun
 * @CreateTime: 2023-02-07 01:19:24 372
 * @LastEditor: wangyun
 * @EditTime: 2023-02-07 01:19:24 378
 */

#if LUA_BEHAVIOUR_EXIST

using System.Collections.Generic;
using UnityEngine;
using LuaApp;
using XLua;

namespace Control {
	public partial class BaseProcessStep {
		[HideInInspector]
		public List<Injection7> luaInjectionList = new List<Injection7>();
		
		private void DoStepLuaCodeExecute() {
			string luaCode = GetSArgument(0);
			if (!string.IsNullOrEmpty(luaCode)) {
				LuaTable selfTable = (obj as LuaBehaviour)?.LuaTable;
				LuaEnv luaEnv = LuaMain.Instance.LuaEnv;
				LuaTable metaTable = luaEnv.NewTable();
				metaTable.Set("__index", luaEnv.Global);
				LuaTable envTable = InjectionConverter.ToDictTable(luaInjectionList);
				envTable.SetMetaTable(metaTable);
				envTable.Set("self", selfTable);
				luaEnv.DoString(luaCode, "LuaCodeExecute", envTable);
			}
		}
		
		private void DoStepLuaSetValue() {
			LuaTable table = (obj as LuaBehaviour)?.LuaTable ?? LuaMain.Instance.LuaEnv.Global;
			string fieldName = sArguments[0];
			string fieldPath = string.IsNullOrEmpty(fieldName) ? sArguments[1] : fieldName;
			object value = InjectionConverter.ToLuaValue(luaInjectionList[0]);
			table.SetInPath(fieldPath, value);
		}
		
		private void DoStepLuaFunctionInvoke() {
			LuaTable table = (obj as LuaBehaviour)?.LuaTable ?? LuaMain.Instance.LuaEnv.Global;
			string funcPath = string.IsNullOrEmpty(sArguments[0]) ? sArguments[1] : sArguments[0];
			bool isInstanceInvoke = bArguments[0];
			int pointIndex = funcPath.LastIndexOf('.');
			if (isInstanceInvoke) {
				LuaTable caller;
				string funcName;
				if (pointIndex == -1) {
					caller = table;
					funcName = funcPath;
				} else {
					string callerPath = funcPath.Substring(0, pointIndex);
					caller = table.GetInPath<LuaTable>(callerPath);
					funcName = funcPath.Substring(pointIndex + 1);
				}
				int parameterCount = luaInjectionList.Count;
				object[] parameters = new object[parameterCount + 1];
				parameters[0] = caller;
				for (int i = 0; i < parameterCount; i++) {
					parameters[i + 1] = InjectionConverter.ToLuaValue(luaInjectionList[i]);
				}
				LuaMain.Instance.FuncInvoke<object>(caller, funcName, parameters);
			} else {
				object[] parameters = luaInjectionList.ConvertAll(InjectionConverter.ToLuaValue).ToArray();
				LuaMain.Instance.FuncInvoke<object>(table, funcPath, parameters);
			}
		}
	}
}

#endif
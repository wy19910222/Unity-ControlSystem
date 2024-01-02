using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Control {
	public class InitializeOnLoad : MonoBehaviour {
		[InitializeOnLoadMethod]
		private static void InitializeOnLoadMethod() {
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			Dictionary<string, bool> symbolExistDict = new Dictionary<string, bool> {
				["CINEMACHINE_EXIST"] = false,
				["SPINE_EXIST"] = false,
				["LUA_BEHAVIOUR_EXIST"] = false,
			};
			foreach (Assembly assembly in assemblies) {
				switch (assembly.GetName().Name) {
					case "Cinemachine":
						symbolExistDict["CINEMACHINE_EXIST"] = true;
						break;
					case "spine-unity":
						symbolExistDict["SPINE_EXIST"] = true;
						break;
					case "Assembly-CSharp": {
						foreach (var exportedType in assembly.ExportedTypes) {
							if (exportedType.FullName == "LuaApp.LuaBehaviour") {
								symbolExistDict["LUA_BEHAVIOUR_EXIST"] = true;
								break;
							}
						}
						break;
					}
				}
			}
			UpdateScriptingDefineSymbolsForGroup(BuildTargetGroup, symbolExistDict);
		}
		
		private static void UpdateScriptingDefineSymbolsForGroup(BuildTargetGroup group, Dictionary<string, bool> symbolExistDict) {
			List<string> symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(';').ToList();
			bool dirty = false;
			foreach (var (symbol, isExist) in symbolExistDict) {
				if (!isExist && symbols.Contains(symbol)) {
					symbols.Remove(symbol);
					dirty = true;
				}
			}
			foreach (var (symbol, isExist) in symbolExistDict) {
				if (isExist && !symbols.Contains(symbol)) {
					symbols.Add(symbol);
					dirty = true;
				}
			}
			if (dirty) {
				PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(";", symbols));
			}
		}

		private static void AddScriptingDefineSymbolsForGroup(BuildTargetGroup group, string symbol) {
			List<string> symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(';').ToList();
			if (!symbols.Contains(symbol)) {
				symbols.Add(symbol);
				PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(";", symbols));
			}
		}
		private static void RemoveScriptingDefineSymbolsForGroup(BuildTargetGroup group, string symbol) {
			List<string> symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(';').ToList();
			if (symbols.Contains(symbol)) {
				symbols.Remove(symbol);
				PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(";", symbols));
			}
		}
		
		private static BuildTargetGroup BuildTargetGroup =>
#if UNITY_ANDROID
				BuildTargetGroup.Android;
#elif UNITY_IOS
				BuildTargetGroup.iOS;
#elif UNITY_STANDALONE
				BuildTargetGroup.Standalone;
#elif UNITY_WEBGL
				BuildTargetGroup.WebGL;
#endif
	}
}

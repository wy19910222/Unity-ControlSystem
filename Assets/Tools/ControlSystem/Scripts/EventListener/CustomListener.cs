/*
 * @Author: wangyun
 * @CreateTime: 2022-07-31 13:17:23 043
 * @LastEditor: wangyun
 * @EditTime: 2022-09-09 11:11:31 857
 */

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Control {
	public class CustomListener : BaseListener {
		public string eventName;

		private void Awake() {
			On(eventName, this);
		}

		private void OnDestroy() {
			OffAll(this);
		}

		protected override void Execute() {
			if (enabled) {
				base.Execute();
			}
		}

		[UsedImplicitly]
		private void CustomEvent(string evtName) {
			if (evtName == eventName) {
				Execute();
			}
		}

		private static readonly Dictionary<string, List<CustomListener>> s_EventListenersDict = new Dictionary<string, List<CustomListener>>();

		public static void Emit(string eventName) {
			if (string.IsNullOrEmpty(eventName)) {
				Debug.LogError("Event name is null!");
				return;
			}

			if (s_EventListenersDict.TryGetValue(eventName, out List<CustomListener> list)) {
				List<CustomListener> listWillExecute = new List<CustomListener>(list);
				foreach (var listener in listWillExecute) {
					try {
						listener.Execute();
					} catch (Exception e) {
						Debug.LogError(e);
					}
				}
			}
		}

		private static void On(string eventName, CustomListener listener) {
			if (Contains(eventName, listener)) {
				Debug.LogError("Listener is already exist!\t" + eventName);
				return;
			}
			
			if (!s_EventListenersDict.TryGetValue(eventName, out List<CustomListener> list)) {
				list = new List<CustomListener>();
				s_EventListenersDict.Add(eventName, list);
			}
			list.Add(listener);
		}

		private static void OffAll(CustomListener listener) {
			List<string> listWillRemove = new List<string>();
			foreach (var pair in s_EventListenersDict) {
				List<CustomListener> list = pair.Value;
				for (int i = list.Count - 1; i >= 0; --i) {
					if (list[i] == listener) {
						list.RemoveAt(i);
					}
				}
				if (list.Count <= 0) {
					listWillRemove.Add(pair.Key);
				}
			}
			foreach (var eventName in listWillRemove) {
				s_EventListenersDict.Remove(eventName);
			}
		}

		private static bool Contains(string eventName, CustomListener listener) {
			if (string.IsNullOrEmpty(eventName)) {
				Debug.LogError("Event name is null!");
				return false;
			}
			
			if (s_EventListenersDict.TryGetValue(eventName, out List<CustomListener> list)) {
				foreach (var _listener in list) {
					if (_listener == listener) {
						return true;
					}
				}
			}
			
			return false;
		}
	}
}
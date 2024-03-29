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
	public class CustomEventListener : BaseEventListener {
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

		private static readonly Dictionary<string, List<CustomEventListener>> s_EventListenersDict = new Dictionary<string, List<CustomEventListener>>();

		public static void Emit(string eventName) {
			if (string.IsNullOrEmpty(eventName)) {
				Debug.LogError("Event name is null!");
				return;
			}
			if (s_EventListenersDict.TryGetValue(eventName, out List<CustomEventListener> list)) {
				List<CustomEventListener> listWillExecute = new List<CustomEventListener>(list);
				foreach (var listener in listWillExecute) {
					try {
						listener.Execute();
					} catch (Exception e) {
						Debug.LogError(e);
					}
				}
			}
		}

		private static void On(string eventName, CustomEventListener eventListener) {
			if (Contains(eventName, eventListener)) {
				Debug.LogError("Listener is already exist!\t" + eventName);
				return;
			}
			if (!s_EventListenersDict.TryGetValue(eventName, out List<CustomEventListener> list)) {
				list = new List<CustomEventListener>();
				s_EventListenersDict.Add(eventName, list);
			}
			list.Add(eventListener);
		}

		private static void OffAll(CustomEventListener eventListener) {
			List<string> listWillRemove = new List<string>();
			foreach (var pair in s_EventListenersDict) {
				List<CustomEventListener> list = pair.Value;
				for (int i = list.Count - 1; i >= 0; --i) {
					if (list[i] == eventListener) {
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

		private static bool Contains(string eventName, CustomEventListener eventListener) {
			if (string.IsNullOrEmpty(eventName)) {
				Debug.LogError("Event name is null!");
				return false;
			}
			if (s_EventListenersDict.TryGetValue(eventName, out List<CustomEventListener> list)) {
				foreach (var _listener in list) {
					if (_listener == eventListener) {
						return true;
					}
				}
			}
			return false;
		}
	}
}
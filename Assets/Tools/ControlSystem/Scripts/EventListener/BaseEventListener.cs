/*
 * @Author: wangyun
 * @CreateTime: 2022-05-28 22:37:12 250
 * @LastEditor: wangyun
 * @EditTime: 2022-05-28 22:37:12 254
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Control {
	public class BaseEventListener : MonoBehaviour {
		public string title;
		[ComponentSelect]
		public List<BaseTrigger> triggers = new List<BaseTrigger>();
		
		private Action m_OnEvent;

		protected virtual void Trigger() {
			foreach (var trigger in triggers) {
				trigger.Trigger();
			}
			m_OnEvent?.Invoke();
		}
		
		public void OnEvent(Action action) {
			m_OnEvent += action;
		}
		
		public void OffTrigger(Action action) {
			m_OnTrigger -= action;
		}
	}
}
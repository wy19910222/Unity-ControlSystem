/*
 * @Author: wangyun
 * @CreateTime: 2022-07-31 13:23:00 655
 * @LastEditor: wangyun
 * @EditTime: 2022-09-09 11:33:23 459
 */

using UnityEngine;
using Sirenix.OdinInspector;

namespace Control {
	public class TriggerCtrlCustomEvent : TriggerCtrlTrigger {
		public string eventName;
		public bool broadcast = true;
		[HideIf("@broadcast")]
		public GameObject target;
		
		protected override void DoTrigger() {
			if (broadcast) {
				CustomEventListener.Emit(eventName);
			} else {
				if (target) {
					target.SendMessage("CustomTrigger", eventName);
				}
			}
		}
	}
}
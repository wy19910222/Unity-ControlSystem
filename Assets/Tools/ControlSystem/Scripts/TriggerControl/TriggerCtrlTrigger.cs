/*
 * @Author: wangyun
 * @CreateTime: 2022-04-20 01:47:15 263
 * @LastEditor: wangyun
 * @EditTime: 2022-04-20 01:47:15 267
 */

using System.Collections.Generic;
using UnityEngine.Serialization;
using Sirenix.OdinInspector;

namespace Control {
	public class TriggerCtrlTrigger : BaseTriggerCtrl {
		[ComponentSelect(true), PropertySpace(10, 0)]
		public List<BaseTriggerCtrl> earlierTriggers = new List<BaseTriggerCtrl>();
		[ComponentSelect(true), PropertySpace(0, 10)]
		public List<BaseTriggerCtrl> laterTriggers = new List<BaseTriggerCtrl>();

		protected override void BeforeTrigger() {
			foreach (var trigger in earlierTriggers) {
				trigger.Trigger();
			}
		}

		protected override void DoTrigger() { }

		protected override void AfterTrigger() {
			foreach (var trigger in laterTriggers) {
				trigger.Trigger();
			}
		}
	}
}
/*
 * @Author: wangyun
 * @CreateTime: 2022-04-20 15:16:02 345
 * @LastEditor: wangyun
 * @EditTime: 2022-04-20 15:16:02 342
 */

using UnityEngine.Events;

namespace Control {
	public class TriggerCtrlEventTrigger : TriggerCtrlTrigger {
		public UnityEvent eventTrigger = new UnityEvent();

		protected override void DoTrigger() {
			eventTrigger.Invoke();
		}
	}
}
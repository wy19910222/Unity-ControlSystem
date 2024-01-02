/*
 * @Author: wangyun
 * @CreateTime: 2022-05-02 19:01:32 063
 * @LastEditor: wangyun
 * @EditTime: 2022-05-02 19:01:32 068
 */

using UnityEngine;

namespace Control {
	public class TriggerCtrlParent : TriggerCtrlTrigger {
		public Transform parent;
		
		protected override void DoTrigger() {
			transform.SetParent(parent);
		}
	}
}
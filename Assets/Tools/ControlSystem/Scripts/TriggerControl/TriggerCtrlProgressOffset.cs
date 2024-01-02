/*
 * @Author: wangyun
 * @CreateTime: 2022-11-18 14:54:59 254
 * @LastEditor: wangyun
 * @EditTime: 2022-11-18 14:54:59 257
 */

using UnityEngine;

namespace Control {
	public class TriggerCtrlProgressOffset : TriggerCtrlTrigger {
		[ComponentSelect]
		public ProgressController controller;
		[Range(0, 1)]
		public float offset;

		protected override void DoTrigger() {
			if (controller) {
				controller.Progress += offset;
			}
		}
	}
}

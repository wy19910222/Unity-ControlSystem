/*
 * @Author: wangyun
 * @CreateTime: 2022-11-18 14:52:59 467
 * @LastEditor: wangyun
 * @EditTime: 2022-11-18 14:52:59 470
 */

using System;
using UnityEngine;

namespace Control {
	[Obsolete("TriggerCtrlStateOffset has been deprecated. Use TriggerCtrlProcess instead")]
	[AddComponentMenu("")]
	public class TriggerCtrlStateOffset : TriggerCtrlTrigger {
		[ComponentSelect]
		public StateController controller;
		public int offset = 1;

		protected override void DoTrigger() {
			if (controller) {
				controller.Index += offset;
			}
		}
	}
}

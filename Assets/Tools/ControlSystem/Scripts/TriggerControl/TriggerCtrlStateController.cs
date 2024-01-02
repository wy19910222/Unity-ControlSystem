/*
 * @Author: wangyun
 * @CreateTime: 2022-04-20 20:37:18 063
 * @LastEditor: wangyun
 * @EditTime: 2022-08-26 00:59:07 136
 */

using System;
using System.Collections.Generic;
using UnityEngine;

using URandom = UnityEngine.Random;

namespace Control {
	[Obsolete("TriggerCtrlStateController has been deprecated. Use TriggerCtrlProcess instead")]
	[AddComponentMenu("")]
	public partial class TriggerCtrlStateController : TriggerCtrlTrigger {
		[ComponentSelect]
		public StateController controller;
		public bool random;
		[HideInInspector]
		public long indexMask = -1;
		[HideInInspector]
		public int index;

		protected override void DoTrigger() {
			if (controller) {
				if (random) {
					List<int> indexes = new List<int>();
					for (int i = 0, length = controller.StateCount; i < length; i++) {
						if ((indexMask & (long) 1 << i) != 0) {
							indexes.Add(i);
						}
					}
					controller.Index = indexes[URandom.Range(0, indexes.Count)];
				} else {
					controller.Index = index;
				}
			}
		}
	}
}

/*
 * @Author: wangyun
 * @CreateTime: 2022-08-31 02:02:59 164
 * @LastEditor: wangyun
 * @EditTime: 2022-08-31 02:02:59 173
 */

using UnityEngine;

namespace Control {
	[RequireComponent(typeof(Animator))]
	public class TriggerCtrlAnimatorController : TriggerCtrlTrigger {
		[ComponentSelect]
		public Animator animator;
		public RuntimeAnimatorController controller;

		private void Reset() {
			animator = GetComponentInChildren<Animator>();
		}
		
		protected override void DoTrigger() {
			animator.runtimeAnimatorController = controller;
		}
	}
}
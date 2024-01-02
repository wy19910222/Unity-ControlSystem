/*
 * @Author: wangyun
 * @CreateTime: 2022-08-31 01:55:37 663
 * @LastEditor: wangyun
 * @EditTime: 2022-08-31 01:55:37 667
 */

using UnityEngine;

namespace Control {
	public class TriggerCtrlAnimatorApplyRootMotion : TriggerCtrlTrigger {
		[ComponentSelect]
		public Animator animator;
		public bool applyRootMotion;

		private void Reset() {
			animator = GetComponentInChildren<Animator>();
		}
		
		protected override void DoTrigger() {
			animator.applyRootMotion = applyRootMotion;
		}
	}
}
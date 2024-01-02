/*
 * @Author: wangyun
 * @CreateTime: 2022-04-20 01:47:15 263
 * @LastEditor: wangyun
 * @EditTime: 2022-04-20 01:47:15 267
 */

using UnityEngine;

namespace Control {
	public class TriggerCtrlAnimatorTrigger : TriggerCtrlTrigger {
		[ComponentSelect]
		public Animator animator;
		[AnimatorParamSelect("animator", AnimatorControllerParameterType.Trigger)]
		public string paramName;

		private void Reset() {
			animator = GetComponentInChildren<Animator>();
		}
		
		protected override void DoTrigger() {
			animator.SetTrigger(paramName);
		}
	}
}
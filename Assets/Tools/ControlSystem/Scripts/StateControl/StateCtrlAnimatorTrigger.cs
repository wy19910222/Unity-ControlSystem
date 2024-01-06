/*
 * @Author: wangyun
 * @CreateTime: 2022-04-17 12:57:12 137
 * @LastEditor: wangyun
 * @EditTime: 2022-09-16 14:15:51 806
 */

using UnityEngine;

namespace Control {
	[RequireComponent(typeof(Animator))]
	public class StateCtrlAnimatorTrigger : BaseStateCtrl<bool> {
		[SelfAnimatorParamSelect(AnimatorControllerParameterType.Trigger)]
		public string paramName;
		public bool falseIsReset;
		public bool trigger;
		
		protected override bool TargetValue {
			get => trigger;
			set {
				trigger = value;
				if (value) {
					GetComponent<Animator>().SetTrigger(paramName);
				} else if (falseIsReset) {
					GetComponent<Animator>().ResetTrigger(paramName);
				}
			}
		}
	}
}
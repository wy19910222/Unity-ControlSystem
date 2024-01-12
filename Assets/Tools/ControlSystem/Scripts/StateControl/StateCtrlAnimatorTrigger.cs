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
		public bool paramValue;
		public bool falseIsReset;
		
		protected override bool TargetValue {
			get => paramValue;
			set {
				paramValue = value;
				if (value) {
					GetComponent<Animator>().SetTrigger(paramName);
				} else if (falseIsReset) {
					GetComponent<Animator>().ResetTrigger(paramName);
				}
			}
		}
	}
}
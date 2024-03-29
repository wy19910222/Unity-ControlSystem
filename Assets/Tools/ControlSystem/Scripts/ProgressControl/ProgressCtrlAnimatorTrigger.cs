/*
 * @Author: wangyun
 * @CreateTime: 2022-04-18 21:31:07 371
 * @LastEditor: wangyun
 * @EditTime: 2022-04-18 21:31:07 366
 */

using UnityEngine;

namespace Control {
	[RequireComponent(typeof(Animator))]
	public class ProgressCtrlAnimatorTrigger : BaseProgressCtrlConst<bool> {
		[SelfAnimatorParamSelect(AnimatorControllerParameterType.Trigger)]
		public string paramName;
		public bool paramValue;
		public bool falseIsReset;
		
		protected override bool TargetValue {
			get => paramValue;
			set {
				if (value != paramValue) {
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
}
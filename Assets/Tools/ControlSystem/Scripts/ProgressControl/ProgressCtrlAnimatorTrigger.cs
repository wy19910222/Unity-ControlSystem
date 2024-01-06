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
		public bool falseIsReset;
		public bool trigger;
		protected override bool TargetValue {
			get => trigger;
			set {
				if (value != trigger) {
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
}
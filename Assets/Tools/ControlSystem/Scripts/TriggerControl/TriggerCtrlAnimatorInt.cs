/*
 * @Author: wangyun
 * @CreateTime: 2022-04-20 01:47:15 263
 * @LastEditor: wangyun
 * @EditTime: 2022-04-20 01:47:15 267
 */

using UnityEngine;
using Sirenix.OdinInspector;

namespace Control {
	public class TriggerCtrlAnimatorInt : TriggerCtrlTrigger {
		[ComponentSelect]
		public Animator animator;
		[AnimatorParamSelect("animator", AnimatorControllerParameterType.Int)]
		public string paramName;
		public int paramValue;
		
		[Space(10)]
		public bool setTriggerOnChange;
		[ShowIf("@setTriggerOnChange"), Indent, LabelText("Param Name")]
		[AnimatorParamSelect("animator", AnimatorControllerParameterType.Trigger)]
		public string triggerParamName;
		[ShowIf("@setTriggerOnChange"), Indent]
		public bool zeroIsReset = true;

		private void Reset() {
			animator = GetComponentInChildren<Animator>();
		}
		
		protected override void DoTrigger() {
			if (setTriggerOnChange) {
				int value = animator.GetInteger(paramName);
				if (paramValue != value) {
					animator.SetInteger(paramName, paramValue);
					if (paramValue == 0 && zeroIsReset) {
						animator.ResetTrigger(triggerParamName);
					} else {
						animator.SetTrigger(triggerParamName);
					}
				}
			} else {
				animator.SetInteger(paramName, paramValue);
			}
		}
	}
}
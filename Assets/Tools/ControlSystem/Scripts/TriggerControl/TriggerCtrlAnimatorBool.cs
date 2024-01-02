/*
 * @Author: wangyun
 * @CreateTime: 2022-04-20 01:47:15 263
 * @LastEditor: wangyun
 * @EditTime: 2022-06-10 10:35:05 224
 */

using UnityEngine;
using Sirenix.OdinInspector;

namespace Control {
	public class TriggerCtrlAnimatorBool : TriggerCtrlTrigger {
		[ComponentSelect]
		public Animator animator;
		[AnimatorParamSelect("animator", AnimatorControllerParameterType.Bool)]
		public string paramName;
		public bool paramValue;
		
		[Space(10)]
		public bool setTriggerOnChange;
		[ShowIf("@setTriggerOnChange"), Indent, LabelText("Param Name")]
		[AnimatorParamSelect("animator", AnimatorControllerParameterType.Trigger)]
		public string triggerParamName;
		[ShowIf("@setTriggerOnChange"), Indent]
		public bool falseIsReset = true;

		private void Reset() {
			animator = GetComponentInChildren<Animator>();
		}
		
		protected override void DoTrigger() {
			if (setTriggerOnChange) {
				bool value = animator.GetBool(paramName);
				if (paramValue != value) {
					animator.SetBool(paramName, paramValue);
					if (!paramValue && falseIsReset) {
						animator.ResetTrigger(triggerParamName);
					} else {
						animator.SetTrigger(triggerParamName);
					}
				}
			} else {
				animator.SetBool(paramName, paramValue);
			}
		}
	}
}
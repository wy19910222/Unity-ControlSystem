/*
 * @Author: wangyun
 * @CreateTime: 2022-04-20 01:47:15 263
 * @LastEditor: wangyun
 * @EditTime: 2022-06-10 10:35:46 412
 */

using UnityEngine;
using Sirenix.OdinInspector;

namespace Control {
	public class TriggerCtrlAnimatorFloat : TriggerCtrlTrigger {
		[ComponentSelect]
		public Animator animator;
		[AnimatorParamSelect("animator", AnimatorControllerParameterType.Float)]
		public string paramName;
		public float paramValue;
		
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
				float value = animator.GetFloat(paramName);
				if (Mathf.Abs(paramValue - value) > Mathf.Epsilon) {
					animator.SetFloat(paramName, paramValue);
					if (paramValue == 0 && zeroIsReset) {
						animator.ResetTrigger(triggerParamName);
					} else {
						animator.SetTrigger(triggerParamName);
					}
				}
			} else {
				animator.SetFloat(paramName, paramValue);
			}
		}
	}
}
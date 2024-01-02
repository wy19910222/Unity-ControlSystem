/*
 * @Author: wangyun
 * @CreateTime: 2022-04-18 21:32:01 149
 * @LastEditor: wangyun
 * @EditTime: 2022-06-10 11:01:49 550
 */

using UnityEngine;
using Sirenix.OdinInspector;

namespace Control {
	[RequireComponent(typeof(Animator))]
	public class ProgressCtrlAnimatorInt : BaseProgressCtrlInt {
		[SelfAnimatorParamSelect(AnimatorControllerParameterType.Int)]
		public string paramName;
		public int paramValue;
		
		[Space(10)]
		public bool setTriggerOnChange;
		[ShowIf("@setTriggerOnChange"), Indent, LabelText("Param Name")]
		[SelfAnimatorParamSelect(AnimatorControllerParameterType.Trigger)]
		public string triggerParamName;
		[ShowIf("@setTriggerOnChange"), Indent]
		public bool zeroIsReset = true;
		
		protected override int TargetValue {
			get {
#if UNITY_EDITOR
				if (Application.isPlaying)
#endif
				{
					paramValue = GetComponent<Animator>().GetInteger(paramName);
				}
				return paramValue;
			}
			set {
				paramValue = value;
				Animator animator = GetComponent<Animator>();
				if (setTriggerOnChange) {
					int curValue = animator.GetInteger(paramName);
					if (paramValue != curValue) {
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
}
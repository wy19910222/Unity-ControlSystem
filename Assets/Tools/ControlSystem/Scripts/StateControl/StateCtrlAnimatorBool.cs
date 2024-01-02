/*
 * @Author: wangyun
 * @CreateTime: 2022-04-17 09:54:56 864
 * @LastEditor: wangyun
 * @EditTime: 2022-09-16 14:15:43 338
 */

using UnityEngine;
using Sirenix.OdinInspector;

namespace Control {
	[RequireComponent(typeof(Animator))]
	public class StateCtrlAnimatorBool : BaseStateCtrl<bool> {
		[SelfAnimatorParamSelect(AnimatorControllerParameterType.Bool)]
		public string paramName;
		public bool paramValue;
		
		[Space(10)]
		public bool setTriggerOnChange;
		[ShowIf("@setTriggerOnChange"), Indent, LabelText("Param Name")]
		[SelfAnimatorParamSelect(AnimatorControllerParameterType.Trigger)]
		public string triggerParamName;
		[ShowIf("@setTriggerOnChange"), Indent]
		public bool falseIsReset = true;
		
		protected override bool TargetValue {
			get {
#if UNITY_EDITOR
				if (Application.isPlaying)
#endif
				{
					paramValue = GetComponent<Animator>().GetBool(paramName);
				}
				return paramValue;
			}
			set {
				paramValue = value;
				Animator animator = GetComponent<Animator>();
				if (setTriggerOnChange) {
					bool curValue = animator.GetBool(paramName);
					if (paramValue != curValue) {
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
}
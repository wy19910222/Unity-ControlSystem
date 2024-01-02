/*
 * @Author: wangyun
 * @CreateTime: 2022-04-17 12:15:01 799
 * @LastEditor: wangyun
 * @EditTime: 2022-10-17 18:26:52 698
 */

using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	[RequireComponent(typeof(Animator))]
	public class StateCtrlAnimatorFloat : BaseStateCtrl<float> {
		[SelfAnimatorParamSelect(AnimatorControllerParameterType.Float)]
		public string paramName;
		public float paramValue;
		
		public bool tween;
		[HideIf("@!this.tween")]
		public float tweenDelay;
		[HideIf("@!this.tween")]
		public float tweenDuration = 0.3F;
		[HideIf("@!this.tween")]
		public Ease tweenEase = Ease.OutQuad;
		[HideIf("@!tween || tweenEase != Ease.INTERNAL_Custom")]
		public AnimationCurve tweenEaseCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
		
		[Space(10)]
		public bool setTriggerOnChange;
		[ShowIf("@setTriggerOnChange"), Indent, LabelText("Param Name")]
		[SelfAnimatorParamSelect(AnimatorControllerParameterType.Trigger)]
		public string triggerParamName;
		[ShowIf("@setTriggerOnChange"), Indent]
		public bool zeroIsReset = true;

		private Tweener m_Tweener;
		
		protected override float TargetValue {
			get {
#if UNITY_EDITOR
				if (Application.isPlaying)
#endif
				{
					paramValue = GetComponent<Animator>().GetFloat(paramName);
				}
				return paramValue;
			}
			set {
				paramValue = value;
				
				if (m_Tweener != null) {
					m_Tweener.Kill();
					m_Tweener = null;
				}
				Animator animator = GetComponent<Animator>();
#if UNITY_EDITOR
				if (tween && !controller.InvalidateTween && Application.isPlaying) {
#else
				if (tween && !controller.InvalidateTween) {
					
#endif
					float oldValue = animator.GetFloat(paramName);
					float t = 0;
					m_Tweener = DOTween.To(
						() => t,
						v => {
							t = v;
							float newValue = Mathf.Lerp(oldValue, paramValue, t);
							if (setTriggerOnChange) {
								float curValue = animator.GetFloat(paramName);
								if (Mathf.Abs(newValue - curValue) > Mathf.Epsilon) {
									animator.SetFloat(paramName, newValue);
									if (newValue == 0 && zeroIsReset) {
										animator.ResetTrigger(triggerParamName);
									} else {
										animator.SetTrigger(triggerParamName);
									}
								}
							} else {
								animator.SetFloat(paramName, newValue);
							}
						},
						1,
						tweenDuration
					);
					if (tweenEase == Ease.INTERNAL_Custom) {
						m_Tweener.SetEase(tweenEaseCurve);
					} else {
						m_Tweener.SetEase(tweenEase);
					}
					m_Tweener.SetDelay(tweenDelay).OnComplete(() => m_Tweener = null);
				} else {
					if (setTriggerOnChange) {
						float curValue = animator.GetFloat(paramName);
						if (Mathf.Abs(paramValue - curValue) > Mathf.Epsilon) {
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
	}
}
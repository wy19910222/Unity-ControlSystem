/*
 * @Author: wangyun
 * @CreateTime: 2022-12-15 13:04:27 717
 * @LastEditor: wangyun
 * @EditTime: 2022-12-15 13:04:27 721
 */

using UnityEngine;
using DG.Tweening;

namespace Control {
	public partial class BaseProcessStep {
		private Tweener m_AnimatorTweener;
		private void DoStepAnimatorParameters() {
			if (obj is Animator animator) {
				string paramName = GetSArgument(0);
				int parameterCount = animator.parameterCount;
				for (int i = 0; i < parameterCount; ++i) {
					AnimatorControllerParameter parameter = animator.GetParameter(i);
					if (parameter.name == paramName) {
						switch (parameter.type) {
							case AnimatorControllerParameterType.Float: {
								float fValue = GetFArgument(0);
								bool isRelative = GetBArgument(0);
								if (isRelative) {
									fValue += animator.GetFloat(paramName);
								}
								if (m_AnimatorTweener != null) {
									m_AnimatorTweener.Kill();
									m_AnimatorTweener = null;
								}
#if UNITY_EDITOR
								if (tween && Application.isPlaying) {
#else
								if (tween) {
#endif
									m_AnimatorTweener = DOTween.To(
											() => animator.GetFloat(paramName),
											v => animator.SetFloat(paramName, v),
											fValue,
											tweenDuration
									);
									if (tweenEase == Ease.INTERNAL_Custom) {
										m_AnimatorTweener.SetEase(tweenEaseCurve);
									} else {
										m_AnimatorTweener.SetEase(tweenEase);
									}
									m_AnimatorTweener.SetDelay(tweenDelay).OnComplete(() => m_AnimatorTweener = null);
								} else {
									animator.SetFloat(paramName, fValue);
								}
								break;
							}
							case AnimatorControllerParameterType.Int: {
								int iValue = GetIArgument(0);
								bool isRelative = GetBArgument(0);
								if (isRelative) {
									iValue += animator.GetInteger(paramName);
								}
								if (m_AnimatorTweener != null) {
									m_AnimatorTweener.Kill();
									m_AnimatorTweener = null;
								}
#if UNITY_EDITOR
								if (tween && Application.isPlaying) {
#else
								if (tween) {
#endif
									int toIntType = GetIArgument(1);
									m_AnimatorTweener = DOTween.To(
										() => animator.GetInteger(paramName),
										v => {
											v = toIntType < 0 ? Mathf.FloorToInt(v) : toIntType > 0 ? Mathf.CeilToInt(v) : Mathf.RoundToInt(v);
											animator.SetInteger(paramName, v);
										},
										iValue,
										tweenDuration
									);
									if (tweenEase == Ease.INTERNAL_Custom) {
										m_AnimatorTweener.SetEase(tweenEaseCurve);
									} else {
										m_AnimatorTweener.SetEase(tweenEase);
									}
									m_AnimatorTweener.SetDelay(tweenDelay).OnComplete(() => m_AnimatorTweener = null);
								} else {
									animator.SetInteger(paramName, iValue);
								}
								break;
							}
							case AnimatorControllerParameterType.Bool: {
								bool bValue = GetBArgument(0);
								animator.SetBool(paramName, bValue);
								break;
							}
							case AnimatorControllerParameterType.Trigger: {
								bool isSet = GetBArgument(0);
								if (isSet) {
									animator.SetTrigger(paramName);
								} else {
									animator.ResetTrigger(paramName);
								}
								break;
							}
						}
						break;
					}
				}
			}
		}
		private void DoStepAnimatorController() {
			if (obj is Animator animator) {
				RuntimeAnimatorController controller = GetObjArgument<RuntimeAnimatorController>(0);
				animator.runtimeAnimatorController = controller;
			}
		}
		private void DoStepAnimatorAvatar() {
			if (obj is Animator animator) {
				Avatar avatar = GetObjArgument<Avatar>(0);
				animator.avatar = avatar;
			}
		}
		private void DoStepAnimatorApplyRootMotion() {
			if (obj is Animator animator) {
				bool applyRootMotion = GetBArgument(0);
				animator.applyRootMotion = applyRootMotion;
			}
		}
	}
}
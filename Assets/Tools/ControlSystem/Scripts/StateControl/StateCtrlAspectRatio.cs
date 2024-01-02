/*
 * @Author: wangyun
 * @CreateTime: 2023-02-16 19:33:31 864
 * @LastEditor: wangyun
 * @EditTime: 2023-07-09 17:21:22 600
 */

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	[RequireComponent(typeof(AspectRatioFitter))]
	public class StateCtrlAspectRatio : BaseStateCtrl<float> {
		public bool tween;
		[HideIf("@!this.tween")]
		public float tweenDelay;
		[HideIf("@!this.tween")]
		public float tweenDuration = 0.3F;
		[HideIf("@!this.tween")]
		public Ease tweenEase = Ease.OutQuad;
		[HideIf("@!tween || tweenEase != Ease.INTERNAL_Custom")]
		public AnimationCurve tweenEaseCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
		[HideIf("@!this.tween")]
		public bool easeBasedSize = true;

		private Tweener m_Tweener;
		
		protected override float TargetValue {
			get => GetComponent<AspectRatioFitter>().aspectRatio;
			set {
				if (m_Tweener != null) {
					m_Tweener.Kill();
					m_Tweener = null;
				}
#if UNITY_EDITOR
				if (tween && !controller.InvalidateTween && Application.isPlaying) {
#else
				if (tween && !controller.InvalidateTween) {
#endif
					AspectRatioFitter fitter = GetComponent<AspectRatioFitter>();
					if (easeBasedSize) {
						m_Tweener = AspectRatioUtils.CreateTweenerBasedSize(fitter, value, tweenDuration);
					} else {
						m_Tweener = DOTween.To(
								() => fitter.aspectRatio,
								v => fitter.aspectRatio = v,
								value,
								tweenDuration
						);
					}
					if (tweenEase == Ease.INTERNAL_Custom) {
						m_Tweener.SetEase(tweenEaseCurve);
					} else {
						m_Tweener.SetEase(tweenEase);
					}
					m_Tweener.SetDelay(tweenDelay).OnComplete(() => m_Tweener = null);
				} else {
					GetComponent<AspectRatioFitter>().aspectRatio = value;
				}
			}
		}
	}
}
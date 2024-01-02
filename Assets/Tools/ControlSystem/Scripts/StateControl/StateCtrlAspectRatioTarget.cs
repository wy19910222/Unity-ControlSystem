/*
 * @Author: wangyun
 * @CreateTime: 2023-07-09 01:49:08 125
 * @LastEditor: wangyun
 * @EditTime: 2023-07-09 01:49:08 131
 */

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	[RequireComponent(typeof(AspectRatioFitter))]
	public class StateCtrlAspectRatioTarget : BaseStateCtrl<RectTransform> {
		public RectTransform target;
		
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

		protected override RectTransform TargetValue {
			get => target;
			set {
				target = value;
				if (m_Tweener != null) {
					m_Tweener.Kill();
					m_Tweener = null;
				}
#if UNITY_EDITOR
				if (tween && !controller.InvalidateTween && Application.isPlaying) {
#else
				if (tween && !controller.InvalidateTween) {
#endif
					Rect rect = value.rect;
					float aspectRatio = rect.width / rect.height;
					AspectRatioFitter fitter = GetComponent<AspectRatioFitter>();
					if (easeBasedSize) {
						m_Tweener = AspectRatioUtils.CreateTweenerBasedSize(fitter, aspectRatio, tweenDuration);
					} else {
						m_Tweener = DOTween.To(
								() => fitter.aspectRatio,
								v => fitter.aspectRatio = v,
								aspectRatio,
								tweenDuration
						);
					}
					if (m_Tweener != null) {
						if (tweenEase == Ease.INTERNAL_Custom) {
							m_Tweener.SetEase(tweenEaseCurve);
						} else {
							m_Tweener.SetEase(tweenEase);
						}
						m_Tweener.SetDelay(tweenDelay).OnComplete(() => m_Tweener = null);
					}
				} else {
					Rect rect = value.rect;
					float aspectRatio = rect.width / rect.height;
					GetComponent<AspectRatioFitter>().aspectRatio = aspectRatio;
				}
			}
		}
	}
}
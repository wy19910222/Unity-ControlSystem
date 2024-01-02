/*
 * @Author: wangyun
 * @CreateTime: 2023-07-09 18:10:50 590
 * @LastEditor: wangyun
 * @EditTime: 2023-07-09 18:10:50 596
 */

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	[RequireComponent(typeof(AspectRatioFitter))]
	public class ProgressCtrlAspectRatioTarget : BaseProgressCtrlFloat {
		public RectTransform fromTarget;
		public RectTransform toTarget;
		public float m_LerpValue;
		public bool basedSize = true;
		
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
			get => m_LerpValue;
			set {
				m_LerpValue = value;
				if (m_Tweener != null) {
					m_Tweener.Kill();
					m_Tweener = null;
				}
#if UNITY_EDITOR
				if (tween && !controller.InvalidateTween && Application.isPlaying) {
#else
				if (tween && !controller.InvalidateTween) {
#endif
					if (fromTarget && toTarget) {
						Rect fromRect = fromTarget.rect;
						float fromAspectRatio = fromRect.width / fromRect.height;
						Rect toRect = toTarget.rect;
						float toAspectRatio = toRect.width / toRect.height;
						AspectRatioFitter fitter = GetComponent<AspectRatioFitter>();
						float aspectRatio = basedSize ?
								AspectRatioUtils.LerpUnclampedBasedSize(fromAspectRatio, toAspectRatio, m_LerpValue, fitter) :
								Mathf.LerpUnclamped(fromAspectRatio, toAspectRatio, m_LerpValue);
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
						if (tweenEase == Ease.INTERNAL_Custom) {
							m_Tweener.SetEase(tweenEaseCurve);
						} else {
							m_Tweener.SetEase(tweenEase);
						}
						m_Tweener.SetDelay(tweenDelay).OnComplete(() => m_Tweener = null);
					}
				} else {
					if (fromTarget && toTarget) {
						Rect fromRect = fromTarget.rect;
						float fromAspectRatio = fromRect.width / fromRect.height;
						Rect toRect = toTarget.rect;
						float toAspectRatio = toRect.width / toRect.height;
						AspectRatioFitter fitter = GetComponent<AspectRatioFitter>();
						fitter.aspectRatio = basedSize ?
								AspectRatioUtils.LerpUnclampedBasedSize(fromAspectRatio, toAspectRatio, m_LerpValue, fitter) :
								Mathf.LerpUnclamped(fromAspectRatio, toAspectRatio, m_LerpValue);
					}
				}
			}
		}
	}
}
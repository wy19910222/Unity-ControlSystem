/*
 * @Author: wangyun
 * @CreateTime: 2023-07-09 18:24:28 027
 * @LastEditor: wangyun
 * @EditTime: 2023-07-09 18:24:28 033
 */

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	public class TriggerAspectRatio : BaseTrigger {
		public AspectRatioFitter fitter;
		public bool targetAsValue;
		[HideIf("@this.targetAsValue")]
		public float value;
		[ShowIf("@this.targetAsValue")]
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

		protected override void DoTrigger() {
			if (fitter) {
				float aspectRatio = value;
				if (targetAsValue) {
					if (!target) {
						return;
					}
					Rect rect = target.rect;
					aspectRatio = rect.width / rect.height;
				}
				
				if (m_Tweener != null) {
					m_Tweener.Kill();
					m_Tweener = null;
				}
#if UNITY_EDITOR
				if (tween && Application.isPlaying) {
#else
				if (tween) {
#endif
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
				} else {
					fitter.aspectRatio = aspectRatio;
				}
			}
		}
	}
}
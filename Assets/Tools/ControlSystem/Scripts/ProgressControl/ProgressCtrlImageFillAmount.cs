/*
 * @Author: wangyun
 * @CreateTime: 2023-02-26 23:01:29 917
 * @LastEditor: wangyun
 * @EditTime: 2023-02-26 23:01:29 923
 */

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	[RequireComponent(typeof(Image))]
	public class ProgressCtrlImageFillAmount : BaseProgressCtrlFloat {
		public bool tween;
		[HideIf("@!this.tween")]
		public float tweenDelay;
		[HideIf("@!this.tween")]
		public float tweenDuration = 0.3F;
		[HideIf("@!this.tween")]
		public Ease tweenEase = Ease.OutQuad;
		[HideIf("@!tween || tweenEase != Ease.INTERNAL_Custom")]
		public AnimationCurve tweenEaseCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

		private Tween m_Tweener;

		protected override float TargetValue {
			get => GetComponent<Image>().fillAmount;
			set {
				if (m_Tweener != null) {
					m_Tweener.Kill();
					m_Tweener = null;
				}
				
				Image image = GetComponent<Image>();
				if (image) {
#if UNITY_EDITOR
					if (tween && !controller.InvalidateTween && Application.isPlaying) {
#else
					if (tween && !controller.InvalidateTween) {
#endif
						m_Tweener = DOTween.To(
								() => image.fillAmount,
								v => image.fillAmount = v,
								value,
								tweenDuration
						);
						if (tweenEase == Ease.INTERNAL_Custom) {
							m_Tweener.SetEase(tweenEaseCurve);
						} else {
							m_Tweener.SetEase(tweenEase);
						}
						m_Tweener.SetDelay(tweenDelay).OnComplete(() => m_Tweener = null);
					} else {
						image.fillAmount = value;
					}
				}
			}
		}
	}
}
/*
 * @Author: wangyun
 * @CreateTime: 2023-02-09 20:10:42 557
 * @LastEditor: wangyun
 * @EditTime: 2023-02-09 20:10:42 561
 */

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	[RequireComponent(typeof(HorizontalOrVerticalLayoutGroup))]
	public class StateCtrlLayoutSpacing : BaseStateCtrl<float> {
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
			get => GetComponent<HorizontalOrVerticalLayoutGroup>().spacing;
			set {
				m_Tweener.Kill();
				m_Tweener = null;
				
#if UNITY_EDITOR
				if (tween && !controller.InvalidateTween && Application.isPlaying) {
#else
				if (tween && !controller.InvalidateTween) {
#endif
					HorizontalOrVerticalLayoutGroup layout = GetComponent<HorizontalOrVerticalLayoutGroup>();
					
					m_Tweener = DOTween.To(
						() => layout.spacing,
						v => layout.spacing = v,
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
					GetComponent<HorizontalOrVerticalLayoutGroup>().spacing = value;
				}
			}
		}
	}
}
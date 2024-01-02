/*
 * @Author: wangyun
 * @CreateTime: 2023-02-09 19:22:25 695
 * @LastEditor: wangyun
 * @EditTime: 2023-02-09 19:22:25 699
 */

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	[RequireComponent(typeof(LayoutGroup))]
	public class StateCtrlLayoutPadding : BaseStateCtrl<RectOffset> {
		public PaddingPart part;
		
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
		
		protected override RectOffset TargetValue {
			get {
				RectOffset rectOffset = new RectOffset();
				LayoutGroup layout = GetComponent<LayoutGroup>();
				if (layout) {
					RectOffset padding = layout.padding;
					rectOffset.left = padding.left;
					rectOffset.right = padding.right;
					rectOffset.top = padding.top;
					rectOffset.bottom = padding.bottom;
				}
				return rectOffset;
			}
			set {
				m_Tweener.Kill();
				m_Tweener = null;

#if UNITY_EDITOR
				if (tween && !controller.InvalidateTween && Application.isPlaying) {
#else
				if (tween && !controller.InvalidateTween) {
#endif
					RectOffset padding = GetComponent<LayoutGroup>().padding;
					m_Tweener = DOTween.To(
						() => padding,
						v => {
							if ((part & PaddingPart.LEFT) != 0) {
								padding.left = v.left;
							}
							if ((part & PaddingPart.RIGHT) != 0) {
								padding.right = v.right;
							}
							if ((part & PaddingPart.TOP) != 0) {
								padding.top = v.top;
							}
							if ((part & PaddingPart.BOTTOM) != 0) {
								padding.bottom = v.bottom;
							}
						},
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
					RectOffset padding = GetComponent<LayoutGroup>().padding;
					if ((part & PaddingPart.LEFT) != 0) {
						padding.left = value.left;
					}
					if ((part & PaddingPart.RIGHT) != 0) {
						padding.right = value.right;
					}
					if ((part & PaddingPart.TOP) != 0) {
						padding.top = value.top;
					}
					if ((part & PaddingPart.BOTTOM) != 0) {
						padding.bottom = value.bottom;
					}
				}
			}
		}
	}
}
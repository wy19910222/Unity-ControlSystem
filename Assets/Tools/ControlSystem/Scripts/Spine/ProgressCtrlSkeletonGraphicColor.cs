/*
 * @Author: wangyun
 * @CreateTime: 2023-01-10 21:36:09 530
 * @LastEditor: wangyun
 * @EditTime: 2023-01-10 21:36:09 533
 */

#if SPINE_EXIST

using UnityEngine;
using DG.Tweening;
using Spine.Unity;
using Sirenix.OdinInspector;

namespace Control {
	[RequireComponent(typeof(SkeletonGraphic))]
	public class ProgressCtrlSkeletonGraphicColor : BaseProgressCtrlColor {
		public bool tween;
		[HideIf("@!this.tween")]
		public float tweenDelay;
		[HideIf("@!this.tween")]
		public float tweenDuration = 0.3F;
		[HideIf("@!this.tween")]
		public Ease tweenEase = Ease.OutQuad;
		[HideIf("@!tween || tweenEase != Ease.INTERNAL_Custom")]
		public AnimationCurve tweenEaseCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

		private Tweener m_Tweener;
		
		protected override Color TargetValue {
			get => GetComponent<SkeletonGraphic>().color;
			set {
				if (m_Tweener != null) {
					m_Tweener.Kill();
					m_Tweener = null;
				}
				
				var graphic = GetComponent<SkeletonGraphic>();
#if UNITY_EDITOR
				if (tween && !controller.InvalidateTween && Application.isPlaying) {
#else
				if (tween && !controller.InvalidateTween) {
#endif
					m_Tweener = graphic.DOColor(SetValue(graphic.color, value), tweenDuration);
					if (tweenEase == Ease.INTERNAL_Custom) {
						m_Tweener.SetEase(tweenEaseCurve);
					} else {
						m_Tweener.SetEase(tweenEase);
					}
					m_Tweener.SetDelay(tweenDelay).OnComplete(() => m_Tweener = null);
				} else {
					graphic.color = SetValue(graphic.color, value);
				}
			}
		}
		
		private Color SetValue(Color c, Color value) {
			if ((part & ColorPart.R) != 0) {
				c.r = value.r;
			}
			if ((part & ColorPart.G) != 0) {
				c.g = value.g;
			}
			if ((part & ColorPart.B) != 0) {
				c.b = value.b;
			}
			if ((part & ColorPart.A) != 0) {
				c.a = value.a;
			}
			return c;
		}
	}
}

#endif
/*
 * @Author: wangyun
 * @CreateTime: 2022-03-31 02:11:28 314
 * @LastEditor: wangyun
 * @EditTime: 2022-06-26 00:49:53 073
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	public class StateCtrlColor : BaseStateCtrl<Color> {
		public ColorPart part = ColorPart.RGB;
		
		public bool tween;
		[HideIf("@!this.tween")]
		public float tweenDelay;
		[HideIf("@!this.tween")]
		public float tweenDuration = 0.3F;
		[HideIf("@!this.tween")]
		public Ease tweenEase = Ease.OutQuad;
		[HideIf("@!tween || tweenEase != Ease.INTERNAL_Custom")]
		public AnimationCurve tweenEaseCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

		private readonly HashSet<Tween> m_TweenerSet = new HashSet<Tween>();
		
		protected override Color TargetValue {
			get {
				var graphic = GetComponent<Graphic>();
				if (graphic) {
					return graphic.color;
				}
				var spriteRenderer = GetComponent<SpriteRenderer>();
				if (spriteRenderer) {
					return spriteRenderer.color;
				}
				return Color.white;
			}
			set {
				foreach (var tweener in m_TweenerSet) {
					tweener?.Kill();
				}
				m_TweenerSet.Clear();
				
				var graphic = GetComponent<Graphic>();
				if (graphic) {
#if UNITY_EDITOR
					if (tween && !controller.InvalidateTween && Application.isPlaying) {
#else
					if (tween && !controller.InvalidateTween) {
#endif
						Tweener tweener = graphic.DOColor(SetValue(graphic.color, value), tweenDuration);
						m_TweenerSet.Add(tweener);
						if (tweenEase == Ease.INTERNAL_Custom) {
							tweener.SetEase(tweenEaseCurve);
						} else {
							tweener.SetEase(tweenEase);
						}
						tweener.SetDelay(tweenDelay).OnComplete(() => m_TweenerSet.Remove(tweener));
					} else {
						graphic.color = SetValue(graphic.color, value);
					}
				}
				var spriteRenderer = GetComponent<SpriteRenderer>();
				if (spriteRenderer) {
#if UNITY_EDITOR
					if (tween && !controller.InvalidateTween && Application.isPlaying) {
#else
					if (tween && !controller.InvalidateTween) {
#endif
						Tweener tweener = spriteRenderer.DOColor(SetValue(spriteRenderer.color, value), tweenDuration);
						m_TweenerSet.Add(tweener);
						if (tweenEase == Ease.INTERNAL_Custom) {
							tweener.SetEase(tweenEaseCurve);
						} else {
							tweener.SetEase(tweenEase);
						}
						tweener.SetDelay(tweenDelay).OnComplete(() => m_TweenerSet.Remove(tweener));
					} else {
						spriteRenderer.color = SetValue(spriteRenderer.color, value);
					}
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
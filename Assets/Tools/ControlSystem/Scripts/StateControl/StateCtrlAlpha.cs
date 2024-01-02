/*
 * @Author: wangyun
 * @CreateTime: 2022-03-31 02:11:35 694
 * @LastEditor: wangyun
 * @EditTime: 2023-01-28 15:41:31 155
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	public class StateCtrlAlpha : BaseStateCtrl<float> {
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
		
		protected override float TargetValue {
			get {
				var spriteRenderer = GetComponent<SpriteRenderer>();
				if (spriteRenderer) {
					return spriteRenderer.color.a;
				}
				var group = GetComponent<CanvasGroup>();
				if (group) {
					return group.alpha;
				}
				var graphic = GetComponent<Graphic>();
				if (graphic) {
					return graphic.color.a;
				}
				return 0;
			}
			set {
				foreach (var tweener in m_TweenerSet) {
					tweener?.Kill();
				}
				m_TweenerSet.Clear();

#if UNITY_EDITOR
				if (tween && !controller.InvalidateTween && Application.isPlaying) {
#else
				if (tween && !controller.InvalidateTween) {
#endif
					var spriteRenderer = GetComponent<SpriteRenderer>();
					if (spriteRenderer) {
						Tweener tweener = spriteRenderer.DOFade(value, tweenDuration);
						m_TweenerSet.Add(tweener);
						tweener.OnComplete(() => m_TweenerSet.Remove(tweener));
						if (tweenEase == Ease.INTERNAL_Custom) {
							tweener.SetEase(tweenEaseCurve);
						} else {
							tweener.SetEase(tweenEase);
						}
						tweener.SetDelay(tweenDelay).OnComplete(() => m_TweenerSet.Remove(tweener));
					}
					var group = GetComponent<CanvasGroup>();
					if (group) {
						Tweener tweener = group.DOFade(value, tweenDuration);
						m_TweenerSet.Add(tweener);
						if (tweenEase == Ease.INTERNAL_Custom) {
							tweener.SetEase(tweenEaseCurve);
						} else {
							tweener.SetEase(tweenEase);
						}
						tweener.SetDelay(tweenDelay).OnComplete(() => m_TweenerSet.Remove(tweener));
					} else {
						var graphic = GetComponent<Graphic>();
						if (graphic) {
							Tweener tweener = graphic.DOFade(value, tweenDuration);
							m_TweenerSet.Add(tweener);
							if (tweenEase == Ease.INTERNAL_Custom) {
								tweener.SetEase(tweenEaseCurve);
							} else {
								tweener.SetEase(tweenEase);
							}
							tweener.SetDelay(tweenDelay).OnComplete(() => m_TweenerSet.Remove(tweener));
						}
					}
				} else {
					var spriteRenderer = GetComponent<SpriteRenderer>();
					if (spriteRenderer) {
						var _color = spriteRenderer.color;
						_color.a = value;
						spriteRenderer.color = _color;
					}
					var group = GetComponent<CanvasGroup>();
					if (group) {
						group.alpha = value;
					} else {
						var graphic = GetComponent<Graphic>();
						if (graphic) {
							var _color = graphic.color;
							_color.a = value;
							graphic.color = _color;
						}
					}
				}
			}
		}
	}
}
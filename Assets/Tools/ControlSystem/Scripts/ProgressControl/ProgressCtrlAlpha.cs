/*
 * @Author: wangyun
 * @CreateTime: 2022-03-31 02:11:35 694
 * @LastEditor: wangyun
 * @EditTime: 2022-06-26 00:12:16 178
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	public class ProgressCtrlAlpha : BaseProgressCtrlFloat {
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
				var group = GetComponent<CanvasGroup>();
				if (group) {
					return group.alpha;
				}
				var graphic = GetComponent<Graphic>();
				if (graphic) {
					return graphic.color.a;
				}
				var spriteRenderer = GetComponent<SpriteRenderer>();
				if (spriteRenderer) {
					return spriteRenderer.color.a;
				}
				return 0;
			}
			set {
				foreach (var tweener in m_TweenerSet) {
					tweener?.Kill();
				}
				m_TweenerSet.Clear();
				
				var group = GetComponent<CanvasGroup>();
				if (group) {
#if UNITY_EDITOR
					if (tween && !controller.InvalidateTween && Application.isPlaying) {
#else
					if (tween && !controller.InvalidateTween) {
#endif
						Tweener tweener = group.DOFade(value, tweenDuration);
						m_TweenerSet.Add(tweener);
						if (tweenEase == Ease.INTERNAL_Custom) {
							tweener.SetEase(tweenEaseCurve);
						} else {
							tweener.SetEase(tweenEase);
						}
						tweener.SetDelay(tweenDelay).OnComplete(() => m_TweenerSet.Remove(tweener));
					} else {
						group.alpha = value;
					}
					return;
				}
				var graphic = GetComponent<Graphic>();
				if (graphic) {
#if UNITY_EDITOR
					if (tween && !controller.InvalidateTween && Application.isPlaying) {
#else
					if (tween && !controller.InvalidateTween) {
#endif
						Tweener tweener = graphic.DOFade(value, tweenDuration);
						m_TweenerSet.Add(tweener);
						if (tweenEase == Ease.INTERNAL_Custom) {
							tweener.SetEase(tweenEaseCurve);
						} else {
							tweener.SetEase(tweenEase);
						}
						tweener.SetDelay(tweenDelay).OnComplete(() => m_TweenerSet.Remove(tweener));
					} else {
						var _color = graphic.color;
						_color.a = value;
						graphic.color = _color;
					}
				}
				var spriteRenderer = GetComponent<SpriteRenderer>();
				if (spriteRenderer) {
#if UNITY_EDITOR
					if (tween && !controller.InvalidateTween && Application.isPlaying) {
#else
					if (tween && !controller.InvalidateTween) {
#endif
						Tweener tweener = spriteRenderer.DOFade(value, tweenDuration);
						m_TweenerSet.Add(tweener);
						if (tweenEase == Ease.INTERNAL_Custom) {
							tweener.SetEase(tweenEaseCurve);
						} else {
							tweener.SetEase(tweenEase);
						}
						tweener.SetDelay(tweenDelay).OnComplete(() => m_TweenerSet.Remove(tweener));
					} else {
						var _color = spriteRenderer.color;
						_color.a = value;
						spriteRenderer.color = _color;
					}
				}
			}
		}
	}
}
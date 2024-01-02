/*
 * @Author: wangyun
 * @CreateTime: 2022-11-03 12:13:48 421
 * @LastEditor: wangyun
 * @EditTime: 2022-11-03 12:13:48 429
 */

using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	public class ProgressCtrlSpriteRendererSize : BaseProgressCtrlVector2 {
		[ComponentSelect]
		public SpriteRenderer spriteRenderer;
		
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

		protected override Vector2 TargetValue {
			get {
				return spriteRenderer ? spriteRenderer.size : Vector2.zero;
			}
			set {
				if (!spriteRenderer) {
					return;
				}
				if (m_Tweener != null) {
					m_Tweener.Kill();
					m_Tweener = null;
				}
#if UNITY_EDITOR
				if (tween && !controller.InvalidateTween && Application.isPlaying) {
#else
				if (tween && !controller.InvalidateTween) {
#endif
					m_Tweener = DOTween.To(
						() => spriteRenderer.size,
						v => spriteRenderer.size = SetValue(spriteRenderer.size, v),
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
					spriteRenderer.size = SetValue(spriteRenderer.size, value);
				}
			}
		}
		
		private Vector2 SetValue(Vector2 v2, Vector2 value) {
			if ((part & Vector2Part.X) != 0) {
				v2.x = value.x;
			}
			if ((part & Vector2Part.Y) != 0) {
				v2.y = value.y;
			}
			return v2;
		}
	}
}
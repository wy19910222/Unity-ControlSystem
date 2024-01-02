/*
 * @Author: wangyun
 * @CreateTime: 2023-03-15 00:57:49 158
 * @LastEditor: wangyun
 * @EditTime: 2023-03-15 00:57:49 163
 */

using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	[RequireComponent(typeof(RectTransform))]
	public class StateCtrlRectTransAnchor : BaseStateCtrl<MinMaxVector2> {
		public Vector2Part part = Vector2Part.XY;
		
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

		protected override MinMaxVector2 TargetValue {
			get => Value;
			set {
				if (m_Tweener != null) {
					m_Tweener.Kill();
					m_Tweener = null;
				}
#if UNITY_EDITOR
				if (tween && !controller.InvalidateTween && Application.isPlaying) {
#else
				if (tween && !controller.InvalidateTween) {
#endif
					MinMaxVector2 startValue = Value;
					const float time = 0;
					m_Tweener = DOTween.To(
							() => time,
							v => Value = MinMaxVector2.LerpUnclamped(startValue, value, v),
							1,
							tweenDuration
					);
					if (m_Tweener != null) {
						if (tweenEase == Ease.INTERNAL_Custom) {
							m_Tweener.SetEase(tweenEaseCurve);
						} else {
							m_Tweener.SetEase(tweenEase);
						}
						m_Tweener.SetDelay(tweenDelay).OnComplete(() => m_Tweener = null);
					}
				} else {
					Value = value;
				}
			}
		}
		
		protected MinMaxVector2 Value {
			get {
				var trans = transform as RectTransform;
				return new MinMaxVector2(trans.anchorMin, trans.anchorMax);
			}
			set {
				var trans = transform as RectTransform;
				trans.anchorMin = SetValue(trans.anchorMin, value.min);
				trans.anchorMax = SetValue(trans.anchorMax, value.max);
			}
		}
		
		private Vector2 SetValue(Vector2 v3, Vector2 value) {
			if ((part & Vector2Part.X) != 0) {
				v3.x = value.x;
			}
			if ((part & Vector2Part.Y) != 0) {
				v3.y = value.y;
			}
			return v3;
		}
	}
}
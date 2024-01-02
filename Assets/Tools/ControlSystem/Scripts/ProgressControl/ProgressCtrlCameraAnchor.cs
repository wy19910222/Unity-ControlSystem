/*
 * @Author: wangyun
 * @CreateTime: 2022-06-01 12:19:44 495
 * @LastEditor: wangyun
 * @EditTime: 2022-06-26 00:12:41 421
 */

using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	public class ProgressCtrlCameraAnchor : BaseProgressCtrlVector2 {
		public Camera anchoredCamera;
		[Range(0, 1), ShowIf("@((int) part & (int) Vector2Part.X) != 0")]
		public float xAnchor;
		[Range(0, 1), ShowIf("@((int) part & (int) Vector2Part.Y) != 0")]
		public float yAnchor;
		
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
				return new Vector2(xAnchor, yAnchor);
			}
			set {
				xAnchor = value.x;
				yAnchor = value.y;
				
				if (!anchoredCamera) {
					anchoredCamera = Camera.main;
				}
				if (anchoredCamera) {
					if (m_Tweener != null) {
						m_Tweener.Kill();
						m_Tweener = null;
					}
#if UNITY_EDITOR
					if (tween && !controller.InvalidateTween && Application.isPlaying) {
#else
					if (tween && !controller.InvalidateTween) {
#endif
						var trans = transform;
						var endPos = Anchor2Position();
						var deltaPos = endPos - trans.position;
						var temp = Vector3.zero;
						m_Tweener = DOTween.To(
							() => temp,
							v => {
								trans.position += v - temp;
								temp = v;
							},
							deltaPos,
							tweenDuration
						);
						if (tweenEase == Ease.INTERNAL_Custom) {
							m_Tweener.SetEase(tweenEaseCurve);
						} else {
							m_Tweener.SetEase(tweenEase);
						}
						m_Tweener.SetDelay(tweenDelay).OnComplete(() => m_Tweener = null);
					} else {
						transform.position = Anchor2Position();
					}
				}
			}
		}

		private Vector3 Anchor2Position() {
			var viewPortPos = anchoredCamera.WorldToViewportPoint(transform.position);
			if ((part & Vector2Part.X) != 0) {
				viewPortPos.x = xAnchor;
			}
			if ((part & Vector2Part.Y) != 0) {
				viewPortPos.y = yAnchor;
			}
			return anchoredCamera.ViewportToWorldPoint(viewPortPos);
		}
	}
}
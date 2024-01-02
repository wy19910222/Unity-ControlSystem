/*
 * @Author: wangyun
 * @CreateTime: 2022-06-01 12:19:44 495
 * @LastEditor: wangyun
 * @EditTime: 2022-06-26 00:44:10 598
 */

using System;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	[Flags]
	public enum TriggerCtrlCameraAnchorPart {
		X = 1 << 0,
		Y = 1 << 1
	}

	public class TriggerCtrlCameraAnchor : TriggerCtrlTrigger {
		[ShowIf("@autoTrigger")]
		public bool triggerOnAwake;
		public Camera anchoredCamera;
		public TriggerCtrlCameraAnchorPart part;
		[Range(0, 1), ShowIf("@((int) part & (int) TriggerCtrlCameraAnchorPart.X) != 0")]
		public float xAnchor;
		[Range(0, 1), ShowIf("@((int) part & (int) TriggerCtrlCameraAnchorPart.Y) != 0")]
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

		protected void Awake() {
			if (autoTrigger && triggerOnAwake) {
				Trigger();
			}
		}

		protected new void Start() {
			if (!triggerOnAwake) {
				base.Start();
			}
		}
		
		protected override void DoTrigger() {
			if (!anchoredCamera) {
				anchoredCamera = Camera.main;
			}
			if (anchoredCamera) {
				if (m_Tweener != null) {
					m_Tweener.Kill();
					m_Tweener = null;
				}
#if UNITY_EDITOR
				if (tween && Application.isPlaying) {
#else
				if (tween) {
#endif
					Transform trans = transform;
					Vector3 endPos = Anchor2Position();
					Vector3 deltaPos = endPos - trans.position;
					Vector3 temp = Vector3.zero;
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

		private Vector3 Anchor2Position() {
			Vector3 viewPortPos = anchoredCamera.WorldToViewportPoint(transform.position);
			if ((part & TriggerCtrlCameraAnchorPart.X) != 0) {
				viewPortPos.x = xAnchor;
			}
			if ((part & TriggerCtrlCameraAnchorPart.Y) != 0) {
				viewPortPos.y = yAnchor;
			}
			return anchoredCamera.ViewportToWorldPoint(viewPortPos);
		}

#if UNITY_EDITOR
		[ContextMenu("Calculate Current Anchor")]
		private void CalculateAnchor() {
			if (!anchoredCamera) {
				anchoredCamera = Camera.main;
			}
			Vector3 viewPortPos = anchoredCamera.WorldToViewportPoint(transform.position);
			if ((part & TriggerCtrlCameraAnchorPart.X) != 0) {
				xAnchor = viewPortPos.x;
			}
			if ((part & TriggerCtrlCameraAnchorPart.Y) != 0) {
				yAnchor = viewPortPos.y;
			}
		}
#endif
	}
}
/*
 * @Author: wangyun
 * @CreateTime: 2022-08-28 01:39:21 517
 * @LastEditor: wangyun
 * @EditTime: 2022-08-28 01:39:21 522
 */

using System;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	public enum TriggerCtrlTransType {
		NONE,
		LOCAL_POSITION = 1,
		LOCAL_ANGLES = 2,
		LOCAL_SCALE = 3
	}

	[Flags]
	public enum TriggerCtrlTransPart {
		X = 1 << 0,
		Y = 1 << 1,
		Z = 1 << 2,
		XYZ = X | Y | Z
	}
	
	public class TriggerCtrlTrans : TriggerCtrlTrigger {
		public TriggerCtrlTransType type = TriggerCtrlTransType.LOCAL_POSITION;
		public TriggerCtrlTransPart part = TriggerCtrlTransPart.XYZ;
		public Vector3 value;
		
		public bool tween;
		[HideIf("@!tween")]
		public float tweenDelay;
		[HideIf("@!tween")]
		public float tweenDuration = 0.3F;
		[HideIf("@!tween")]
		public Ease tweenEase = Ease.OutQuad;
		[HideIf("@!tween || tweenEase != Ease.INTERNAL_Custom")]
		public AnimationCurve tweenEaseCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

		private Tweener m_Tweener;

		protected override void DoTrigger() {
			if (m_Tweener != null) {
				m_Tweener.Kill();
				m_Tweener = null;
			}
#if UNITY_EDITOR
			if (tween && Application.isPlaying) {
#else
			if (tween) {
#endif
				var trans = transform;
				switch (type) {
					case TriggerCtrlTransType.LOCAL_POSITION:
						m_Tweener = DOTween.To(
								() => trans.localPosition,
								v => trans.localPosition = SetValue(trans.localPosition, v),
								value,
								tweenDuration
						);
						break;
					case TriggerCtrlTransType.LOCAL_ANGLES:
						m_Tweener = DOTween.To(
								() => trans.localRotation,
								v => trans.localEulerAngles = SetValue(trans.localEulerAngles, v.eulerAngles),
								value,
								tweenDuration
						);
						break;
					case TriggerCtrlTransType.LOCAL_SCALE:
						m_Tweener = DOTween.To(
								() => trans.localScale,
								v => trans.localScale = SetValue(trans.localScale, v),
								value,
								tweenDuration
						);
						break;
				}
				if (m_Tweener != null) {
					if (tweenEase == Ease.INTERNAL_Custom) {
						m_Tweener.SetEase(tweenEaseCurve);
					} else {
						m_Tweener.SetEase(tweenEase);
					}
					m_Tweener.SetDelay(tweenDelay).OnComplete(() => m_Tweener = null);
				}
			} else {
				switch (type) {
					case TriggerCtrlTransType.LOCAL_POSITION:
						transform.localPosition = SetValue(transform.localPosition, value);
						break;
					case TriggerCtrlTransType.LOCAL_ANGLES:
						transform.localEulerAngles = SetValue(transform.localEulerAngles, value);
						break;
					case TriggerCtrlTransType.LOCAL_SCALE:
						transform.localScale = SetValue(transform.localScale, value);
						break;
				}
			}
		}
		
		private Vector3 SetValue(Vector3 v3, Vector3 v) {
			if ((part & TriggerCtrlTransPart.X) != 0) {
				v3.x = v.x;
			}
			if ((part & TriggerCtrlTransPart.Y) != 0) {
				v3.y = v.y;
			}
			if ((part & TriggerCtrlTransPart.Z) != 0) {
				v3.z = v.z;
			}
			return v3;
		}
	}
}
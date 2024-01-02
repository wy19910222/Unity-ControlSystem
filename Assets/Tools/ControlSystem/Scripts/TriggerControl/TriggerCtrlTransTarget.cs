/*
 * @Author: wangyun
 * @CreateTime: 2022-08-02 20:33:50 684
 * @LastEditor: wangyun
 * @EditTime: 2022-08-02 20:33:50 689
 */

using System;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	public enum TriggerCtrlTransTargetType {
		NONE,
		POSITION = 1,
		ANGLES = 2,
		LOCAL_SCALE = 3
	}

	[Flags]
	public enum TriggerCtrlTransTargetPart {
		X = 1 << 0,
		Y = 1 << 1,
		Z = 1 << 2,
		XYZ = X | Y | Z
	}
	
	public class TriggerCtrlTransTarget : TriggerCtrlTrigger {
		public TriggerCtrlTransTargetType type = TriggerCtrlTransTargetType.POSITION;
		public TriggerCtrlTransTargetPart part = TriggerCtrlTransTargetPart.XYZ;
		public Transform target;
		
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
				if (target) {
					var trans = transform;
					switch (type) {
						case TriggerCtrlTransTargetType.POSITION:
							m_Tweener = DOTween.To(
										() => trans.position,
										v => trans.position = SetValue(trans.position, v),
										target.position,
										tweenDuration
							);
							break;
						case TriggerCtrlTransTargetType.ANGLES:
							m_Tweener = DOTween.To(
										() => trans.rotation,
										v => trans.eulerAngles = SetValue(trans.eulerAngles, v.eulerAngles),
										target.eulerAngles,
										tweenDuration
							);
							break;
						case TriggerCtrlTransTargetType.LOCAL_SCALE:
							m_Tweener = DOTween.To(
										() => trans.localScale,
										v => trans.localScale = SetValue(trans.localScale, v),
										target.localScale,
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
				}
			} else {
				if (target) {
					switch (type) {
						case TriggerCtrlTransTargetType.POSITION:
							transform.position = SetValue(transform.position, target.position);
							break;
						case TriggerCtrlTransTargetType.ANGLES:
							transform.eulerAngles = SetValue(transform.eulerAngles, target.eulerAngles);
							break;
						case TriggerCtrlTransTargetType.LOCAL_SCALE:
							transform.localScale = SetValue(transform.localScale, target.localScale);
							break;
					}
				}
			}
		}
		
		private Vector3 SetValue(Vector3 v3, Vector3 value) {
			if ((part & TriggerCtrlTransTargetPart.X) != 0) {
				v3.x = value.x;
			}
			if ((part & TriggerCtrlTransTargetPart.Y) != 0) {
				v3.y = value.y;
			}
			if ((part & TriggerCtrlTransTargetPart.Z) != 0) {
				v3.z = value.z;
			}
			return v3;
		}
	}
}
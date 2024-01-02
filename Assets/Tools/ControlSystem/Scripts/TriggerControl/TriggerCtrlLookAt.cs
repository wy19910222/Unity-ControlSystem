/*
 * @Author: wangyun
 * @CreateTime: 2022-04-24 17:11:17 160
 * @LastEditor: wangyun
 * @EditTime: 2022-04-24 17:11:17 148
 */

using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	public class TriggerCtrlLookAt : TriggerCtrlTrigger {
		public Vector3 baseDir = Vector3.forward;
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
				Transform trans = transform;
				Quaternion fromRot = trans.rotation;
				trans.LookAt(target);
				Quaternion toRot = trans.rotation * Quaternion.FromToRotation(baseDir, Vector3.forward);
				trans.rotation = fromRot;
				m_Tweener = trans.DORotateQuaternion(toRot, tweenDuration);
				if (tweenEase == Ease.INTERNAL_Custom) {
					m_Tweener.SetEase(tweenEaseCurve);
				} else {
					m_Tweener.SetEase(tweenEase);
				}
				m_Tweener.SetDelay(tweenDelay).OnComplete(() => m_Tweener = null);
			} else {
				Transform trans = transform;
				trans.LookAt(target);
				trans.rotation *= Quaternion.FromToRotation(baseDir, Vector3.forward);
			}
		}
	}
}
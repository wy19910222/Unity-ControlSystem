/*
 * @Author: wangyun
 * @CreateTime: 2022-08-28 01:48:04 030
 * @LastEditor: wangyun
 * @EditTime: 2022-08-28 01:48:04 033
 */

using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Control {
	public class ProgressCtrlProgress : BaseProgressCtrlFloat {
		[ComponentSelect]
		public ProgressController target;
		
		public bool tween;
		[HideIf("@!this.tween")]
		public float tweenDelay;
		[HideIf("@!this.tween")]
		public float tweenDuration = 0.3F;
		[HideIf("@!this.tween")]
		public Ease tweenEase = Ease.OutQuad;
		[HideIf("@!tween || tweenEase != Ease.INTERNAL_Custom")]
		public AnimationCurve tweenEaseCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

		private Tween m_Tweener;

		protected override void Reset() {
			base.Reset();
			target = GetComponent<ProgressController>();
		}
		
		protected override float TargetValue {
			get => target ? target.Progress : 0;
			set {
				if (m_Tweener != null) {
					m_Tweener.Kill();
					m_Tweener = null;
				}
				if (target) {
#if UNITY_EDITOR
					if (tween && !controller.InvalidateTween && Application.isPlaying) {
#else
					if (tween && !controller.InvalidateTween) {
#endif
						m_Tweener = DOTween.To(
							() => target.Progress,
							v => target.Progress = v,
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
						target.Progress = value;
					}
				}
			}
		}
	}
}
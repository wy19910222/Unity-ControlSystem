/*
 * @Author: wangyun
 * @CreateTime: 2022-04-20 20:33:25 096
 * @LastEditor: wangyun
 * @EditTime: 2022-07-25 12:15:08 484
 */

using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	public class TriggerCtrlProgressController : TriggerCtrlTrigger {
		[ComponentSelect]
		public ProgressController controller;
		public bool random;
		[HideIf("@random")]
		[Range(0, 1)]
		public float progress;
		
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
			if (controller) {
				if (m_Tweener != null) {
					m_Tweener.Kill();
					m_Tweener = null;
				}
#if UNITY_EDITOR
				if (tween && Application.isPlaying) {
#else
				if (tween) {
#endif
					m_Tweener = DOTween.To(
						() => controller.Progress,
						v => controller.Progress = v,
						random ? Random.value : progress,
						tweenDuration
					);
					if (tweenEase == Ease.INTERNAL_Custom) {
						m_Tweener.SetEase(tweenEaseCurve);
					} else {
						m_Tweener.SetEase(tweenEase);
					}
					m_Tweener.SetDelay(tweenDelay).OnComplete(() => m_Tweener = null);
				} else {
					controller.Progress = random ? Random.value : progress;
				}
			}
		}
	}
}
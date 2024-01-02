/*
 * @Author: wangyun
 * @CreateTime: 2022-08-08 21:07:15 733
 * @LastEditor: wangyun
 * @EditTime: 2022-08-08 21:07:15 737
 */

#if SPINE_EXIST

using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Spine.Unity;
using Sirenix.OdinInspector;

namespace Control {
	public class StateCtrlSkeletonSpeed : BaseStateCtrl<float> {
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
				SkeletonAnimation sa = GetComponent<SkeletonAnimation>();
				if (sa) {
					return sa.timeScale;
				}
				SkeletonGraphic sg = GetComponent<SkeletonGraphic>();
				if (sg) {
					return sg.timeScale;
				}
				return 1;
			}
			set {
				foreach (var tweener in m_TweenerSet) {
					tweener?.Kill();
				}
				m_TweenerSet.Clear();
				
				SkeletonAnimation sa = GetComponent<SkeletonAnimation>();
				if (sa) {
#if UNITY_EDITOR
					if (tween && !controller.InvalidateTween && Application.isPlaying) {
#else
					if (tween && !controller.InvalidateTween) {
#endif
						Tweener tweener = DOTween.To(
							() => sa.timeScale,
							v => sa.timeScale = v,
							value,
							tweenDuration
						);
						m_TweenerSet.Add(tweener);
						if (tweenEase == Ease.INTERNAL_Custom) {
							tweener.SetEase(tweenEaseCurve);
						} else {
							tweener.SetEase(tweenEase);
						}
						tweener.SetDelay(tweenDelay).OnComplete(() => m_TweenerSet.Remove(tweener));
					} else {
						sa.timeScale = value;
					}
				}
				SkeletonGraphic sg = GetComponent<SkeletonGraphic>();
				if (sg) {
#if UNITY_EDITOR
					if (tween && !controller.InvalidateTween && Application.isPlaying) {
#else
					if (tween && !controller.InvalidateTween) {
#endif
						Tweener tweener = DOTween.To(
							() => sg.timeScale,
							v => sg.timeScale = v,
							value,
							tweenDuration
						);
						m_TweenerSet.Add(tweener);
						if (tweenEase == Ease.INTERNAL_Custom) {
							tweener.SetEase(tweenEaseCurve);
						} else {
							tweener.SetEase(tweenEase);
						}
						tweener.SetDelay(tweenDelay).OnComplete(() => m_TweenerSet.Remove(tweener));
					} else {
						sg.timeScale = value;
					}
				}
			}
		}
	}
}

#endif
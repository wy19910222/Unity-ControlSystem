/*
 * @Author: wangyun
 * @CreateTime: 2023-01-28 16:15:28 604
 * @LastEditor: wangyun
 * @EditTime: 2023-01-28 16:15:28 608
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Sirenix.OdinInspector;

namespace Control {
	public class StateCtrlTextFontSize : BaseStateCtrl<float> {
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
				Text text = GetComponent<Text>();
				if (text) {
					return text.fontSize;
				}
				TMP_Text tmp_text = GetComponent<TMP_Text>();
				// ReSharper disable once ConvertIfStatementToReturnStatement
				if (tmp_text) {
					return tmp_text.fontSize;
				}
				return 0;
			}
			set {
				foreach (var tweener in m_TweenerSet) {
					tweener?.Kill();
				}
				m_TweenerSet.Clear();
				
#if UNITY_EDITOR
				if (tween && !controller.InvalidateTween && Application.isPlaying) {
#else
				if (tween && !controller.InvalidateTween) {
#endif
					Text text = GetComponent<Text>();
					if (text) {
						Tweener tweener = DOTween.To(
							() => text.fontSize,
							v => text.fontSize = Mathf.RoundToInt(v),
							value,
							tweenDuration
						);
						m_TweenerSet.Add(tweener);
						tweener.OnComplete(() => m_TweenerSet.Remove(tweener));
						if (tweenEase == Ease.INTERNAL_Custom) {
							tweener.SetEase(tweenEaseCurve);
						} else {
							tweener.SetEase(tweenEase);
						}
						tweener.SetDelay(tweenDelay).OnComplete(() => m_TweenerSet.Remove(tweener));
					}
					TMP_Text tmp_text = GetComponent<TMP_Text>();
					if (tmp_text) {
						Tweener tweener = DOTween.To(
							() => tmp_text.fontSize,
							v => tmp_text.fontSize = v,
							value,
							tweenDuration
						);
						m_TweenerSet.Add(tweener);
						tweener.OnComplete(() => m_TweenerSet.Remove(tweener));
						if (tweenEase == Ease.INTERNAL_Custom) {
							tweener.SetEase(tweenEaseCurve);
						} else {
							tweener.SetEase(tweenEase);
						}
						tweener.SetDelay(tweenDelay).OnComplete(() => m_TweenerSet.Remove(tweener));
					}
				} else {
					Text text = GetComponent<Text>();
					if (text) {
						text.fontSize = Mathf.RoundToInt(value);
					}
					TMP_Text tmp_text = GetComponent<TMP_Text>();
					if (tmp_text) {
						tmp_text.fontSize = value;
					}
				}
			}
		}
	}
}
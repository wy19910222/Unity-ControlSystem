/*
 * @Author: wangyun
 * @CreateTime: 2023-01-28 16:40:30 046
 * @LastEditor: wangyun
 * @EditTime: 2023-01-28 16:40:30 054
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Sirenix.OdinInspector;

namespace Control {
	public enum ProgressCtrlTextSpacingType {
		LINE,
		CHAR,
		WORD,
		PARAGRAPH
	}

	public class ProgressCtrlTextSpacing : BaseProgressCtrlFloat {
		public ProgressCtrlTextSpacingType type = ProgressCtrlTextSpacingType.LINE;
		
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
					return type == ProgressCtrlTextSpacingType.LINE ? text.lineSpacing : 0;
				}
				TMP_Text tmp_text = GetComponent<TMP_Text>();
				if (tmp_text) {
					switch (type) {
						case ProgressCtrlTextSpacingType.LINE:
							return tmp_text.lineSpacing;
						case ProgressCtrlTextSpacingType.CHAR:
							return tmp_text.characterSpacing;
						case ProgressCtrlTextSpacingType.WORD:
							return tmp_text.wordSpacing;
						case ProgressCtrlTextSpacingType.PARAGRAPH:
							return tmp_text.paragraphSpacing;
						default:
							return 0;
					}
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
						if (type == ProgressCtrlTextSpacingType.LINE) {
							Tweener tweener = DOTween.To(
								() => text.lineSpacing,
								v => text.lineSpacing = v,
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
					}
					TMP_Text tmp_text = GetComponent<TMP_Text>();
					if (tmp_text) {
						Tweener tweener = DOTween.To(
							() => {
								switch (type) {
									case ProgressCtrlTextSpacingType.LINE:
										return tmp_text.lineSpacing;
									case ProgressCtrlTextSpacingType.CHAR:
										return tmp_text.characterSpacing;
									case ProgressCtrlTextSpacingType.WORD:
										return tmp_text.wordSpacing;
									case ProgressCtrlTextSpacingType.PARAGRAPH:
										return tmp_text.paragraphSpacing;
									default:
										return 0;
								}
							},
							v => {
								switch (type) {
									case ProgressCtrlTextSpacingType.LINE:
										tmp_text.lineSpacing = v;
										break;
									case ProgressCtrlTextSpacingType.CHAR:
										tmp_text.characterSpacing = v;
										break;
									case ProgressCtrlTextSpacingType.WORD:
										tmp_text.wordSpacing = v;
										break;
									case ProgressCtrlTextSpacingType.PARAGRAPH:
										tmp_text.paragraphSpacing = v;
										break;
									default:
										throw new ArgumentOutOfRangeException();
								}
							},
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
						if (type == ProgressCtrlTextSpacingType.LINE) {
							text.lineSpacing = value;
						}
					}
					TMP_Text tmp_text = GetComponent<TMP_Text>();
					if (tmp_text) {
						switch (type) {
							case ProgressCtrlTextSpacingType.LINE:
								tmp_text.lineSpacing = value;
								break;
							case ProgressCtrlTextSpacingType.CHAR:
								tmp_text.characterSpacing = value;
								break;
							case ProgressCtrlTextSpacingType.WORD:
								tmp_text.wordSpacing = value;
								break;
							case ProgressCtrlTextSpacingType.PARAGRAPH:
								tmp_text.paragraphSpacing = value;
								break;
							default:
								throw new ArgumentOutOfRangeException();
						}
					}
				}
			}
		}
	}
}
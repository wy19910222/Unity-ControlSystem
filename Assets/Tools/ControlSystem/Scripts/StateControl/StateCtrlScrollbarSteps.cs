/*
 * @Author: wangyun
 * @CreateTime: 2023-02-26 22:40:18 248
 * @LastEditor: wangyun
 * @EditTime: 2023-02-26 22:40:18 254
 */

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	public enum StateCtrlScrollbarStepsType {
		Round,
		Floor = 1,
		Ceil = 2
	}
	
	[RequireComponent(typeof(Scrollbar))]
	public class StateCtrlScrollbarSteps : BaseStateCtrl<int> {
		[HideIf("@!this.tween")]
		public StateCtrlScrollbarStepsType type;
		
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

		protected override int TargetValue {
			get => Value;
			set {
				if (m_Tweener != null) {
					m_Tweener.Kill();
					m_Tweener = null;
				}
				
#if UNITY_EDITOR
				if (tween && !controller.InvalidateTween && Application.isPlaying) {
#else
				if (tween && !controller.InvalidateTween) {
#endif
					m_Tweener = DOTween.To(
							() => Value,
							v => {
								switch (type) {
									case StateCtrlScrollbarStepsType.Round:
										Value = Mathf.RoundToInt(v);
										break;
									case StateCtrlScrollbarStepsType.Floor:
										Value = Mathf.FloorToInt(v);
										break;
									case StateCtrlScrollbarStepsType.Ceil:
										Value = Mathf.CeilToInt(v);
										break;
								}
							},
							(float) value,
							tweenDuration
					);
					if (tweenEase == Ease.INTERNAL_Custom) {
						m_Tweener.SetEase(tweenEaseCurve);
					} else {
						m_Tweener.SetEase(tweenEase);
					}
					m_Tweener.SetDelay(tweenDelay).OnComplete(() => m_Tweener = null);
				} else {
					Value = value;
				}
			}
		}

		private int Value {
			get {
				Scrollbar scrollbar = GetComponent<Scrollbar>();
				return scrollbar ? scrollbar.numberOfSteps : 0;
			}
			set {
				Scrollbar scrollbar = GetComponent<Scrollbar>();
				if (scrollbar) {
					scrollbar.numberOfSteps = value;
				}
			}
		}
	}
}
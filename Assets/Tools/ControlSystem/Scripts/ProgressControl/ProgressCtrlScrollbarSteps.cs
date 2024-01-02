/*
 * @Author: wangyun
 * @CreateTime: 2023-02-26 23:08:42 411
 * @LastEditor: wangyun
 * @EditTime: 2023-02-26 23:08:42 423
 */

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	public enum ProgressCtrlScrollbarStepsType {
		Round,
		Floor = 1,
		Ceil = 2
	}
	
	[RequireComponent(typeof(Scrollbar))]
	public class ProgressCtrlScrollbarSteps : BaseProgressCtrlFloat {
		[HideIf("@!this.tween")]
		public ProgressCtrlScrollbarStepsType type;
		
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

		protected override float TargetValue {
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
							v => Value = v,
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
					Value = value;
				}
			}
		}

		private float Value {
			get {
				Scrollbar scrollbar = GetComponent<Scrollbar>();
				return scrollbar ? scrollbar.numberOfSteps : 0;
			}
			set {
				Scrollbar scrollbar = GetComponent<Scrollbar>();
				if (scrollbar) {
					switch (type) {
						case ProgressCtrlScrollbarStepsType.Round:
							scrollbar.numberOfSteps = Mathf.RoundToInt(value);
							break;
						case ProgressCtrlScrollbarStepsType.Floor:
							scrollbar.numberOfSteps = Mathf.FloorToInt(value);
							break;
						case ProgressCtrlScrollbarStepsType.Ceil:
							scrollbar.numberOfSteps = Mathf.CeilToInt(value);
							break;
					}
				}
			}
		}
	}
}
/*
 * @Author: wangyun
 * @CreateTime: 2023-02-26 22:15:34 196
 * @LastEditor: wangyun
 * @EditTime: 2023-02-26 22:15:34 206
 */

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	public enum ProgressCtrlSliderFloatType {
		VALUE,
		MIN_VALUE,
		MAX_VALUE
	}
	
	[RequireComponent(typeof(Slider))]
	public class ProgressCtrlSliderFloat : BaseProgressCtrlFloat {
		public ProgressCtrlSliderFloatType type;
		
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
				Slider slider = GetComponent<Slider>();
				if (slider) {
					switch (type) {
						case ProgressCtrlSliderFloatType.VALUE:
							return slider.value;
						case ProgressCtrlSliderFloatType.MIN_VALUE:
							return slider.minValue;
						case ProgressCtrlSliderFloatType.MAX_VALUE:
							return slider.maxValue;
					}
				}
				return 0;
			}
			set {
				Slider slider = GetComponent<Slider>();
				if (slider) {
					switch (type) {
						case ProgressCtrlSliderFloatType.VALUE:
							slider.value = value;
							break;
						case ProgressCtrlSliderFloatType.MIN_VALUE:
							slider.minValue = value;
							break;
						case ProgressCtrlSliderFloatType.MAX_VALUE:
							slider.maxValue = value;
							break;
					}
				}
			}
		}
	}
}
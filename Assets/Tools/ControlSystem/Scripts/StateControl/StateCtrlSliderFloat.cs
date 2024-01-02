/*
 * @Author: wangyun
 * @CreateTime: 2023-02-26 22:25:25 840
 * @LastEditor: wangyun
 * @EditTime: 2023-02-26 22:25:25 846
 */

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	public enum StateCtrlSliderFloatType {
		VALUE,
		MIN_VALUE,
		MAX_VALUE
	}
	
	[RequireComponent(typeof(Slider))]
	public class StateCtrlSliderFloat : BaseStateCtrl<float> {
		public StateCtrlSliderFloatType type;
		
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
						case StateCtrlSliderFloatType.VALUE:
							return slider.value;
						case StateCtrlSliderFloatType.MIN_VALUE:
							return slider.minValue;
						case StateCtrlSliderFloatType.MAX_VALUE:
							return slider.maxValue;
					}
				}
				return 0;
			}
			set {
				Slider slider = GetComponent<Slider>();
				if (slider) {
					switch (type) {
						case StateCtrlSliderFloatType.VALUE:
							slider.value = value;
							break;
						case StateCtrlSliderFloatType.MIN_VALUE:
							slider.minValue = value;
							break;
						case StateCtrlSliderFloatType.MAX_VALUE:
							slider.maxValue = value;
							break;
					}
				}
			}
		}
	}
}
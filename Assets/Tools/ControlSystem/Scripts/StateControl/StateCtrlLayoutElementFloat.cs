/*
 * @Author: wangyun
 * @CreateTime: 2023-04-15 22:03:55 460
 * @LastEditor: wangyun
 * @EditTime: 2023-04-15 22:03:55 465
 */

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	public enum StateCtrlLayoutElementFloatType {
		MIN_WIDTH,
		MIN_HEIGHT,
		PREFERRED_WIDTH,
		PREFERRED_HEIGHT,
		FLEXIBLE_WIDTH,
		FLEXIBLE_HEIGHT
	}
	
	[RequireComponent(typeof(LayoutElement))]
	public class StateCtrlLayoutElementFloat : BaseStateCtrl<float> {
		public StateCtrlLayoutElementFloatType type;
		
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
				LayoutElement layoutElement = GetComponent<LayoutElement>();
				if (layoutElement) {
					switch (type) {
						case StateCtrlLayoutElementFloatType.MIN_WIDTH:
							return layoutElement.minWidth;
						case StateCtrlLayoutElementFloatType.MIN_HEIGHT:
							return layoutElement.minHeight;
						case StateCtrlLayoutElementFloatType.PREFERRED_WIDTH:
							return layoutElement.preferredWidth;
						case StateCtrlLayoutElementFloatType.PREFERRED_HEIGHT:
							return layoutElement.preferredHeight;
						case StateCtrlLayoutElementFloatType.FLEXIBLE_WIDTH:
							return layoutElement.flexibleWidth;
						case StateCtrlLayoutElementFloatType.FLEXIBLE_HEIGHT:
							return layoutElement.flexibleHeight;
					}
				}
				return 0;
			}
			set {
				LayoutElement layoutElement = GetComponent<LayoutElement>();
				if (layoutElement) {
					switch (type) {
						case StateCtrlLayoutElementFloatType.MIN_WIDTH:
							layoutElement.minWidth = value;
							break;
						case StateCtrlLayoutElementFloatType.MIN_HEIGHT:
							layoutElement.minHeight = value;
							break;
						case StateCtrlLayoutElementFloatType.PREFERRED_WIDTH:
							layoutElement.preferredWidth = value;
							break;
						case StateCtrlLayoutElementFloatType.PREFERRED_HEIGHT:
							layoutElement.preferredHeight = value;
							break;
						case StateCtrlLayoutElementFloatType.FLEXIBLE_WIDTH:
							layoutElement.flexibleWidth = value;
							break;
						case StateCtrlLayoutElementFloatType.FLEXIBLE_HEIGHT:
							layoutElement.flexibleHeight = value;
							break;
					}
				}
			}
		}
	}
}
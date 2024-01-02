/*
 * @Author: wangyun
 * @CreateTime: 2022-07-15 15:01:52 488
 * @LastEditor: wangyun
 * @EditTime: 2022-07-15 15:01:52 482
 */

using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	public enum StateCtrlRectTransTargetType {
		NONE,
		ANCHOR_MIN = 1,
		ANCHOR_MAX = 2,
		ANCHORED_POSITION = 3,
		SIZE_DELTA = 4,
		PIVOT = 5
	}

	[RequireComponent(typeof(RectTransform))]
	public class StateCtrlRectTransTarget : BaseStateCtrl<RectTransform> {
		public StateCtrlRectTransTargetType type;
		public Vector2Part part = Vector2Part.XY;
		public RectTransform target;
		
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

		protected override RectTransform TargetValue {
			get => target;
			set {
				target = value;
				if (m_Tweener != null) {
					m_Tweener.Kill();
					m_Tweener = null;
				}
#if UNITY_EDITOR
				if (tween && !controller.InvalidateTween && Application.isPlaying) {
#else
				if (tween && !controller.InvalidateTween) {
#endif
					if (target) {
						var trans = transform as RectTransform;
						switch (type) {
							case StateCtrlRectTransTargetType.ANCHOR_MIN:
								m_Tweener = DOTween.To(() => trans.anchorMin, _value => trans.anchorMin = _value, SetValue(trans.anchorMin, target.anchorMin), tweenDuration);
								break;
							case StateCtrlRectTransTargetType.ANCHOR_MAX:
								m_Tweener = DOTween.To(() => trans.anchorMax, _value => trans.anchorMax = _value, SetValue(trans.anchorMax, target.anchorMax), tweenDuration);
								break;
							case StateCtrlRectTransTargetType.ANCHORED_POSITION:
								m_Tweener = DOTween.To(() => trans.anchoredPosition, _value => trans.anchoredPosition = _value, SetValue(trans.anchoredPosition, target.anchoredPosition), tweenDuration);
								break;
							case StateCtrlRectTransTargetType.SIZE_DELTA:
								m_Tweener = DOTween.To(() => trans.sizeDelta, _value => trans.sizeDelta = _value, SetValue(trans.sizeDelta, target.sizeDelta), tweenDuration);
								break;
							case StateCtrlRectTransTargetType.PIVOT:
								m_Tweener = DOTween.To(() => trans.pivot, _value => trans.pivot = _value, SetValue(trans.pivot, target.pivot), tweenDuration);
								break;
						}
						if (m_Tweener != null) {
							if (tweenEase == Ease.INTERNAL_Custom) {
								m_Tweener.SetEase(tweenEaseCurve);
							} else {
								m_Tweener.SetEase(tweenEase);
							}
							m_Tweener.SetDelay(tweenDelay).OnComplete(() => m_Tweener = null);
						}
					}
				} else {
					if (target) {
						var trans = transform as RectTransform;
						switch (type) {
							case StateCtrlRectTransTargetType.ANCHOR_MIN:
								trans.anchorMin = SetValue(trans.anchorMin, target.anchorMin);
								break;
							case StateCtrlRectTransTargetType.ANCHOR_MAX:
								trans.anchorMax = SetValue(trans.anchorMax, target.anchorMax);
								break;
							case StateCtrlRectTransTargetType.ANCHORED_POSITION:
								trans.anchoredPosition = SetValue(trans.anchoredPosition, target.anchoredPosition);
								break;
							case StateCtrlRectTransTargetType.SIZE_DELTA:
								trans.sizeDelta = SetValue(trans.sizeDelta, target.sizeDelta);
								break;
							case StateCtrlRectTransTargetType.PIVOT:
								trans.pivot = SetValue(trans.pivot, target.pivot);
								break;
						}
					}
				}
			}
		}
		
		private Vector2 SetValue(Vector2 v3, Vector2 value) {
			if ((part & Vector2Part.X) != 0) {
				v3.x = value.x;
			}
			if ((part & Vector2Part.Y) != 0) {
				v3.y = value.y;
			}
			return v3;
		}
	}
}
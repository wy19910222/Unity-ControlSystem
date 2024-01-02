/*
 * @Author: wangyun
 * @CreateTime: 2022-03-30 16:21:41 627
 * @LastEditor: wangyun
 * @EditTime: 2022-06-26 01:02:11 680
 */

using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	public enum StateCtrlRectTransType {
		NONE,
		ANCHOR_MIN = 1,
		ANCHOR_MAX = 2,
		ANCHORED_POSITION = 3,
		SIZE_DELTA = 4,
		PIVOT = 5
	}

	[RequireComponent(typeof(RectTransform))]
	public class StateCtrlRectTrans : BaseStateCtrl<Vector2> {
		public StateCtrlRectTransType type;
		public Vector2Part part = Vector2Part.XY;
		
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

		protected override Vector2 TargetValue {
			get {
				var trans = transform as RectTransform;
				switch (type) {
					case StateCtrlRectTransType.ANCHOR_MIN:
						return trans.anchorMin;
					case StateCtrlRectTransType.ANCHOR_MAX:
						return trans.anchorMax;
					case StateCtrlRectTransType.ANCHORED_POSITION:
						return trans.anchoredPosition;
					case StateCtrlRectTransType.SIZE_DELTA:
						return trans.sizeDelta;
					case StateCtrlRectTransType.PIVOT:
						return trans.pivot;
				}
				return Vector2.zero;
			}
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
					var trans = transform as RectTransform;
					switch (type) {
						case StateCtrlRectTransType.ANCHOR_MIN:
							m_Tweener = DOTween.To(() => trans.anchorMin, _value => trans.anchorMin = _value, SetValue(trans.anchorMin, value), tweenDuration);
							break;
						case StateCtrlRectTransType.ANCHOR_MAX:
							m_Tweener = DOTween.To(() => trans.anchorMax, _value => trans.anchorMax = _value, SetValue(trans.anchorMax, value), tweenDuration);
							break;
						case StateCtrlRectTransType.ANCHORED_POSITION:
							m_Tweener = DOTween.To(() => trans.anchoredPosition, _value => trans.anchoredPosition = _value, SetValue(trans.anchoredPosition, value), tweenDuration);
							break;
						case StateCtrlRectTransType.SIZE_DELTA:
							m_Tweener = DOTween.To(() => trans.sizeDelta, _value => trans.sizeDelta = _value, SetValue(trans.sizeDelta, value), tweenDuration);
							break;
						case StateCtrlRectTransType.PIVOT:
							m_Tweener = DOTween.To(() => trans.pivot, _value => trans.pivot = _value, SetValue(trans.pivot, value), tweenDuration);
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
				} else {
					var trans = transform as RectTransform;
					switch (type) {
						case StateCtrlRectTransType.ANCHOR_MIN:
							trans.anchorMin = SetValue(trans.anchorMin, value);
							break;
						case StateCtrlRectTransType.ANCHOR_MAX:
							trans.anchorMax = SetValue(trans.anchorMax, value);
							break;
						case StateCtrlRectTransType.ANCHORED_POSITION:
							trans.anchoredPosition = SetValue(trans.anchoredPosition, value);
							break;
						case StateCtrlRectTransType.SIZE_DELTA:
							trans.sizeDelta = SetValue(trans.sizeDelta, value);
							break;
						case StateCtrlRectTransType.PIVOT:
							trans.pivot = SetValue(trans.pivot, value);
							break;
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
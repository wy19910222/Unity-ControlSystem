/*
 * @Author: wangyun
 * @CreateTime: 2022-04-18 21:27:57 579
 * @LastEditor: wangyun
 * @EditTime: 2022-06-26 00:28:43 884
 */

using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	public enum ProgressCtrlRectTransType {
		NONE,
		ANCHOR_MIN = 1,
		ANCHOR_MAX = 2,
		ANCHORED_POSITION = 3,
		SIZE_DELTA = 4,
		PIVOT = 5
	}

	[RequireComponent(typeof(RectTransform))]
	public class ProgressCtrlRectTrans : BaseProgressCtrlVector2 {
		public ProgressCtrlRectTransType type;
		
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
		
		protected override bool PartCtrl => false;

		protected override Vector2 TargetValue {
			get {
				var trans = transform as RectTransform;
				switch (type) {
					case ProgressCtrlRectTransType.ANCHOR_MIN:
						return trans.anchorMin;
					case ProgressCtrlRectTransType.ANCHOR_MAX:
						return trans.anchorMax;
					case ProgressCtrlRectTransType.ANCHORED_POSITION:
						return trans.anchoredPosition;
					case ProgressCtrlRectTransType.SIZE_DELTA:
						return trans.sizeDelta;
					case ProgressCtrlRectTransType.PIVOT:
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
						case ProgressCtrlRectTransType.ANCHOR_MIN:
							m_Tweener = DOTween.To(() => trans.anchorMin, _value => trans.anchorMin = _value, value, tweenDuration);
							break;
						case ProgressCtrlRectTransType.ANCHOR_MAX:
							m_Tweener = DOTween.To(() => trans.anchorMax, _value => trans.anchorMax = _value, value, tweenDuration);
							break;
						case ProgressCtrlRectTransType.ANCHORED_POSITION:
							m_Tweener = DOTween.To(() => trans.anchoredPosition, _value => trans.anchoredPosition = _value, value, tweenDuration);
							break;
						case ProgressCtrlRectTransType.SIZE_DELTA:
							m_Tweener = DOTween.To(() => trans.sizeDelta, _value => trans.sizeDelta = _value, value, tweenDuration);
							break;
						case ProgressCtrlRectTransType.PIVOT:
							m_Tweener = DOTween.To(() => trans.pivot, _value => trans.pivot = _value, value, tweenDuration);
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
						case ProgressCtrlRectTransType.ANCHOR_MIN:
							trans.anchorMin = value;
							break;
						case ProgressCtrlRectTransType.ANCHOR_MAX:
							trans.anchorMax = value;
							break;
						case ProgressCtrlRectTransType.ANCHORED_POSITION:
							trans.anchoredPosition = value;
							break;
						case ProgressCtrlRectTransType.SIZE_DELTA:
							trans.sizeDelta = value;
							break;
						case ProgressCtrlRectTransType.PIVOT:
							trans.pivot = value;
							break;
					}
				}
			}
		}
	}
}
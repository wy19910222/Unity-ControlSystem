/*
 * @Author: wangyun
 * @CreateTime: 2022-07-15 17:09:45 067
 * @LastEditor: wangyun
 * @EditTime: 2022-07-15 17:09:45 057
 */

using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	public enum ProgressCtrlRectTransLerpType {
		NONE,
		ANCHOR_MIN = 1,
		ANCHOR_MAX = 2,
		ANCHORED_POSITION = 3,
		SIZE_DELTA = 4,
		PIVOT = 5
	}

	[RequireComponent(typeof(RectTransform))]
	public class ProgressCtrlRectTransLerp : BaseProgressCtrlFloat {
		public ProgressCtrlRectTransLerpType type;
		public RectTransform fromTarget;
		public RectTransform toTarget;
		public float m_LerpValue;
		
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

		protected override float TargetValue {
			get {
				return m_LerpValue;
			}
			set {
				m_LerpValue = value;
				if (m_Tweener != null) {
					m_Tweener.Kill();
					m_Tweener = null;
				}
#if UNITY_EDITOR
				if (tween && !controller.InvalidateTween && Application.isPlaying) {
#else
				if (tween && !controller.InvalidateTween) {
#endif
					if (fromTarget && toTarget) {
						var trans = transform as RectTransform;
						switch (type) {
							case ProgressCtrlRectTransLerpType.ANCHOR_MIN:
								m_Tweener = DOTween.To(() => trans.anchorMin, _value => trans.anchorMin = _value,
											Vector2.LerpUnclamped(fromTarget.anchorMin, toTarget.anchorMin, m_LerpValue), tweenDuration);
								break;
							case ProgressCtrlRectTransLerpType.ANCHOR_MAX:
								m_Tweener = DOTween.To(() => trans.anchorMax, _value => trans.anchorMax = _value,
											Vector2.LerpUnclamped(fromTarget.anchorMax, toTarget.anchorMax, m_LerpValue), tweenDuration);
								break;
							case ProgressCtrlRectTransLerpType.ANCHORED_POSITION:
								m_Tweener = DOTween.To(() => trans.anchoredPosition, _value => trans.anchoredPosition = _value,
											Vector2.LerpUnclamped(fromTarget.anchoredPosition, toTarget.anchoredPosition, m_LerpValue), tweenDuration);
								break;
							case ProgressCtrlRectTransLerpType.SIZE_DELTA:
								m_Tweener = DOTween.To(() => trans.sizeDelta, _value => trans.sizeDelta = _value,
											Vector2.LerpUnclamped(fromTarget.sizeDelta, toTarget.sizeDelta, m_LerpValue), tweenDuration);
								break;
							case ProgressCtrlRectTransLerpType.PIVOT:
								m_Tweener = DOTween.To(() => trans.pivot, _value => trans.pivot = _value,
											Vector2.LerpUnclamped(fromTarget.pivot, toTarget.pivot, m_LerpValue), tweenDuration);
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
					if (fromTarget && toTarget) {
						var trans = transform as RectTransform;
						switch (type) {
							case ProgressCtrlRectTransLerpType.ANCHOR_MIN:
								trans.anchorMin = Vector2.LerpUnclamped(fromTarget.anchorMin, toTarget.anchorMin, m_LerpValue);
								break;
							case ProgressCtrlRectTransLerpType.ANCHOR_MAX:
								trans.anchorMax = Vector2.LerpUnclamped(fromTarget.anchorMax, toTarget.anchorMax, m_LerpValue);
								break;
							case ProgressCtrlRectTransLerpType.ANCHORED_POSITION:
								trans.anchoredPosition = Vector2.LerpUnclamped(fromTarget.anchoredPosition, toTarget.anchoredPosition, m_LerpValue);
								break;
							case ProgressCtrlRectTransLerpType.SIZE_DELTA:
								trans.sizeDelta = Vector2.LerpUnclamped(fromTarget.sizeDelta, toTarget.sizeDelta, m_LerpValue);
								break;
							case ProgressCtrlRectTransLerpType.PIVOT:
								trans.pivot = Vector2.LerpUnclamped(fromTarget.pivot, toTarget.pivot, m_LerpValue);
								break;
						}
					}
				}
			}
		}

		private void OnValidate() {
			TargetValue = m_LerpValue;
		}
	}
}
/*
 * @Author: wangyun
 * @CreateTime: 2022-07-15 17:15:00 532
 * @LastEditor: wangyun
 * @EditTime: 2022-07-15 17:15:00 527
 */

using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	public enum ProgressCtrlTransTargetType {
		NONE,
		POSITION = 1,
		ANGLES = 2,
		LOCAL_SCALE = 3
	}

	public class ProgressCtrlTransTarget : BaseProgressCtrlFloat {
		public ProgressCtrlTransTargetType type = ProgressCtrlTransTargetType.POSITION;
		public Transform fromTarget;
		public Transform toTarget;
		[ShowIf("@this.type == ProgressCtrlTransTargetType.POSITION")]
		public Vector3 ignoreDirection;
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
			get => m_LerpValue;
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
						var trans = transform;
						switch (type) {
							case ProgressCtrlTransTargetType.POSITION:
								m_Tweener = DOTween.To(
										() => trans.position,
										_value => {
											if (ignoreDirection == Vector3.zero) {
												trans.position = _value;
											} else {
												Transform parent = trans.parent;
												Vector3 ignoreGlobalDirection = parent ? parent.TransformDirection(ignoreDirection) : ignoreDirection;
												Vector3 currentPos = trans.position;
												currentPos += Vector3.ProjectOnPlane(_value - currentPos, ignoreGlobalDirection);
												trans.position = currentPos;
											}
										},
										Vector3.LerpUnclamped(fromTarget.position, toTarget.position, m_LerpValue),
										tweenDuration
								);
								break;
							case ProgressCtrlTransTargetType.ANGLES:
								m_Tweener = DOTween.To(() => trans.rotation, _value => trans.rotation = _value,
											Quaternion.LerpUnclamped(fromTarget.rotation, toTarget.rotation, m_LerpValue).eulerAngles, tweenDuration);
								break;
							case ProgressCtrlTransTargetType.LOCAL_SCALE:
								m_Tweener = DOTween.To(() => trans.localScale, _value => trans.localScale = _value,
											Vector3.LerpUnclamped(fromTarget.localScale, toTarget.localScale, m_LerpValue), tweenDuration);
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
						var trans = transform;
						switch (type) {
							case ProgressCtrlTransTargetType.POSITION:
								Vector3 position = Vector3.LerpUnclamped(fromTarget.position, toTarget.position, m_LerpValue);
								if (ignoreDirection == Vector3.zero) {
									trans.position = position;
								} else {
									Transform parent = trans.parent;
									Vector3 ignoreGlobalDirection = parent ? parent.TransformDirection(ignoreDirection) : ignoreDirection;
									Vector3 currentPos = trans.position;
									currentPos += Vector3.ProjectOnPlane(position - currentPos, ignoreGlobalDirection);
									trans.position = currentPos;
								}
								break;
							case ProgressCtrlTransTargetType.ANGLES:
								trans.rotation = Quaternion.LerpUnclamped(fromTarget.rotation, toTarget.rotation, m_LerpValue);
								break;
							case ProgressCtrlTransTargetType.LOCAL_SCALE:
								trans.localScale = Vector3.LerpUnclamped(fromTarget.localScale, toTarget.localScale, m_LerpValue);
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
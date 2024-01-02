/*
 * @Author: wangyun
 * @CreateTime: 2022-04-18 14:22:51 442
 * @LastEditor: wangyun
 * @EditTime: 2022-06-26 00:36:50 559
 */

using System;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	[Obsolete("ProgressCtrlTransFloat has been deprecated. Use ProgressCtrlTrans instead")]
	public enum ProgressCtrlTransPart {
		X = 1 << 0,
		Y = 1 << 1,
		Z = 1 << 2
	}

	[Obsolete("ProgressCtrlTransFloat has been deprecated. Use ProgressCtrlTrans instead")]
	[AddComponentMenu("")]
	public class ProgressCtrlTransFloat : BaseProgressCtrlFloat {
		public ProgressCtrlTransType type = ProgressCtrlTransType.LOCAL_POSITION;
		public ProgressCtrlTransPart part = ProgressCtrlTransPart.X;
		
		public bool tween;
		[HideIf("@!this.tween")]
		public float tweenDelay;
		[HideIf("@!this.tween")]
		public float tweenDuration = 0.3F;
		[HideIf("@!this.tween")]
		public Ease tweenEase = Ease.OutQuad;

		private Tweener m_Tweener;

		protected override float TargetValue {
			get {
				var trans = transform;
				switch (type) {
					case ProgressCtrlTransType.LOCAL_POSITION:
						return GetValue(trans.localPosition);
					case ProgressCtrlTransType.LOCAL_ANGLES:
						return GetValue(trans.localEulerAngles);
					case ProgressCtrlTransType.LOCAL_SCALE:
						return GetValue(trans.localScale);
				}
				return 0;
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
					switch (type) {
						case ProgressCtrlTransType.LOCAL_POSITION:
							switch (part) {
								case ProgressCtrlTransPart.X: {
									m_Tweener = transform.DOLocalMoveX(value, tweenDuration).SetDelay(tweenDelay).SetEase(tweenEase);
									m_Tweener.OnComplete(() => m_Tweener = null);
									break;
								}
								case ProgressCtrlTransPart.Y: {
									m_Tweener = transform.DOLocalMoveY(value, tweenDuration).SetDelay(tweenDelay).SetEase(tweenEase);
									m_Tweener.OnComplete(() => m_Tweener = null);
									break;
								}
								case ProgressCtrlTransPart.Z: {
									m_Tweener = transform.DOLocalMoveZ(value, tweenDuration).SetDelay(tweenDelay).SetEase(tweenEase);
									m_Tweener.OnComplete(() => m_Tweener = null);
									break;
								}
							}
							break;
						case ProgressCtrlTransType.LOCAL_ANGLES: {
							var trans = transform;
							m_Tweener = DOTween.To(
									() => GetValue(trans.localEulerAngles),
									v => trans.localEulerAngles = SetValue(trans.localEulerAngles, v),
									value,
									tweenDuration
							).SetDelay(tweenDelay).SetEase(tweenEase);
							m_Tweener.OnComplete(() => m_Tweener = null);
							break;
						}
						case ProgressCtrlTransType.LOCAL_SCALE:
							switch (part) {
								case ProgressCtrlTransPart.X: {
									m_Tweener = transform.DOScaleX(value, tweenDuration).SetDelay(tweenDelay).SetEase(tweenEase);
									m_Tweener.OnComplete(() => m_Tweener = null);
									break;
								}
								case ProgressCtrlTransPart.Y: {
									m_Tweener = transform.DOScaleY(value, tweenDuration).SetDelay(tweenDelay).SetEase(tweenEase);
									m_Tweener.OnComplete(() => m_Tweener = null);
									break;
								}
								case ProgressCtrlTransPart.Z: {
									m_Tweener = transform.DOScaleZ(value, tweenDuration).SetDelay(tweenDelay).SetEase(tweenEase);
									m_Tweener.OnComplete(() => m_Tweener = null);
									break;
								}
							}
							break;
					}
				} else {
					switch (type) {
						case ProgressCtrlTransType.LOCAL_POSITION:
							transform.localPosition = SetValue(transform.localPosition, value);
							break;
						case ProgressCtrlTransType.LOCAL_ANGLES:
							transform.localEulerAngles = SetValue(transform.localEulerAngles, value);
							break;
						case ProgressCtrlTransType.LOCAL_SCALE:
							transform.localScale = SetValue(transform.localScale, value);
							break;
					}
				}
			}
		}

		private float GetValue(Vector3 v3) {
			switch (part) {
				case ProgressCtrlTransPart.X:
					return v3.x;
				case ProgressCtrlTransPart.Y:
					return v3.y;
				case ProgressCtrlTransPart.Z:
					return v3.z;
				default:
					return 0;
			}
		}
		
		private Vector3 SetValue(Vector3 v3, float value) {
			switch (part) {
				case ProgressCtrlTransPart.X:
					v3.x = value;
					break;
				case ProgressCtrlTransPart.Y:
					v3.y = value;
					break;
				case ProgressCtrlTransPart.Z:
					v3.z = value;
					break;
			}
			return v3;
		}
	}
}
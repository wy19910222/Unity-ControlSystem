/*
 * @Author: wangyun
 * @CreateTime: 2022-06-20 00:39:56 945
 * @LastEditor: wangyun
 * @EditTime: 2022-08-03 18:08:26 765
 */

using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	public enum ProgressCtrlTransType {
		NONE,
		LOCAL_POSITION = 1,
		LOCAL_ANGLES = 2,
		LOCAL_SCALE = 3
	}
	
	public class ProgressCtrlTrans : BaseProgressCtrlVector3 {
		public ProgressCtrlTransType type = ProgressCtrlTransType.LOCAL_POSITION;
		[HideIf("@this.type != ProgressCtrlTransType.LOCAL_ANGLES")]
		[InfoBox("缓动时每帧会调用反射，少用")]
		public bool useRotationOrder;
		
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

		protected override Vector3 TargetValue {
			get {
				var trans = transform;
				switch (type) {
					case ProgressCtrlTransType.LOCAL_POSITION:
						return trans.localPosition;
					case ProgressCtrlTransType.LOCAL_ANGLES:
#if UNITY_EDITOR
						const System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
						System.Type typeOfTrans = typeof(Transform);
						System.Reflection.PropertyInfo pi = typeOfTrans.GetProperty("rotationOrder", flags);
						object rotationOrder = pi?.GetValue(transform, null);
						System.Reflection.MethodInfo mi = typeOfTrans.GetMethod("GetLocalEulerAngles", flags);
						object value = mi?.Invoke(transform, new [] { rotationOrder });
						return value is Vector3 localEulerAngles ? localEulerAngles : Vector3.zero;
#else
						return trans.localEulerAngles;
#endif
					case ProgressCtrlTransType.LOCAL_SCALE:
						return trans.localScale;
				}
				return Vector3.zero;
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
					if (type == ProgressCtrlTransType.LOCAL_ANGLES) {
						var trans = transform;
						if (useRotationOrder) {
							m_Tweener = DOTween.To(
								() => GetLocalEulerAngles(trans),
								v => trans.localEulerAngles = SetValue(GetLocalEulerAngles(trans), v),
								value,
								tweenDuration
							);
						} else {
							if (part == Vector3Part.XYZ) {
								Quaternion rotationFrom = trans.localRotation;
								Quaternion rotationTo = Quaternion.Euler(value);
								float temp = 0;
								m_Tweener = DOTween.To(
									() => temp,
									v => {
										trans.localRotation = Quaternion.LerpUnclamped(rotationFrom, rotationTo, v);
										temp = v;
									},
									1,
									tweenDuration
								);
							} else {
								m_Tweener = DOTween.To(
									() => trans.localRotation,
									v => trans.localEulerAngles = SetValue(trans.localEulerAngles, v.eulerAngles),
									value,
									tweenDuration
								);
							}
						}
					} else {
						m_Tweener = DOTween.To(
							() => Value,
							v => Value = v,
							value,
							tweenDuration
						);
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
					Value = value;
				}
			}
		}

		private Vector3 Value {
			get {
				var trans = transform;
				switch (type) {
					case ProgressCtrlTransType.LOCAL_POSITION:
						return trans.localPosition;
					case ProgressCtrlTransType.LOCAL_ANGLES:
#if UNITY_EDITOR
						return GetLocalEulerAngles(trans);
#else
						return trans.localEulerAngles;
#endif
					case ProgressCtrlTransType.LOCAL_SCALE:
						return trans.localScale;
				}
				return Vector3.zero;
			}
			set {
				switch (type) {
					case ProgressCtrlTransType.LOCAL_POSITION:
						transform.localPosition = SetValue(transform.localPosition, value);
						break;
					case ProgressCtrlTransType.LOCAL_ANGLES:
						var localEulerAngles = useRotationOrder ? GetLocalEulerAngles(transform) : transform.localEulerAngles;
						transform.localEulerAngles = SetValue(localEulerAngles, value);
						break;
					case ProgressCtrlTransType.LOCAL_SCALE:
						transform.localScale = SetValue(transform.localScale, value);
						break;
				}
			}
		}
		
		private Vector3 SetValue(Vector3 v, Vector3 value) {
			if ((part & Vector3Part.X) != 0) {
				v.x = value.x;
			}
			if ((part & Vector3Part.Y) != 0) {
				v.y = value.y;
			}
			if ((part & Vector3Part.Z) != 0) {
				v.z = value.z;
			}
			return v;
		}
		
		private static bool s_IsInit;
		private static System.Reflection.PropertyInfo s_RotationOrderPI;
		private static System.Reflection.MethodInfo s_GetLocalEulerAnglesMI;
		private static Vector3 GetLocalEulerAngles(Transform trans) {
			if (!s_IsInit) {
				const System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
				System.Type typeOfTrans = typeof(Transform);
				s_RotationOrderPI = typeOfTrans.GetProperty("rotationOrder", flags);
				s_GetLocalEulerAnglesMI = typeOfTrans.GetMethod("GetLocalEulerAngles", flags);
				s_IsInit = true;
			}
			object rotationOrder = s_RotationOrderPI?.GetValue(trans, null);
			object value = s_GetLocalEulerAnglesMI?.Invoke(trans, new [] { rotationOrder });
			return value is Vector3 localEulerAngles ? localEulerAngles : Vector3.zero;
		}
	}
}
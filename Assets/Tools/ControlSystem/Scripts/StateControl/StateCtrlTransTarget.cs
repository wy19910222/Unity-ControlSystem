/*
 * @Author: wangyun
 * @CreateTime: 2022-07-15 15:04:53 991
 * @LastEditor: wangyun
 * @EditTime: 2022-07-15 15:04:53 983
 */

using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	public enum StateCtrlTransTargetType {
		NONE,
		POSITION = 1,
		ANGLES = 2,
		LOCAL_SCALE = 3
	}

	public class StateCtrlTransTarget : BaseStateCtrl<Transform> {
		public StateCtrlTransTargetType type;
		[Tooltip("以local轴拆分")]
		public Vector3Part part = Vector3Part.XYZ;
		public Transform target;
		[HideIf("@this.type != StateCtrlTransTargetType.ANGLES || !this.tween && this.part == Vector3Part.XYZ")]
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

		protected override Transform TargetValue {
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
						var trans = transform;
						if (type == StateCtrlTransTargetType.ANGLES) {
							if (useRotationOrder) {
								Transform parent = trans.parent;
								Transform _temp = new GameObject().transform;
								_temp.rotation = target.rotation;
								_temp.SetParent(parent);
								Vector3 targetLocalEulerAngles = GetLocalEulerAngles(_temp);
								DestroyImmediate(_temp.gameObject);
								m_Tweener = DOTween.To(
									() => GetLocalEulerAngles(trans),
									v => trans.localEulerAngles = SetValue(GetLocalEulerAngles(trans), v),
									targetLocalEulerAngles,
									tweenDuration
								);
							} else {
								if (part == Vector3Part.XYZ) {
									Quaternion rotationFrom = trans.rotation;
									Quaternion rotationTo = target.rotation;
									float temp = 0;
									m_Tweener = DOTween.To(
										() => temp,
										v => {
											trans.rotation = Quaternion.LerpUnclamped(rotationFrom, rotationTo, v);
											temp = v;
										},
										1,
										tweenDuration
									);
								} else {
									Transform parent = trans.parent;
									Quaternion targetLocalRotation = parent ? target.rotation * Quaternion.Inverse(parent.rotation) : target.rotation;
									m_Tweener = DOTween.To(
										() => trans.localRotation,
										v => trans.localEulerAngles = SetValue(trans.localEulerAngles, v.eulerAngles),
										targetLocalRotation.eulerAngles,
										tweenDuration
									);
								}
							}
						} else {
							switch (type) {
								case StateCtrlTransTargetType.POSITION:
									Transform parent = trans.parent;
									Vector3 targetLocalPosition = parent ? parent.InverseTransformPoint(target.position) : target.position;
									m_Tweener = DOTween.To(
										() => trans.localPosition,
										v => trans.localPosition = SetValue(trans.localPosition, v),
										targetLocalPosition,
										tweenDuration
									);
									break;
								case StateCtrlTransTargetType.LOCAL_SCALE:
									m_Tweener = DOTween.To(
										() => trans.localScale,
										v => trans.localScale = SetValue(trans.localScale, v),
										target.localScale,
										tweenDuration
									);
									break;
							}
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
						if (part == Vector3Part.XYZ) {
							switch (type) {
								case StateCtrlTransTargetType.POSITION:
									transform.position = target.position;
									break;
								case StateCtrlTransTargetType.ANGLES:
									transform.rotation = target.rotation;
									break;
								case StateCtrlTransTargetType.LOCAL_SCALE:
									transform.localScale = target.localScale;
									break;
							}
						} else {
							switch (type) {
								case StateCtrlTransTargetType.POSITION: {
									Transform trans = transform;
									Transform parent = trans.parent;
									Vector3 targetLocalPosition = parent ? parent.InverseTransformPoint(target.position) : target.position;
									trans.localPosition = SetValue(trans.localPosition, targetLocalPosition);
									break;
								}
								case StateCtrlTransTargetType.ANGLES: {
									Transform trans = transform;
									Transform parent = trans.parent;
									if (useRotationOrder) {
										Transform _temp = new GameObject().transform;
										_temp.rotation = target.rotation;
										_temp.SetParent(parent);
										Vector3 targetLocalEulerAngles = GetLocalEulerAngles(_temp);
										DestroyImmediate(_temp.gameObject);
										transform.localEulerAngles = SetValue(GetLocalEulerAngles(trans), targetLocalEulerAngles);
									} else {
										Quaternion targetLocalRotation = parent ? target.rotation * Quaternion.Inverse(parent.rotation) : target.rotation;
										transform.localEulerAngles = SetValue(transform.localEulerAngles, targetLocalRotation.eulerAngles);
									}
									break;
								}
								case StateCtrlTransTargetType.LOCAL_SCALE: {
									transform.localScale = SetValue(transform.localScale, target.localScale);
									break;
								}
							}
						}
					}
				}
			}
		}

		private Vector3 SetValue(Vector3 v3, Vector3 value) {
			if ((part & Vector3Part.X) != 0) {
				v3.x = value.x;
			}
			if ((part & Vector3Part.Y) != 0) {
				v3.y = value.y;
			}
			if ((part & Vector3Part.Z) != 0) {
				v3.z = value.z;
			}
			return v3;
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
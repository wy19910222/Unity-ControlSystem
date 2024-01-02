/*
 * @Author: wangyun
 * @CreateTime: 2022-04-18 14:22:34 542
 * @LastEditor: wangyun
 * @EditTime: 2022-09-26 03:21:17 112
 */

using System;
using UnityEngine;

namespace Control {
	public abstract class BaseProgressCtrl : MonoBehaviour {
		[ProgressControllerSelect]
		public ProgressController controller;
		public abstract void Capture(float progress);
		public abstract void Apply(float progress);
	}

	public abstract class BaseProgressCtrl<TValue> : BaseProgressCtrl {
		[SerializeField, LimitedEditable]
		private TValue m_FromValue;
		[SerializeField, LimitedEditable]
		private TValue m_ToValue;
		[SerializeField, CanResetCurve]
		protected AnimationCurve m_Curve = new AnimationCurve(new Keyframe(0, 0, 1, 1), new Keyframe(1, 1, 1, 1));

		protected virtual void Reset() {
			controller = GetComponentInParent<ProgressController>();
			m_ToValue = m_FromValue = TargetValue;
		}

		private TValue GetValue(float progress) {
			var t = m_Curve.Evaluate(progress);
			return Lerp(m_FromValue, m_ToValue, t);
		}

		private void SetValue(float progress, TValue value) {
			var t = GetT(m_FromValue, m_ToValue, value);
			var curveKeys = m_Curve.keys;
			for (int index = 0, length = curveKeys.Length; index < length; index++) {
				var curveKey = curveKeys[index];
				if (Mathf.Abs(curveKey.time - progress) < Mathf.Epsilon) {
					m_Curve.RemoveKey(index);
					break;
				}
			}
			AddKey(progress, t);
		}

		protected virtual void AddKey(float progress, float t) {
			m_Curve.AddKey(progress, t);
		}

		protected virtual TValue TargetValue { get; set; }

		protected abstract TValue Lerp(TValue from, TValue to, float t);
		protected abstract float GetT(TValue from, TValue to, TValue value);
		protected abstract bool Equals(TValue value1, TValue value2);

		public override void Capture(float progress) {
			TValue targetValue = TargetValue;
			// 如果form和to相同，则把to设置成新记录的值，然后曲线上所有点的纵坐标都设置为0
			if (Equals(m_FromValue, m_ToValue)) {
				m_ToValue = targetValue;
				var curveKeys = m_Curve.keys;
				for (int index = 0, length = curveKeys.Length; index < length; index++) {
					var curveKey = curveKeys[index];
					m_Curve.MoveKey(index, new Keyframe(curveKey.time, 0));
				}
			}
			SetValue(progress, targetValue);
#if UNITY_EDITOR
			// 缩放curve
			ScaleCurve();
#endif
		}

		protected void ScaleCurve() {
			// 缩放到最低点为0，最高点为1
			var curveKeys = m_Curve.keys;
			var curveLength = curveKeys.Length;
			if (curveLength > 0) {
				if (curveLength == 1) {
					var keyFrame = curveKeys[0];
					if (keyFrame.time != 0 || keyFrame.value != 0) {
						// 把唯一的点重置成(0, 0)
						m_FromValue = Lerp(m_FromValue, m_ToValue, keyFrame.value);
						m_Curve.RemoveKey(0);
						m_Curve.AddKey(0, 0);
					}
				} else {
					// 找到最低点和最高点
					float minT = float.MaxValue, maxT = float.MinValue;
					foreach (var curveKey in curveKeys) {
						if (curveKey.value < minT) {
							minT = curveKey.value;
						}
						if (curveKey.value > maxT) {
							maxT = curveKey.value;
						}
					}
					// 缩放到最低点为0，最高点为1
					m_Curve = new AnimationCurve();
					float deltaT = maxT - minT;
					if (deltaT < Mathf.Epsilon) {
						foreach (var curveKey in curveKeys) {
							m_Curve.AddKey(new Keyframe(curveKey.time, 0));
						}
					} else {
						foreach (var curveKey in curveKeys) {
							m_Curve.AddKey(new Keyframe(
									curveKey.time,
									(curveKey.value - minT) / deltaT,
									curveKey.inTangent / deltaT,
									curveKey.outTangent / deltaT
							));
						}
					}
					(m_FromValue, m_ToValue) = (Lerp(m_FromValue, m_ToValue, minT), Lerp(m_FromValue, m_ToValue, maxT));
				}
			}
		}

		public override void Apply(float progress) {
#if UNITY_EDITOR
			UnityEditor.Undo.RegisterFullObjectHierarchyUndo(this, "Apply");
#endif
			TargetValue = GetValue(progress);
		}
	}
	
	public abstract class BaseProgressCtrlFloat : BaseProgressCtrl<float> {
		protected override float Lerp(float from, float to, float t) {
			return from + t * (to - from);
		}

		protected override float GetT(float from, float to, float value) {
			var delta = to - from;
			return delta == 0 ? 0 : (value - from) / delta;
		}

		protected override bool Equals(float value1, float value2) {
			return Mathf.Abs(value1 - value2) < Mathf.Epsilon;
		}

		protected override void AddKey(float progress, float t) {
			float curT = m_Curve.Evaluate(progress);
			if (Mathf.Abs(curT - t) < Mathf.Epsilon) {
				const float deltaProgress = 0.001F;
				float tangent = (t - m_Curve.Evaluate(progress - deltaProgress)) / deltaProgress;
				m_Curve.AddKey(new Keyframe(progress, t, tangent, tangent));
			} else {
				m_Curve.AddKey(progress, t);
#if UNITY_EDITOR
				// Clamped auto
				for (int i = 1, length = m_Curve.length - 1; i < length; ++i) {
					UnityEditor.AnimationUtility.SetKeyLeftTangentMode(m_Curve, i, UnityEditor.AnimationUtility.TangentMode.ClampedAuto);
					UnityEditor.AnimationUtility.SetKeyRightTangentMode(m_Curve, i, UnityEditor.AnimationUtility.TangentMode.ClampedAuto);
				}
#endif
			}
		}
	}
	
	public abstract class BaseProgressCtrlDouble : BaseProgressCtrl<double> {
		protected override double Lerp(double from, double to, float t) {
			return from + t * (to - from);
		}

		protected override float GetT(double from, double to, double value) {
			var delta = to - from;
			return delta == 0 ? 0 : (float) ((value - from) / delta);
		}

		protected override bool Equals(double value1, double value2) {
			double delta = value1 - value2;
			return delta < Mathf.Epsilon && delta > -Mathf.Epsilon;
		}

		protected override void AddKey(float progress, float t) {
			float curT = m_Curve.Evaluate(progress);
			if (Mathf.Abs(curT - t) < Mathf.Epsilon) {
				const float deltaProgress = 0.001F;
				float tangent = (t - m_Curve.Evaluate(progress - deltaProgress)) / deltaProgress;
				m_Curve.AddKey(new Keyframe(progress, t, tangent, tangent));
			} else {
				m_Curve.AddKey(progress, t);
#if UNITY_EDITOR
				// Clamped auto
				for (int i = 1, length = m_Curve.length - 1; i < length; ++i) {
					UnityEditor.AnimationUtility.SetKeyLeftTangentMode(m_Curve, i, UnityEditor.AnimationUtility.TangentMode.ClampedAuto);
					UnityEditor.AnimationUtility.SetKeyRightTangentMode(m_Curve, i, UnityEditor.AnimationUtility.TangentMode.ClampedAuto);
				}
#endif
			}
		}
	}
	
	public abstract class BaseProgressCtrlInt : BaseProgressCtrl<int> {
		protected override int Lerp(int from, int to, float t) {
			return Mathf.RoundToInt(from + t * (to - from));
		}

		protected override float GetT(int from, int to, int value) {
			var delta = to - from;
			return delta == 0 ? 0 : (float) (value - from) / delta;
		}

		protected override bool Equals(int value1, int value2) {
			return value1 == value2;
		}

		protected override void AddKey(float progress, float t) {
			float curT = m_Curve.Evaluate(progress);
			if (Mathf.Abs(curT - t) < Mathf.Epsilon) {
				const float deltaProgress = 0.001F;
				float tangent = (t - m_Curve.Evaluate(progress - deltaProgress)) / deltaProgress;
				m_Curve.AddKey(new Keyframe(progress, t, tangent, tangent));
			} else {
				var curveKeys = m_Curve.keys;
				var curveLength = curveKeys.Length;
				var index = curveLength;
				for (int i = 0; i < curveLength; i++) {
					if (progress < curveKeys[i].time) {
						index = i;
						break;
					}
				}
				if (index > 0) {
					var prev = curveKeys[index - 1];
					var inTangent = (t - prev.value) / (progress - prev.time);
					if (index < curveLength) {
						var next = curveKeys[index];
						var outTangent = (t - next.value) / (progress - next.time);
						m_Curve.AddKey(new Keyframe(progress, t, inTangent, outTangent));
						m_Curve.MoveKey(index - 1, new Keyframe(prev.time, prev.value, prev.inTangent, inTangent));
						m_Curve.MoveKey(index + 1, new Keyframe(next.time, next.value, outTangent, next.outTangent));
					} else {
						m_Curve.AddKey(new Keyframe(progress, t, inTangent, inTangent));
						m_Curve.MoveKey(index - 1, new Keyframe(prev.time, prev.value, prev.inTangent, inTangent));
					}
				} else {
					if (index < curveLength) {
						var next = curveKeys[index];
						var outTangent = (t - next.value) / (progress - next.time);
						m_Curve.AddKey(new Keyframe(progress, t, outTangent, outTangent));
						m_Curve.MoveKey(index + 1, new Keyframe(next.time, next.value, outTangent, next.outTangent));
					} else {
						m_Curve.AddKey(progress, t);
					}
				}
			}
		}
	}
	
	public abstract class BaseProgressCtrlConst<TValue> : BaseProgressCtrl<TValue> where TValue : IEquatable<TValue>  {
		protected override TValue Lerp(TValue from, TValue to, float t) {
			return t >= 1 ? to : from;
		}

		protected override float GetT(TValue from, TValue to, TValue value) {
			return object.Equals(value, to) && !object.Equals(value, from) ? 1 : 0;
		}

		protected override bool Equals(TValue value1, TValue value2) {
			return object.Equals(value1, value2);
		}

		protected override void AddKey(float progress, float t) {
			m_Curve.AddKey(new Keyframe(progress, t, Mathf.Infinity, Mathf.Infinity));
		}
	}
	
	public abstract class BaseProgressCtrlFloats<TValue> : BaseProgressCtrl where TValue : IEquatable<TValue> {
		[SerializeField, LimitedEditable]
		protected TValue m_FromValue;
		[SerializeField, LimitedEditable]
		protected TValue m_ToValue;
		// 为了固定Curve数量，故在子类声明
		// [SerializeField, CanResetCurve]
		// protected AnimationCurve[] m_Curves = Array.Empty<AnimationCurve>();

		private void Reset() {
			controller = GetComponentInParent<ProgressController>();
			m_ToValue = m_FromValue = TargetValue;
		}
		
		protected abstract int PartCount { get; }

		protected abstract float GetValuePart(TValue value, int part);
		protected abstract TValue SetValuePart(TValue value, int part, float valuePart);

		protected abstract AnimationCurve GetCurve(int part);
		protected abstract void SetCurve(int part, AnimationCurve curve);

		private TValue GetValue(float progress) {
			var ret = default(TValue);
			for (int part = 0; part < PartCount; part++) {
				var t = GetCurve(part).Evaluate(progress);
				var valuePart = Lerp(GetValuePart(m_FromValue, part), GetValuePart(m_ToValue, part), t);
				ret = SetValuePart(ret, part, valuePart);
			}
			return ret;
		}
		private void SetValue(float progress, TValue value) {
			for (int part = 0; part < PartCount; part++) {
				var t = GetT(GetValuePart(m_FromValue, part), GetValuePart(m_ToValue, part), GetValuePart(value, part));
				var curve = GetCurve(part);
				var curveKeys = curve.keys;
				for (int index = 0, length = curveKeys.Length; index < length; index++) {
					var curveKey = curveKeys[index];
					if (Mathf.Abs(curveKey.time - progress) < Mathf.Epsilon) {
						curve.RemoveKey(index);
						break;
					}
				}
				AddKey(curve, progress, t);
			}
		}
		protected void AddKey(AnimationCurve curve, float progress, float t) {
			float curT = curve.Evaluate(progress);
			if (Mathf.Abs(curT - t) < Mathf.Epsilon) {
				float deltaProgress = progress < 0.5F ? 0.001F : -0.001F;
				float tangent = (curve.Evaluate(progress + deltaProgress) - t) / deltaProgress;
				curve.AddKey(new Keyframe(progress, t, tangent, tangent));
			} else {
				curve.AddKey(progress, t);
#if UNITY_EDITOR
				// Clamped auto
				for (int i = 1, length = curve.length - 1; i < length; ++i) {
					UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, i, UnityEditor.AnimationUtility.TangentMode.ClampedAuto);
					UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, i, UnityEditor.AnimationUtility.TangentMode.ClampedAuto);
				}
#endif
			}
		}

		protected abstract TValue TargetValue { get; set; }

		protected float Lerp(float from, float to, float t) {
			return from + t * (to - from);
		}
		protected float GetT(float from, float to, float value) {
			var delta = to - from;
			return delta == 0 ? 0 : (value - from) / delta;
		}
		protected bool Equals(float value1, float value2) {
			return Mathf.Abs(value1 - value2) < Mathf.Epsilon;
		}

		public override void Capture(float progress) {
			TValue targetValue = TargetValue;
			// 如果form和to相同，则把to设置成新记录的值，然后曲线上所有点的纵坐标都设置为0
			for (int part = 0; part < PartCount; part++) {
				if (Equals(GetValuePart(m_FromValue, part), GetValuePart(m_ToValue, part))) {
					m_ToValue = SetValuePart(m_ToValue, part, GetValuePart(targetValue, part));
					AnimationCurve curve = GetCurve(part);
					var curveKeys = curve.keys;
					for (int index = 0, length = curveKeys.Length; index < length; index++) {
						var curveKey = curveKeys[index];
						curve.MoveKey(index, new Keyframe(curveKey.time, 0));
					}
				}
			}
			SetValue(progress, targetValue);
#if UNITY_EDITOR
			// 缩放curve
			for (int part = 0; part < PartCount; part++) {
				AnimationCurve curve = GetCurve(part);
				var curveKeys = curve.keys;
				var curveLength = curveKeys.Length;
				if (curveLength > 0) {
					if (curveLength == 1) {
						var keyFrame = curveKeys[0];
						if (keyFrame.time != 0 || keyFrame.value != 0) {
							// 把唯一的点重置成(0, 0)
							float from = GetValuePart(m_FromValue, part), to = GetValuePart(m_ToValue, part);
							m_FromValue = SetValuePart(m_FromValue, part, Lerp(from, to, keyFrame.value));
							curve.RemoveKey(0);
							curve.AddKey(0, 0);
						}
					} else {
						// 找到最低点和最高点
						float minT = float.MaxValue, maxT = float.MinValue;
						foreach (var curveKey in curveKeys) {
							if (curveKey.value < minT) {
								minT = curveKey.value;
							}
							if (curveKey.value > maxT) {
								maxT = curveKey.value;
							}
						}
						// 缩放到最低点为0，最高点为1
						SetCurve(part, curve = new AnimationCurve());
						float deltaT = maxT - minT;
						if (deltaT < Mathf.Epsilon) {
							foreach (var curveKey in curveKeys) {
								curve.AddKey(new Keyframe(curveKey.time, 0));
							}
						} else {
							foreach (var curveKey in curveKeys) {
								curve.AddKey(new Keyframe(
										curveKey.time,
										(curveKey.value - minT) / deltaT,
										curveKey.inTangent / deltaT,
										curveKey.outTangent / deltaT
								));
							}
						}
						float from = GetValuePart(m_FromValue, part), to = GetValuePart(m_ToValue, part);
						m_FromValue = SetValuePart(m_FromValue, part, Lerp(from, to, minT));
						m_ToValue = SetValuePart(m_ToValue, part, Lerp(from, to, maxT));
					}
				}
			}
#endif
		}

		public override void Apply(float progress) {
			TargetValue = GetValue(progress);
		}

		protected static void LinearTangent(AnimationCurve curve) {
			var curveKeys = curve.keys;
			var curveLength = curveKeys.Length;
			if (curveLength > 1) {
				for (int i = 0; i < curveLength; ++i) {
					var current = curveKeys[i];
					if (i > 0) {
						var prev = curveKeys[i - 1];
						float deltaTime = current.time - prev.time;
						float deltaValue = current.value - prev.value;
						current.inTangent = deltaTime < Mathf.Epsilon ? 0 : deltaValue / deltaTime;
						if (i == curveLength - 1) {
							current.outTangent = current.inTangent;
						}
					}
					if (i < curveLength - 1) {
						var next = curveKeys[i + 1];
						float deltaTime = next.time - current.time;
						float deltaValue = next.value - current.value;
						current.outTangent = deltaTime < Mathf.Epsilon ? 0 : deltaValue / deltaTime;
						if (i == 0) {
							current.inTangent = current.outTangent;
						}
					}
					curve.MoveKey(i, current);
				}
			}
		}
		protected static void ConstantTangent(AnimationCurve curve) {
			var curveKeys = curve.keys;
			var curveLength = curveKeys.Length;
			if (curveLength > 1) {
				for (int i = 0; i < curveLength; ++i) {
					var current = curveKeys[i];
					current.inTangent = Mathf.Infinity;
					current.outTangent = Mathf.Infinity;
					curve.MoveKey(i, current);
				}
			}
		}
		protected static void ClampedAutoTangent(AnimationCurve curve) {
#if UNITY_EDITOR
			var curveLength = curve.keys.Length;
			if (curveLength > 1) {
				for (int i = 0; i < curveLength; ++i) {
					UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, i, UnityEditor.AnimationUtility.TangentMode.ClampedAuto);
					UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, i, UnityEditor.AnimationUtility.TangentMode.ClampedAuto);
				}
			}
#endif
		}
	}
}
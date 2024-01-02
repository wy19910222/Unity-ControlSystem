/*
 * @Author: wangyun
 * @CreateTime: 2023-03-16 02:35:25 003
 * @LastEditor: wangyun
 * @EditTime: 2023-03-16 02:35:25 009
 */

using UnityEngine;
using Sirenix.OdinInspector;

namespace Control {
	[RequireComponent(typeof(RectTransform))]
	public class BaseProgressCtrlMinMaxVector2 : BaseProgressCtrl {
		[SerializeField, LimitedEditable]
		protected MinMaxVector2 m_FromValue;
		[SerializeField, LimitedEditable]
		protected MinMaxVector2 m_ToValue;
		public Vector2Part part = Vector2Part.XY;
		[SerializeField, CanResetCurve]
		[ShowIf("@((int) (part & Vector2Part.X)) != 0")]
		protected AnimationCurve m_CurveX = new AnimationCurve(new Keyframe(0, 0, 1, 1), new Keyframe(1, 1, 1, 1));
		[SerializeField, CanResetCurve]
		[ShowIf("@((int) (part & Vector2Part.Y)) != 0")]
		protected AnimationCurve m_CurveY = new AnimationCurve(new Keyframe(0, 0, 1, 1), new Keyframe(1, 1, 1, 1));
		
		private void Reset() {
			controller = GetComponentInParent<ProgressController>();
			m_ToValue = m_FromValue = TargetValue;
		}

		private MinMax GetValuePart(MinMaxVector2 value, int partIndex) {
			return value[partIndex];
		}
		private MinMaxVector2 SetValuePart(MinMaxVector2 value, int partIndex, MinMax valuePart) {
			value[partIndex] = valuePart;
			return value;
		}

		private AnimationCurve GetCurve(int partIndex) {
			return partIndex == 0 ? m_CurveX : m_CurveY;
		}
		private void SetCurve(int partIndex, AnimationCurve curve) {
			if (partIndex == 0) {
				m_CurveX = curve;
			} else {
				m_CurveY = curve;
			}
		}

		private MinMaxVector2 GetValue(float progress) {
			var ret = default(MinMaxVector2);
			for (int partIndex = 0; partIndex < 2; partIndex++) {
				var t = GetCurve(partIndex).Evaluate(progress);
				var valuePart = Lerp(GetValuePart(m_FromValue, partIndex), GetValuePart(m_ToValue, partIndex), t);
				ret = SetValuePart(ret, partIndex, valuePart);
			}
			return ret;
		}
		
		private void SetValue(float progress, MinMaxVector2 value) {
			for (int partIndex = 0; partIndex < 2; partIndex++) {
				var t = GetT(GetValuePart(m_FromValue, partIndex), GetValuePart(m_ToValue, partIndex), GetValuePart(value, partIndex));
				var curve = GetCurve(partIndex);
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
		private void AddKey(AnimationCurve curve, float progress, float t) {
			float curT = curve.Evaluate(progress);
			if (Mathf.Abs(curT - t) < Mathf.Epsilon) {
				const float deltaProgress = 0.001F;
				float tangent = (t - curve.Evaluate(progress - deltaProgress)) / deltaProgress;
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

		protected virtual MinMaxVector2 TargetValue { get; set; }

		protected MinMax Lerp(MinMax from, MinMax to, float t) {
			return MinMax.LerpUnclamped(from, to, t);
		}
		protected float GetT(MinMax from, MinMax to, MinMax value) {
			return to.Equals(value) ? 1 : from.Equals(value) ? 0 : (value.min - from.min) / (to.min - from.min);
		}
		protected bool Equals(MinMax value1, MinMax value2) {
			return value1.Equals(value2);
		}
		
		public override void Capture(float progress) {
			MinMaxVector2 targetValue = TargetValue;
			// 如果form和to相同，则把to设置成新记录的值，然后曲线上所有点的纵坐标都设置为0
			for (int partIndex = 0; partIndex < 2; partIndex++) {
				if (Equals(GetValuePart(m_FromValue, partIndex), GetValuePart(m_ToValue, partIndex))) {
					m_ToValue = SetValuePart(m_ToValue, partIndex, GetValuePart(targetValue, partIndex));
					AnimationCurve curve = GetCurve(partIndex);
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
			ScaleCurve();
#endif
		}
		private void ScaleCurve() {
			for (int partIndex = 0; partIndex < 2; partIndex++) {
				AnimationCurve curve = GetCurve(partIndex);
				var curveKeys = curve.keys;
				var curveLength = curveKeys.Length;
				if (curveLength > 0) {
					if (curveLength == 1) {
						var keyFrame = curveKeys[0];
						if (keyFrame.time != 0 || keyFrame.value != 0) {
							// 把唯一的点重置成(0, 0)
							MinMax from = GetValuePart(m_FromValue, partIndex), to = GetValuePart(m_ToValue, partIndex);
							m_FromValue = SetValuePart(m_FromValue, partIndex, Lerp(from, to, keyFrame.value));
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
						SetCurve(partIndex, curve = new AnimationCurve());
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
						MinMax from = GetValuePart(m_FromValue, partIndex), to = GetValuePart(m_ToValue, partIndex);
						m_FromValue = SetValuePart(m_FromValue, partIndex, Lerp(from, to, minT));
						m_ToValue = SetValuePart(m_ToValue, partIndex, Lerp(from, to, maxT));
					}
				}
			}
		}

		public override void Apply(float progress) {
			TargetValue = GetValue(progress);
		}
	}
}
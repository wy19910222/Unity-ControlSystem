/*
 * @Author: wangyun
 * @CreateTime: 2023-01-20 04:30:52 032
 * @LastEditor: wangyun
 * @EditTime: 2023-01-20 04:30:52 038
 */

using System.Collections.Generic;
using UnityEngine;

namespace Control {
	public abstract class BaseProgressCtrlCurve : BaseProgressCtrl<AnimationCurve> {
		protected override AnimationCurve Lerp(AnimationCurve from, AnimationCurve to, float t) {
			return CurveLerpUtils.CurveLerpUnclamped(from, to, t);
		}

		protected override float GetT(AnimationCurve from, AnimationCurve to, AnimationCurve value) {
			List<float> tArray = new List<float>();
			Keyframe[] fromKeys = m_Curve.keys;
			for (int index = 0, length = fromKeys.Length; index < length; index++) {
				Keyframe fromKeyframe = fromKeys[index];
				float fromValue = fromKeyframe.value;
				float toValue = to.Evaluate(fromKeyframe.time);
				float valueValue = value.Evaluate(fromKeyframe.time);
				tArray.Add((valueValue - fromValue) / (toValue - fromValue));
			}
			Keyframe[] toKeys = m_Curve.keys;
			for (int index = 0, length = toKeys.Length; index < length; index++) {
				Keyframe toKeyframe = toKeys[index];
				float fromValue = from.Evaluate(toKeyframe.time);
				float toValue = toKeyframe.value;
				float valueValue = value.Evaluate(toKeyframe.time);
				tArray.Add((valueValue - fromValue) / (toValue - fromValue));
			}
			float sumT = 0;
			foreach (float t in tArray) {
				sumT += t;
			}
			return sumT / tArray.Count;
		}

		protected override bool Equals(AnimationCurve value1, AnimationCurve value2) {
			return value1?.Equals(value2) ?? value2?.Equals(null) ?? true;
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
}
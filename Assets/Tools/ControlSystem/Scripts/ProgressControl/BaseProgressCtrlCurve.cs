/*
 * @Author: wangyun
 * @CreateTime: 2023-01-20 04:30:52 032
 * @LastEditor: wangyun
 * @EditTime: 2023-01-20 04:30:52 038
 */

using UnityEngine;

namespace Control {
	public abstract class BaseProgressCtrlCurve : BaseProgressCtrl<AnimationCurve> {
		protected float m_T;
		
		protected override AnimationCurve Lerp(AnimationCurve from, AnimationCurve to, float t) {
			return CurveLerpUtils.CurveLerpUnclamped(from, to, t);
		}

		protected override float GetT(AnimationCurve from, AnimationCurve to, AnimationCurve value) {
			return m_T;
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
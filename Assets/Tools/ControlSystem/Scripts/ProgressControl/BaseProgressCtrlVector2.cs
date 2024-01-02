/*
 * @Author: wangyun
 * @CreateTime: 2022-04-18 21:23:39 603
 * @LastEditor: wangyun
 * @EditTime: 2022-06-15 20:23:06 965
 */

using UnityEngine;
using Sirenix.OdinInspector;

namespace Control {
	public abstract class BaseProgressCtrlVector2 : BaseProgressCtrlFloats<Vector2> {
		[ShowIf("@PartCtrl")]
		public Vector2Part part = Vector2Part.XY;
		[SerializeField, CanResetCurve]
		[ShowIf("@!PartCtrl || ((int) (part & Vector2Part.X)) != 0")]
		protected AnimationCurve m_CurveX = new AnimationCurve(new Keyframe(0, 0, 1, 1), new Keyframe(1, 1, 1, 1));
		[SerializeField, CanResetCurve]
		[ShowIf("@!PartCtrl || ((int) (part & Vector2Part.Y)) != 0")]
		protected AnimationCurve m_CurveY = new AnimationCurve(new Keyframe(0, 0, 1, 1), new Keyframe(1, 1, 1, 1));

		protected virtual bool PartCtrl => true;
		
		protected override int PartCount => 2;
		
		protected override float GetValuePart(Vector2 value, int partIndex) {
			return value[partIndex];
		}
		protected override Vector2 SetValuePart(Vector2 value, int partIndex, float valuePart) {
			value[partIndex] = valuePart;
			return value;
		}

		protected override AnimationCurve GetCurve(int partIndex) {
			return partIndex == 0 ? m_CurveX : m_CurveY;
		}
		protected override void SetCurve(int partIndex, AnimationCurve curve) {
			if (partIndex == 0) {
				m_CurveX = curve;
			} else {
				m_CurveY = curve;
			}
		}

		[ContextMenu("LinearTangent")]
		protected void LinearTangent() {
			LinearTangent(m_CurveX);
			LinearTangent(m_CurveY);
		}
		[ContextMenu("ConstantTangent")]
		protected void ConstantTangent() {
			ConstantTangent(m_CurveX);
			ConstantTangent(m_CurveY);
		}
		[ContextMenu("ClampedAutoTangent")]
		protected void ClampedAutoTangent() {
			ClampedAutoTangent(m_CurveX);
			ClampedAutoTangent(m_CurveY);
		}
	}
}
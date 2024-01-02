/*
 * @Author: wangyun
 * @CreateTime: 2022-04-18 14:22:34 542
 * @LastEditor: wangyun
 * @EditTime: 2022-06-20 00:21:40 015
 */

using UnityEngine;
using Sirenix.OdinInspector;

namespace Control {
	public abstract class BaseProgressCtrlVector3 : BaseProgressCtrlFloats<Vector3> {
		[ShowIf("@PartCtrl")]
		public Vector3Part part = Vector3Part.XYZ;
		[SerializeField, CanResetCurve]
		[ShowIf("@!PartCtrl || ((int) (part & Vector3Part.X)) != 0")]
		protected AnimationCurve m_CurveX = new AnimationCurve(new Keyframe(0, 0, 1, 1), new Keyframe(1, 1, 1, 1));
		[SerializeField, CanResetCurve]
		[ShowIf("@!PartCtrl || ((int) (part & Vector3Part.Y)) != 0")]
		protected AnimationCurve m_CurveY = new AnimationCurve(new Keyframe(0, 0, 1, 1), new Keyframe(1, 1, 1, 1));
		[SerializeField, CanResetCurve]
		[ShowIf("@!PartCtrl || ((int) (part & Vector3Part.Z)) != 0")]
		protected AnimationCurve m_CurveZ = new AnimationCurve(new Keyframe(0, 0, 1, 1), new Keyframe(1, 1, 1, 1));

		protected virtual bool PartCtrl => true;
		
		protected override int PartCount => 3;
		
		protected override float GetValuePart(Vector3 value, int partIndex) {
			return value[partIndex];
		}
		protected override Vector3 SetValuePart(Vector3 value, int partIndex, float valuePart) {
			value[partIndex] = valuePart;
			return value;
		}

		protected override AnimationCurve GetCurve(int partIndex) {
			switch (partIndex) {
				case 0:
					return m_CurveX;
				case 1:
					return m_CurveY;
				default:
					return m_CurveZ;
			}
		}
		protected override void SetCurve(int partIndex, AnimationCurve curve) {
			switch (partIndex) {
				case 0:
					m_CurveX = curve;
					break;
				case 1:
					m_CurveY = curve;
					break;
				default:
					m_CurveZ = curve;
					break;
			}
		}

		[ContextMenu("LinearTangent")]
		protected void LinearTangent() {
			LinearTangent(m_CurveX);
			LinearTangent(m_CurveY);
			LinearTangent(m_CurveZ);
		}
		[ContextMenu("ConstantTangent")]
		protected void ConstantTangent() {
			ConstantTangent(m_CurveX);
			ConstantTangent(m_CurveY);
			ConstantTangent(m_CurveZ);
		}
		[ContextMenu("ClampedAutoTangent")]
		protected void ClampedAutoTangent() {
			ClampedAutoTangent(m_CurveX);
			ClampedAutoTangent(m_CurveY);
			ClampedAutoTangent(m_CurveZ);
		}
	}
}
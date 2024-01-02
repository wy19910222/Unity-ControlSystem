/*
 * @Author: wangyun
 * @CreateTime: 2022-04-18 14:22:34 542
 * @LastEditor: wangyun
 * @EditTime: 2022-06-20 00:06:49 671
 */

using UnityEngine;
using Sirenix.OdinInspector;

namespace Control {
	public abstract class BaseProgressCtrlColor : BaseProgressCtrlFloats<Color> {
		[ShowIf("@PartCtrl")]
		public ColorPart part = ColorPart.RGB;
		[SerializeField, CanResetCurve]
		[ShowIf("@!PartCtrl || ((int) (part & ColorPart.R)) != 0")]
		protected AnimationCurve m_CurveR = new AnimationCurve(new Keyframe(0, 0, 1, 1), new Keyframe(1, 1, 1, 1));
		[SerializeField, CanResetCurve]
		[ShowIf("@!PartCtrl || ((int) (part & ColorPart.G)) != 0")]
		protected AnimationCurve m_CurveG = new AnimationCurve(new Keyframe(0, 0, 1, 1), new Keyframe(1, 1, 1, 1));
		[SerializeField, CanResetCurve]
		[ShowIf("@!PartCtrl || ((int) (part & ColorPart.B)) != 0")]
		protected AnimationCurve m_CurveB = new AnimationCurve(new Keyframe(0, 0, 1, 1), new Keyframe(1, 1, 1, 1));
		[SerializeField, CanResetCurve]
		[ShowIf("@!PartCtrl || ((int) (part & ColorPart.A)) != 0")]
		protected AnimationCurve m_CurveA = new AnimationCurve(new Keyframe(0, 0, 1, 1), new Keyframe(1, 1, 1, 1));

		protected virtual bool PartCtrl => true;

		protected override int PartCount => 4;
		
		protected override float GetValuePart(Color value, int partIndex) {
			return value[partIndex];
		}
		protected override Color SetValuePart(Color value, int partIndex, float valuePart) {
			value[partIndex] = valuePart;
			return value;
		}
		
		protected override AnimationCurve GetCurve(int partIndex) {
			switch (partIndex) {
				case 0:
					return m_CurveR;
				case 1:
					return m_CurveG;
				case 2:
					return m_CurveB;
				default:
					return m_CurveA;
			}
		}
		protected override void SetCurve(int partIndex, AnimationCurve curve) {
			switch (partIndex) {
				case 0:
					m_CurveR = curve;
					break;
				case 1:
					m_CurveG = curve;
					break;
				case 2:
					m_CurveB = curve;
					break;
				default:
					m_CurveA = curve;
					break;
			}
		}

		[ContextMenu("LinearTangent")]
		protected void LinearTangent() {
			LinearTangent(m_CurveR);
			LinearTangent(m_CurveG);
			LinearTangent(m_CurveB);
			LinearTangent(m_CurveA);
		}
		[ContextMenu("ConstantTangent")]
		protected void ConstantTangent() {
			ConstantTangent(m_CurveR);
			ConstantTangent(m_CurveG);
			ConstantTangent(m_CurveB);
			ConstantTangent(m_CurveA);
		}
		[ContextMenu("ClampedAutoTangent")]
		protected void ClampedAutoTangent() {
			ClampedAutoTangent(m_CurveR);
			ClampedAutoTangent(m_CurveG);
			ClampedAutoTangent(m_CurveB);
			ClampedAutoTangent(m_CurveA);
		}
	}
}

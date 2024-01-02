/*
 * @Author: wangyun
 * @CreateTime: 2023-01-20 04:39:08 543
 * @LastEditor: wangyun
 * @EditTime: 2023-01-28 22:06:06 523
 */

using Sirenix.OdinInspector;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace Control {
	public abstract class BaseProgressCtrlMinMaxCurve : BaseProgressCtrl {
#if UNITY_EDITOR
		[OnInspectorGUI("DrawEditButton", false)]
#endif
		[SerializeField, EnableIf("@UnityEditor.EditorPrefs.GetBool(\"BaseProgressCtrlMinMaxCurve.ValueEditable\")")]
		private MinMaxCurve m_FromValue;
		[SerializeField, EnableIf("@UnityEditor.EditorPrefs.GetBool(\"BaseProgressCtrlMinMaxCurve.ValueEditable\")")]
		private MinMaxCurve m_ToValue;
		[SerializeField, CanResetCurve]
		protected AnimationCurve m_Curve = new AnimationCurve(new Keyframe(0, 0, 1, 1), new Keyframe(1, 1, 1, 1));

		protected virtual void Reset() {
			controller = GetComponentInParent<ProgressController>();
			m_ToValue = m_FromValue = TargetValue;
		}

		protected MinMaxCurve GetValue(float progress) {
			var t = m_Curve.Evaluate(progress);
			return Lerp(m_FromValue, m_ToValue, t);
		}

		protected void SetValue(float progress, MinMaxCurve value) {
			var t = GetT(m_FromValue, m_ToValue, value);
			// 因为无法获得大于0小于1的t，所以只允许设置0或1的值，其他值需要自己手动拖曲线
			if (Mathf.Approximately(t, 0) || Mathf.Approximately(t, 1)) {
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
		}
		protected void AddKey(float progress, float t) {
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

		protected virtual MinMaxCurve TargetValue { get; set; }

		protected MinMaxCurve Lerp(MinMaxCurve from, MinMaxCurve to, float t) {
			return CurveLerpUtils.MinMaxCurveLerpUnclamped(from, to, t);
		}
		protected float GetT(MinMaxCurve from, MinMaxCurve to, MinMaxCurve value) {
			return to.Equals(value) ? 1 : from.Equals(value) ? 0 : 0.5F;
		}
		protected bool Equals(MinMaxCurve value1, MinMaxCurve value2) {
			return value1.Equals(value2);
		}

		public override void Capture(float progress) {
			MinMaxCurve targetValue = TargetValue;
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
		private void ScaleCurve() {
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
			TargetValue = GetValue(progress);
		}
		
#if UNITY_EDITOR
		protected void DrawEditButton() {
			UnityEditor.EditorGUILayout.BeginHorizontal();
			bool prevEnabled = GUI.enabled;
			GUI.enabled = true;
			bool valueEditable = UnityEditor.EditorPrefs.GetBool("BaseProgressCtrlMinMaxCurve.ValueEditable");
			if (CustomEditorGUI.Toggle(valueEditable, "编辑初值末值", CustomEditorGUI.COLOR_TOGGLE_CHECKED_EDITOR) != valueEditable) {
				UnityEditor.EditorPrefs.SetBool("BaseProgressCtrlMinMaxCurve.ValueEditable", !valueEditable);
			}
			if (GUILayout.Button("当前为初值")) {
				m_FromValue = TargetValue;
			}
			if (GUILayout.Button("当前为末值")) {
				m_ToValue = TargetValue;
			}
			GUI.enabled = prevEnabled;
			UnityEditor.EditorGUILayout.EndHorizontal();
		}
#endif
	}
}
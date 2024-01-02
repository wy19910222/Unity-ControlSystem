/*
 * @Author: wangyun
 * @CreateTime: 2022-07-22 00:45:33 469
 * @LastEditor: wangyun
 * @EditTime: 2022-07-22 00:45:33 473
 */

using System;
using UnityEngine;
using Sirenix.OdinInspector;

using URandom = UnityEngine.Random;

namespace Control {
	public enum TriggerCtrlRandomTransType {
		NONE,
		LOCAL_POSITION = 1,
		LOCAL_ANGLES = 2,
		LOCAL_SCALE = 3
	}

	[Flags]
	public enum TriggerCtrlRandomTransPart {
		X = 1 << 0,
		Y = 1 << 1,
		Z = 1 << 2,
		XYZ = X | Y | Z
	}
	
	public class TriggerCtrlRandomTrans : TriggerCtrlTrigger {
		public TriggerCtrlRandomTransType type = TriggerCtrlRandomTransType.LOCAL_POSITION;
		public TriggerCtrlRandomTransPart part = TriggerCtrlRandomTransPart.XYZ;
		public Vector3 min;
		public Vector3 max;
		[ShowIf("@((int) part & (int) part - 1) != 0")]
		public bool uniform;
		
		protected override void DoTrigger() {
			Vector3 value = uniform ? Vector3.Lerp(min, max, URandom.Range(0F, 1F))
					: new Vector3(URandom.Range(min.x, max.x), URandom.Range(min.y, max.y), URandom.Range(min.z, max.z));
			switch (type) {
				case TriggerCtrlRandomTransType.LOCAL_POSITION:
					transform.localPosition = SetValue(transform.localPosition, value);
					break;
				case TriggerCtrlRandomTransType.LOCAL_ANGLES:
					transform.localEulerAngles = SetValue(transform.localEulerAngles, value);
					break;
				case TriggerCtrlRandomTransType.LOCAL_SCALE:
					transform.localScale = SetValue(transform.localScale, value);
					break;
			}
		}
		
		private Vector3 SetValue(Vector3 v3, Vector3 value) {
			if ((part & TriggerCtrlRandomTransPart.X) != 0) {
				v3.x = value.x;
			}
			if ((part & TriggerCtrlRandomTransPart.Y) != 0) {
				v3.y = value.y;
			}
			if ((part & TriggerCtrlRandomTransPart.Z) != 0) {
				v3.z = value.z;
			}
			return v3;
		}
	}
}
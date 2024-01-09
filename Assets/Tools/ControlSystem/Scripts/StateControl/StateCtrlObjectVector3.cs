/*
 * @Author: wangyun
 * @CreateTime: 2024-01-09 15:59:55 206
 * @LastEditor: wangyun
 * @EditTime: 2024-01-09 15:59:55 210
 */

using UnityEngine;
using UnityEngine.Events;

namespace Control {
	public class StateCtrlObjectVector3 : BaseStateCtrl<Vector3> {
		public Vector3 valueTemp;
		public UnityEvent<Vector3> unityEvent;

		protected override Vector3 TargetValue {
			get => valueTemp;
			set {
				valueTemp = value;
				unityEvent.Invoke(value);
			}
		}
	}
}
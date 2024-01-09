/*
 * @Author: wangyun
 * @CreateTime: 2024-01-09 15:24:28 044
 * @LastEditor: wangyun
 * @EditTime: 2024-01-09 15:24:28 055
 */

using UnityEngine;
using UnityEngine.Events;

namespace Control {
	public class ProgressCtrlObjectVector3 : BaseProgressCtrlVector3 {
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
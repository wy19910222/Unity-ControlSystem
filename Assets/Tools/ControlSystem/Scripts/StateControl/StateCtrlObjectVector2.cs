/*
 * @Author: wangyun
 * @CreateTime: 2024-01-09 15:59:31 581
 * @LastEditor: wangyun
 * @EditTime: 2024-01-09 15:59:31 586
 */

using UnityEngine;
using UnityEngine.Events;

namespace Control {
	public class StateCtrlObjectVector2 : BaseStateCtrl<Vector2> {
		public Vector2 valueTemp;
		public UnityEvent<Vector2> unityEvent;

		protected override Vector2 TargetValue {
			get => valueTemp;
			set {
				valueTemp = value;
				unityEvent.Invoke(value);
			}
		}
	}
}
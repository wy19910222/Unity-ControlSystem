/*
 * @Author: wangyun
 * @CreateTime: 2024-01-09 15:55:34 454
 * @LastEditor: wangyun
 * @EditTime: 2024-01-09 15:55:34 465
 */

using UnityEngine.Events;

namespace Control {
	public class StateCtrlObjectBool : BaseStateCtrl<bool> {
		public bool valueTemp;
		public UnityEvent<bool> unityEvent;

		protected override bool TargetValue {
			get => valueTemp;
			set {
				valueTemp = value;
				unityEvent.Invoke(value);
			}
		}
	}
}
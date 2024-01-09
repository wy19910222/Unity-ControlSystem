/*
 * @Author: wangyun
 * @CreateTime: 2024-01-09 15:57:43 464
 * @LastEditor: wangyun
 * @EditTime: 2024-01-09 15:57:43 469
 */

using UnityEngine.Events;

namespace Control {
	public class StateCtrlObjectInt : BaseStateCtrl<int> {
		public int valueTemp;
		public UnityEvent<int> unityEvent;

		protected override int TargetValue {
			get => valueTemp;
			set {
				valueTemp = value;
				unityEvent.Invoke(value);
			}
		}
	}
}
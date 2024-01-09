/*
 * @Author: wangyun
 * @CreateTime: 2024-01-09 16:00:54 658
 * @LastEditor: wangyun
 * @EditTime: 2024-01-09 16:00:54 662
 */

using UnityEngine.Events;

namespace Control {
	public class StateCtrlObjectString : BaseStateCtrl<string> {
		public string valueTemp;
		public UnityEvent<string> unityEvent;

		protected override string TargetValue {
			get => valueTemp;
			set {
				valueTemp = value;
				unityEvent.Invoke(value);
			}
		}
	}
}
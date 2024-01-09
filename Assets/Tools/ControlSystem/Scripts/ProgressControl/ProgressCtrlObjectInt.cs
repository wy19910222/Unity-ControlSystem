/*
 * @Author: wangyun
 * @CreateTime: 2024-01-09 15:23:51 996
 * @LastEditor: wangyun
 * @EditTime: 2024-01-09 15:23:52 000
 */

using UnityEngine.Events;

namespace Control {
	public class ProgressCtrlObjectInt : BaseProgressCtrlInt {
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
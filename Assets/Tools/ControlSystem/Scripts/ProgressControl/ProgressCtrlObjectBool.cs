/*
 * @Author: wangyun
 * @CreateTime: 2024-01-09 15:23:59 464
 * @LastEditor: wangyun
 * @EditTime: 2024-01-09 15:23:59 468
 */

using UnityEngine.Events;

namespace Control {
	public class ProgressCtrlObjectBool : BaseProgressCtrlConst<bool> {
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
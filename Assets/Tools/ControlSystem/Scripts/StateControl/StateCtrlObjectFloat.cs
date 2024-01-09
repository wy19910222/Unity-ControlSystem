/*
 * @Author: wangyun
 * @CreateTime: 2024-01-09 15:57:11 079
 * @LastEditor: wangyun
 * @EditTime: 2024-01-09 15:57:11 082
 */

using UnityEngine.Events;

namespace Control {
	public class StateCtrlObjectFloat : BaseStateCtrl<float> {
		public float valueTemp;
		public UnityEvent<float> unityEvent;

		protected override float TargetValue {
			get => valueTemp;
			set {
				valueTemp = value;
				unityEvent.Invoke(value);
			}
		}
	}
}
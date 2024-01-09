/*
 * @Author: wangyun
 * @CreateTime: 2024-01-09 15:23:42 000
 * @LastEditor: wangyun
 * @EditTime: 2024-01-09 15:23:42 004
 */

using UnityEngine.Events;

namespace Control {
	public class ProgressCtrlObjectFloat : BaseProgressCtrlFloat {
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